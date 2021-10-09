using System;

namespace Configurator.Utilities
{
    public interface IConsoleLogger
    {
        void Debug(string message);
        void Verbose(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Error(string message, Exception exception);
        void Progress(string message);
        void Result(string message);
    }

    public class ConsoleLogger : IConsoleLogger
    {
        public void Debug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Write("DEBUG", message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Info(string message)
        {
            Write("INFO ", message);
        }

        public void Verbose(string message)
        {
            Write("INFOV", message);
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Write("WARN ", message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Write("ERROR", message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Error(string message, Exception exception)
        {
            Error($"{message}\n{exception}");
        }

        public void Progress(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Write("PRGRS", message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Result(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Write("RESLT", message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void Write(string level, string message)
        {
            var logMessage = $"[{DateTimeOffset.Now:O}] [{level}] {message}";
            Console.WriteLine(logMessage);
        }
    }
}
