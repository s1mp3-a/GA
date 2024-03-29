﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    public class GeneticAlgorithm
    {
        public enum StopCondition
        {
            NumberOfIterations,
            BestResultDelta
        };

        private readonly Logger _logger;
        private readonly Random _random = new Random();
        private Population _population;
        private readonly Func<Floating[], double> _fitness;
        private double _crossoverProbability;
        private double _mutationProbability;

        public double BestFit => Best.Score(_fitness);
        public Individual Best { get; private set; }

        public GeneticAlgorithm(Func<Floating[], double> fitnessFunction, double crossProb, double mutProb, Logger logger)
        {
            _population = PopulationFabric.Create(100, new[] {-1.3d, 1.3d, 1.3d, 1.3d, 1.3d, 1.3d, 1.3d });
            _fitness = fitnessFunction;
            _crossoverProbability = crossProb;
            _mutationProbability = mutProb;
            _logger = logger;
        }

        /// <summary>
        /// Запуск генетического алгоритма
        /// </summary>
        /// <param name="numberOfGenerations">Количество поколений</param>
        public void Run(int numberOfGenerations)
        {
            var best = _population[0];

            for (int generation = 0; generation < numberOfGenerations ; generation++)
            {
                foreach (var member in _population)
                {
                    double memberScore = member.Score(_fitness);
                    double bestScore = best.Score(_fitness);
                    if(memberScore < bestScore)
                    {
                        best = member.Clone() as Individual;
                    }
                }

                var selected = _population.Select(m => Tournament(m, 2)).ToArray();

                List<Individual> newPop = new List<Individual>(_population.Size);

                for (int i = 0; i < _population.Size; i+= 2)
                {
                    var parent1 = selected[i];
                    var parent2 = selected[i+1];

                    var children = CrossOver(parent1, parent2);

                    children.c1.Mutate(_mutationProbability, _random);
                    children.c2.Mutate(_mutationProbability, _random);

                    newPop.Add(children.c1);
                    newPop.Add(children.c2);
                }

                this._population.UpdatePopulation(newPop);
                _logger.Log(GetGenerationLogMessage(generation, best));
            }

            Best = best;
        }

        /// <summary>
        /// Формирование лога с информацией о поколении
        /// </summary>
        /// <param name="generation">Номер поколения</param>
        /// <param name="best">Лучшая особь</param>
        /// <returns></returns>
        private string GetGenerationLogMessage(int generation, Individual best)
        {
            var individualInfo = string.Join("\r\n", best.Values.Select((x, i) => $"{(ValueTag)i} = {x.Value:0.0#####}"));

            return $"\r\nПоколение {generation + 1} | Ошибка = {best.Score(_fitness)}\r\n{individualInfo}\r\n";
        }

        /// <summary>
        /// Скрещивание особей со случайной вероятностью
        /// </summary>
        private (Individual c1, Individual c2) CrossOver(Individual p1, Individual p2)
        {
            var c1 = p1;
            var c2 = p2;

            if(_random.NextDouble() < _crossoverProbability)
            {
                c1 = p1.CrossOver(p2, _random);
                c2 = p2.CrossOver(p1, _random);
            }
            
            return (c1, c2);
        }

        /// <summary>
        /// Отбор лучшей особи
        /// </summary>
        private Individual Tournament(Individual individual, int tournamentSize)
        {
            var selectionIdx = _random.Next(_population.Size);
            var indices = Enumerable.Range(0, tournamentSize).Select(i => _random.Next(_population.Size)).ToArray();

            var selectionScore = _population[selectionIdx].Score(_fitness);

            foreach (var index in indices)
            {
                var testScore = _population[index].Score(_fitness);
                if(testScore < selectionScore)
                {
                    selectionIdx = index;
                    selectionScore = testScore;
                }
            }

            return _population[selectionIdx];
        }
    }
}
