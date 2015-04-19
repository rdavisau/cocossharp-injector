using System;
using Splat;

namespace CocosInjector.DebugApp
{
    internal class ConsoleLogger : ILogger
    {
        public LogLevel Level { get; set; }

        public void Write(string message, LogLevel logLevel)
        {
            Console.WriteLine(message);
        }
    }
}