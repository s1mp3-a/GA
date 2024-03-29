﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GA
{
    /// <summary>
    /// Теги для значений функции
    /// </summary>
    public enum ValueTag
    {
        p0,
        l,
        b,
        x0,
        y0,
        m,
        n
    }

    public static class Program
    {
        private const string DefaultGAConfigFilePath = "ga_cfg.json";

        private const string DefaultCfgFilePath = "path.cfg";

        private const string DefaultInputPath = ".\\IO\\input.json";
        private const string DefaultOutputPath = ".\\IO\\plot_input.json";

        private const string DefaultIOLoggerPath = ".\\log\\io.log";
        private const string DefaultAlgoLoggerPath = ".\\log\\algorithm.log";

        private const string DefaultPlotExePath = "OpenTK__Plotter.exe";

        private static Logger _ioLogger;

        private static InputData _inputData;

        /// <summary>
        /// Точка входа программы
        /// </summary>
        /// <param name="args">Массив, где первый элемент - путь к файлу с входными данными</param>
        static void Main(string[] args)
        {
            _ioLogger = new Logger(DefaultIOLoggerPath);
            var generationsLogger = new Logger(DefaultAlgoLoggerPath, false);

            FloatingSettings.IntPartSize = 3;
            FloatingSettings.FracPartSize = 10;
            FloatingSettings.IsUnsigned = true;

            var filePath = GetInputFilePath();
            _inputData = GetInputData(filePath);

            if(_inputData == null)
                return;

            var gaConfig = GetGAConfig();

            if (gaConfig == null)
                return;

            GeneticAlgorithm algo;

            algo = new GeneticAlgorithm(Function, gaConfig.CrossoverProbability, gaConfig.MutationProbability, generationsLogger);
            algo.Run(gaConfig.Generations);

            WriteOutputData(algo);

            _ioLogger.Log($"Готово! Результат находится в {DefaultOutputPath}");

            OpenPlotter();
        }

        static void OpenPlotter()
        {
            try
            {
                Process.Start(DefaultPlotExePath);
            }
            catch
            {
                _ioLogger.Log($"Не найден визуализатор по пути {DefaultPlotExePath}");
            }
        }

        /// <summary>
        /// Получение входных данных
        /// </summary>
        /// <param name="filePath">Путь к файлу с входными данными</param>
        static InputData GetInputData(string filePath)
        {
            string inputJson;
            InputData inputData = null;

            try
            {
                _ioLogger.Log($"Взятие исходных данных из {filePath}"); 
                inputJson = File.ReadAllText(filePath);
                inputData = JsonConvert.DeserializeObject<InputData>(inputJson); // десериализация входных данных
            }
            catch
            {
            }

            if (inputData?.E == null || inputData?.Phi == null)
            {
                _ioLogger.Log($"Не удалось прочитать исходные данные из {filePath}!");

                if (DefaultInputPath != filePath)
                    inputData = GetInputData(DefaultInputPath);
            }

            return inputData;
        }

        static GAConfig GetGAConfig()
        {
            string configJson;
            GAConfig config = null;

            try
            {
                _ioLogger.Log($"Взятие настроек из {DefaultGAConfigFilePath}");
                configJson = File.ReadAllText(DefaultGAConfigFilePath);
                config = JsonConvert.DeserializeObject<GAConfig>(configJson);
            }
            catch
            {
                _ioLogger.Log($"Не удалось взять настройки из {DefaultGAConfigFilePath}");
            }

            return config;
        }

        /// <summary>
        /// Получение пути к файлу с входными данными
        /// </summary>
        /// <returns></returns>
        static string GetInputFilePath()
        {
            try
            {
                return File.ReadAllText(DefaultCfgFilePath);
            }
            catch
            {
                _ioLogger.Log("path.cfg не найден!");
                return DefaultInputPath;
            }
        }

        /// <summary>
        /// Запись результатов
        /// </summary>
        static void WriteOutputData(GeneticAlgorithm algo)
        {
            var output = new OutputData
            {
                FunctionValue = algo.BestFit,
                Values = algo.Best.Values.Select((x, i) => new ValueWithTag
                {
                    Value = x.Value,
                    Tag = ((ValueTag)i).ToString()
                })
            };

            // сериализация и запись выходных данных
            using (var sw = new StreamWriter(DefaultOutputPath))
            {
                sw.Write(JsonConvert.SerializeObject(output, Formatting.Indented));
            }
        }

        static double Function(Floating[] fs)
        {
            var xs = fs.Select(f => f.Value).ToArray();

            var p0 = xs[(int)ValueTag.p0];
            var l = xs[(int)ValueTag.l];
            var b = xs[(int)ValueTag.b];
            var x0 = xs[(int)ValueTag.x0];
            var y0 = xs[(int)ValueTag.y0];
            var m = xs[(int)ValueTag.m];
            var n = xs[(int)ValueTag.n];

            return Enumerable.Range(0, _inputData.E.Length)
                .Select(i =>
                {
                    var e = _inputData.E[i];
                    var a = Math.Pow(1 - Math.Pow(Math.Abs(2 * (e.X - x0) / l), n), 1 / n) * Math.Pow(1 - Math.Pow(Math.Abs(2 * (e.Y - y0) / b), m), 1 / m);
                    a = double.IsNaN(a) ? 0 : a;
                    var res = Math.Pow(_inputData.Phi[i][e.Y][e.X] * p0 * a - e.Value, 2);
                    return res;
                })
                .Sum();

        }
    }
}
