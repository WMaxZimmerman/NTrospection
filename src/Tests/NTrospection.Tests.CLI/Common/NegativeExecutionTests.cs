using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NTrospection.Tests.CLI.Common
{
    [TestClass]
    public class NegativeExecutionTests : BaseCliTest
    {
        [TestMethod]
        public void AbleToHandleMissingAllAguments()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                $"Please enter a controller. Use '{helpString}' to see available controllers."
            };
            _processor.ProcessArguments(new string[] { });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToHandleMissingAllAgumentsAfterController()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                $"'' is not a valid command.  Use '{helpString}' to see available commands."
            };
            _processor.ProcessArguments(new[] { "execute" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToHandleMissingParameters()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "The parameter 'sample' must be specified"
            };
            _processor.ProcessArguments(new[] { "execute", "example" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToHandleInvalidParameters()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "The parameter 'invalidParam' is not a valid parameter"
            };
            _processor.ProcessArguments(new[] { "execute", "example", $"{argPre}sample", "EnumOne", $"{argPre}invalidParam", "bad" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToHandleInvalidParameterValue()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "An error occurred while attempting to execute the command.",
                "This is most likely due to invalid arguments.",
                $"Please verify the command usage with '{helpString}' and try again."
            };
            _processor.ProcessArguments(new[] { "execute", "example", $"{argPre}sample", "Enum" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToHandleCallToCommandThatThrowsException()
        {
            mockConsole.Clear();

            var consoleLines = new List<string>
            {
                "An error occurred while executing the command.",
                "Message: I blew up yer thingy.",
                GetStackTraceForException($"App{Path.DirectorySeparatorChar}Controllers", "ExecutionController", "ThrowExceptionMethod", "SampleEnum sample", 28)
            };
            _processor.ProcessArguments(new[] { "execute", "exception", $"{argPre}sample", "EnumOne" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }
    }
}
