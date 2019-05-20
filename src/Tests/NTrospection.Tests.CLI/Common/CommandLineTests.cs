using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI.Common;

namespace NTrospection.Tests.CLI.Common
{
    [TestClass]
    public class CommandLineTests
    {
        [TestMethod]
        public void GetCommandLineArgsAbleToHandleQuotes()
        {
            var inputString = "controller command --parameter \"value\" --paramaterTwo -2";
            var actualArgArray = CommandLine.GetCommandLineArgs(inputString);
            var expectedArgArray = new[] { "controller", "command", "--parameter", "value", "--paramaterTwo", "-2" };

            AssertArraysAreEqual(expectedArgArray, actualArgArray);
        }

        [TestMethod]
        public void GetCommandLineArgsAbleToHandleExtraWhiteSpace()
        {
            var inputString = " controller command --parameter   \"value\" --paramaterTwo -2";
            var actualArgArray = CommandLine.GetCommandLineArgs(inputString);
            var expectedArgArray = new[] { "controller", "command", "--parameter", "value", "--paramaterTwo", "-2" };

            AssertArraysAreEqual(expectedArgArray, actualArgArray);
        }

        [TestMethod]
        public void GetCommandLineArgsAbleToHandleMissingWhiteSpace()
        {
            var inputString = " controller command --parameter\"value\" --paramaterTwo -2";
            var actualArgArray = CommandLine.GetCommandLineArgs(inputString);
            var expectedArgArray = new[] { "controller", "command", "--parameter", "value", "--paramaterTwo", "-2" };

            AssertArraysAreEqual(expectedArgArray, actualArgArray);
        }

        [TestMethod]
        public void GetCommandLineArgsAbleToHandleSingleArgument()
        {
            var inputString = "?";
            var actualArgArray = CommandLine.GetCommandLineArgs(inputString);
            var expectedArgArray = new[] { "?" };

            AssertArraysAreEqual(expectedArgArray, actualArgArray);
        }

        private void AssertArraysAreEqual(string[] expected, string[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
