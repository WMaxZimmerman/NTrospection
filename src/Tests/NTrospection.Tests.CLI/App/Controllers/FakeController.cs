using NTrospection.CLI.Attributes;
using System;
using System.Collections.Generic;

namespace NTrospection.Tests.CLI.App.Controllers
{
    [CliController("fake", "This is a test description.")]
    public class FakeController
    {
        [CliCommand("example", "This is an example description.")]
        public void TestMethod(string required, int opt = 0)
        {
            Console.WriteLine($"{required} {opt}");
        }
        
        // === Fake Method For Reflection ===
        [CliCommand("Fake", "Fake Method For Reflection")]
        public void FakeMethod(
                               [CliParameter('f', "")]int foo,
                               string bar,
                               [CliParameter("")]bool foobar = false)
        {

        }

        [CliCommand("Fake", "Fake Method For Reflection")]
        public void FakeMethodNoParams()
        {

        }
        
        [CliCommand("Fake", "Fake Method For Reflection")]
        public static void FakeStaticMethod()
        {

        }

        // === Fake Method For Reflection ===
        public void FakeMethodNonCommand()
        {

        }

        // === Fake Method For Reflection ===
        [CliCommand("Fake", "Fake Method For Reflection")]
        public void FakeMethodWithCollections(List<int> foo, int[] bar)
        {

        }

    }
}
