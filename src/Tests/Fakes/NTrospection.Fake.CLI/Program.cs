using System;
using NTrospection.CLI.Core;

namespace NTrospection.Fake.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
	    var processor = new Processor();
            processor.ProcessArguments(args);
        }
    }
}
