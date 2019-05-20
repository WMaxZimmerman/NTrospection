using NTrospection.CLI.Common.Attributes;
using System;

namespace NTrospection.Tests.CLI.Common.Controllers
{
    [CliController("default", "This is a test description.")]
    public class DefaultCommandController
    {
        [CliDefaultCommand("bool", "This is an example description.")]
        public static void TestMethod(bool withOutput)
        {
            if (withOutput)
            {
                Console.WriteLine("Here is some output.");
            }
        }
    }
}
