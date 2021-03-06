using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI.Core;

namespace NTrospection.Tests.CLI.Common
{
    [TestClass]
    public class DocumentationTests : BaseCliTest
    {	
        [TestMethod]
        public void AbleToRetriveProgramDocumentation()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "default - This is a test description.",
                "document - This is a test description.",
                "execute - This is a test description.",
                "fake - This is a test description.",
                "fake - This is a test description.",
                "fake - This is a test description.",
                "sanscommand - A Controller with no commands for testing"
            };
            _processor.ProcessArguments(new[] { helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, false);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToRetriveControllerDocumentation()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "example",
                "Description: This is an example description.",
                "Parameters:",
                $"{argPre}required (String): This parameter is Required",
                $"{argPre}opt (Int32): This parameter is Optional"
            };
            _processor.ProcessArguments(new[] { "document", helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, true);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToRetriveCommandDocumentation()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "example",
                "Description: This is an example description.",
                "Parameters:",
                $"{argPre}required (String): This parameter is Required",
                $"{argPre}opt (Int32): This parameter is Optional"
            };
            _processor.ProcessArguments(new[] { "document", "example", helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, true);
            Assert.AreEqual(expectedString, temp);
        }

        //[TestMethod]
        //public void AbleToRetriveCommandDocumentationWithDetailedParams()
        //{
        //    SetParamDetail("detailed");
        //    mockConsole.Clear();
        //    var consoleLines = new List<string>
        //    {
        //        "example",
        //        "Description: This is an example description.",
        //        "Parameters:",
        //        $"{argPre}required (String): This parameter is Required.",
        //        $"{argPre}opt (Int32): This parameter is Optional with a default value of 0."
        //    };
        //    Processor.ProcessArguments(new[] { "document", "example", helpString });
        //    var temp = mockConsole.ToString();
        //    var expectedString = ConvertConsoleLinesToString(consoleLines, true);
        //    Assert.AreEqual(expectedString, temp);
        //}

        [TestMethod]
        public void AbleToRetriveCommandDocumentationWithEnum()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "example",
                "Description: This is an example description.",
                "Parameters:",
                $"{argPre}sample (SampleEnum): This parameter is Required and must be one of the following (EnumOne, EnumTwo, EnumThree)"
            };
            _processor.ProcessArguments(new[] { "execute", "example", helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, true);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToRetriveCommandDocumentationFromNonStaticMethodWithEnum()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "nonstatic",
                "Description: This is an example description.",
                "Parameters:",
                $"{argPre}sample (SampleEnum): This parameter is Required and must be one of the following (EnumOne, EnumTwo, EnumThree)"
            };
            _processor.ProcessArguments(new[] { "execute", "nonstatic", helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, true);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToRetriveCommandDocumentationWithListOfEnum()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "list",
                "Description: This is an example description.",
                "Parameters:",
                $"{argPre}values (List of SampleEnum): This parameter is Required and must be a collection of one of the following (EnumOne, EnumTwo, EnumThree)",
                $"{argPre}something (Int32): This parameter is Required"
            };
            _processor.ProcessArguments(new[] { "execute", "list", helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, true);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToRetriveCommandDocumentationWithAlias()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "enumerable",
                "Description: This is an example description.",
                "Parameters:",
                $"{argPre}values (List of String): This parameter is Required",
                $"{argPre}something | {argPre}s (Int32): This parameter is Required"
            };
            _processor.ProcessArguments(new[] { "execute", "enumerable", helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, true);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToRetriveCommandDocumentationWithoutAlias()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "without-alias",
                "Description: This is an example description.",
                "Parameters:",
                $"{argPre}something (Boolean): This parameter is Required",
                "Description: without-alias"
            };
            _processor.ProcessArguments(new[] { "execute", "without-alias", helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, true);
            Assert.AreEqual(expectedString, temp);
        }

        [TestMethod]
        public void AbleToRetriveCommandDocumentationWithAliasAndDescription()
        {
            mockConsole.Clear();
            var consoleLines = new List<string>
            {
                "array",
                "Description: This is an example description.",
                "Parameters:",
                $"{argPre}values (List of String): This parameter is Required",
                $"{argPre}something | {argPre}s (Int32): This parameter is Required",
                "Description: This parameter does something."
            };
            _processor.ProcessArguments(new[] { "execute", "array", helpString });
            var temp = mockConsole.ToString();
            var expectedString = ConvertConsoleLinesToString(consoleLines, true);
            Assert.AreEqual(expectedString, temp);
        }
    }
}
