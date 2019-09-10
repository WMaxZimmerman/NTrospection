using System;

namespace NTrospection.CLI.Services
{
    public interface IConsoleService
    {
        void WriteLine(string message);
        void Write(string message);
        string ReadLine();
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

        public void Write(string message)
        {
            Console.Write(message);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
