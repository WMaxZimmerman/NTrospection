using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI;

namespace NTrospection.Tests.CLI.Common
{
    [TestClass]
    public class PositiveExecutionTests : BaseCliTest
    {
        [TestMethod]
        public void AbleToCallCommandWithoutOptionalParameter()
        {
            mockConsole.Clear();
            var consoleLines = new List<string> { "bleh 0" };
            Processor.ProcessArguments(new[] { "document", "example", $"{argPre}required", "bleh" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithOptionalParameter()
        {
            mockConsole.Clear();
            var consoleLines = new List<string> { "bleh 5" };
            Processor.ProcessArguments(new[] { "document", "example", $"{argPre}required", "bleh", $"{argPre}opt", "5" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithEnumParameter()
        {
            mockConsole.Clear();
            var consoleLines = new List<string> { "EnumOne" };
            Processor.ProcessArguments(new[] { "execute", "example", $"{argPre}sample", "EnumOne" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithArrayParameter()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "thingOne",
                "thingThree",
                "thingTwo",
                "3"
            };
            Processor.ProcessArguments(new[] { "execute", "array", $"{argPre}values", "thingOne", "thingThree", "thingTwo", $"{argPre}something", "3" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithParameterAlias()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "thingOne",
                "thingThree",
                "thingTwo",
                "3"
            };
            Processor.ProcessArguments(new[] { "execute", "enumerable", $"{argPre}values", "thingOne", "thingThree", "thingTwo", $"{argPre}s", "3" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithListParameter()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "EnumTwo",
                "EnumOne",
                "EnumThree",
                "3"
            };
            Processor.ProcessArguments(new[] { "execute", "list", $"{argPre}values", "EnumTwo", "EnumOne", "EnumThree", $"{argPre}something", "3" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithIEnumerableParameter()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "thingOne",
                "thingThree",
                "thingTwo",
                "3"
            };
            Processor.ProcessArguments(new[] { "execute", "enumerable", $"{argPre}values", "thingOne", "thingThree", "thingTwo", $"{argPre}something", "3" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithArrayParameterWithNoValues()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "3"
            };
            Processor.ProcessArguments(new[] { "execute", "array", $"{argPre}values", $"{argPre}something", "3" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithListParameterWithNoValues()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "3"
            };
            Processor.ProcessArguments(new[] { "execute", "list", $"{argPre}values", $"{argPre}something", "3" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithEnumerableParameterWithNoValues()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "3"
            };
            Processor.ProcessArguments(new[] { "execute", "enumerable", $"{argPre}values", $"{argPre}something", "3" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallCommandWithBooleanParameterWithNoValues()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "Here is some output."
            };
            Processor.ProcessArguments(new[] { "execute", "bool", $"{argPre}withOutput" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToCallNonStaticComamndWithEnum()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "EnumOne"
            };
            Processor.ProcessArguments(new[] { "execute", "nonstatic", $"{argPre}sample", "EnumOne" });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines);
            Assert.AreEqual(expectedString, temp);
        }
    }
}
