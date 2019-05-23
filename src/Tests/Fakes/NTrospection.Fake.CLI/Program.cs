using System;
using NTrospection.CLI.Common;

namespace NTrospection.Fake.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Processor.ProcessArguments(args);
        }
    }
}
