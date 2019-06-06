using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI.Core;

namespace NTrospection.Tests.CLI.Common
{
    [TestClass]
    public class DefaultCommandTest : BaseCliTest
    {
        [TestMethod]
        public void AbleToCallCommandWithName()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "Here is some output."
            };
            _processor.ProcessArguments(new[] { "default", "bool", $"{argPre}withOutput" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithoutName()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "Here is some output."
            };
            _processor.ProcessArguments(new[] { "default", $"{argPre}withOutput" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }
    }
}
