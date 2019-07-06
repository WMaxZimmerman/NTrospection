using NTrospection.CLI.Attributes;

namespace NTrospection.Tests.CLI.App.Controllers
{
    [CliController("fake", "This is a test description.")]
    public class FakeInstanceController
    {
        [CliCommand("one", "Fake Method For Reflection")]
        public void FakeMethodOne()
        {

        }
        [CliCommand("two", "Fake Method For Reflection")]
        public void FakeMethodTwo()
        {

        }
        [CliCommand("three", "Fake Method For Reflection")]
        public void FakeMethodThree()
        {

        }
    }
}
