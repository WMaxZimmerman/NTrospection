using NTrospection.CLI.Attributes;

namespace NTrospection.Tests.CLI.App.Controllers
{
    [CliController("fake", "This is a test description.")]
    public class FakeStaticController
    {
        [CliCommand("one", "Fake Method For Reflection")]
        public static void FakeMethodOne()
        {

        }
        [CliCommand("two", "Fake Method For Reflection")]
        public static void FakeMethodTwo()
        {

        }
        [CliCommand("three", "Fake Method For Reflection")]
        public static void FakeMethodThree()
        {

        }
    }
}
