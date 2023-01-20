using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    public class Logger
    {
        private bool _outputToConsole;
        private string _filePath;

        private bool _isBadPath = false;

        public Logger(string filePath, bool outputToConsole = true)
        {
            _filePath = filePath;
            _outputToConsole = outputToConsole;

            try
            {
                File.Delete(_filePath);
            }
            catch
            {
                Console.WriteLine($"Неверный путь для логирования: {_filePath}");
                _isBadPath = true;
            }
        }

        public void Log(string message)
        {
            if (_outputToConsole)
                Console.WriteLine(message);

            if (_isBadPath)
                return;

            File.AppendAllText(_filePath, $"{DateTime.Now}: {message}\r\n");
        }
    }
}
