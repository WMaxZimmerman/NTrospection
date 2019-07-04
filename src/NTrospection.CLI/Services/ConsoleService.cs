using System;

namespace NTrospection.CLI.Services
{
    public interface IConsoleService
    {
        void WriteLine(string message);
    }

    public class ConsoleService : IConsoleService
    {
        public ConsoleService()
        {
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
