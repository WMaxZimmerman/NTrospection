using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI.Services;

namespace NTrospection.Tests.CLI.Services
{
    [TestClass]
    public class ConsoleServiceTests
    {
        private StringWriter consoleMock;
        private StringBuilder mockConsole = new StringBuilder();
        private ConsoleService _service;

        [TestInitialize]
        public void Init()
        {
            consoleMock = new StringWriter(mockConsole);
            Console.SetOut(consoleMock);

            _service = new ConsoleService();
        }

        [TestMethod]
        public void WriteLine_OutputsLineToConsole()
        {
            var message = "hello world";
            var expectedMessage = message + Environment.NewLine;
            _service.WriteLine(message);

            Assert.AreEqual(expectedMessage, mockConsole.ToString());
        }
    }
}
