using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Core;
using NTrospection.CLI.Models;
using NTrospection.CLI.Services;
using NTrospection.CLI.Repositories;

namespace NTrospection.Tests.CLI.Services
{
    [TestClass]
    public class ArgumentServiceTests
    {
        private ArgumentService _service;

        private Mock<ISettings> _mockSettings;
        private Mock<IArgumentRepository> _mockArgService;
        private Mock<IControllerService> _mockControllerService;

        private string _help = "?";
        private string _pre = "-";
        
        [TestInitialize]
        public void Init()
        {
            _mockSettings = new Mock<ISettings>();
            _mockArgService = new Mock<IArgumentRepository>();
            _mockControllerService = new Mock<IControllerService>();
            
            _service = new ArgumentService(_mockSettings.Object,
                                           _mockArgService.Object);

            _mockSettings.Setup(s => s.HelpString()).Returns(_help);
            _mockSettings.Setup(s => s.ArgumentPrefix()).Returns(_pre);
        }

        [TestMethod]
        public void ProcessArgs_RetrunsNewProcessedArguments_WhenPassedEmptyArgs()
        {
            var args = new List<string>();
            var expected = new ProcessedArguments();

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ProcessArgs_SetsHelpCallTrue_WhenLastArgIsHelpString()
        {
            var args = new List<string>{ _help };
            var expected = true;

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual.IsHelpCall);
        }

        [TestMethod]
        public void ProcessArgs_SetsHelpCallFalse_WhenLastArgIsNotHelpString()
        {
            var args = new List<string>{ "foo" };
            var expected = false;

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual.IsHelpCall);
        }

        [TestMethod]
        public void ProcessArgs_SetsControllerToFirstArg_WhenPassedNonEmptyArgs()
        {
            var expected = "foo";
            var args = new List<string>{ expected };
            _mockArgService.Setup(s => s.TryGetArg(args, 0)).Returns(expected);

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual.Controller);
        }

        [TestMethod]
        public void ProcessArgs_SetsCommandToSecondArg_WhenPassedEnoughArgs()
        {
            
            var expected = "bar";
            var args = new List<string>{ "foo", expected };
            _mockArgService.Setup(s => s.TryGetArg(args, 1)).Returns(expected);

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual.Command);
        }

        [TestMethod]
        public void ProcessArgs_SetsCommandNull_WhenPassedLessThanTwoArgs()
        {
            
            string expected = null;
            var args = new List<string>{ "foo" };

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual.Command);
        }

        [TestMethod]
        public void ProcessArgs_SetsCommandNull_WhenPassedParameterAsSecondArg()
        {
            
            string expected = null;
            var args = new List<string>{ "foo", $"{_pre}bar" };

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual.Command);
        }

        [TestMethod]
        public void ProcessArgs_SetsArgument_WhenPassedParameters()
        {
            
            string expected = null;

            var paramList = new List<string>
            {
                $"{_pre}foo",
                "bar",
                $"{_pre}bar",
                "foo"
            };
            var args = new List<string>{ "foo", "bar" };
            args.AddRange(paramList);
            var expectedArgs = new List<CommandLineArgument>{ new CommandLineArgument()};
            _mockArgService.Setup(s => s.SetArguments(paramList)).Returns(expectedArgs);

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual.Command);
            _mockArgService.Verify(s => s.SetArguments(paramList), Times.Once());
            CollectionAssert.AreEqual(expectedArgs, actual.Arguments);
        }

        [TestMethod]
        public void ProcessArgs_SetsArgument_WhenPassedParametersAndNoCommand()
        {
            string expected = null;

            var paramList = new List<string>
            {
                $"{_pre}foo",
                "bar",
                $"{_pre}bar",
                "foo"
            };
            var args = new List<string>{ "foo" };
            args.AddRange(paramList);
            var expectedArgs = new List<CommandLineArgument>{ new CommandLineArgument()};
            _mockArgService.Setup(s => s.SetArguments(paramList)).Returns(expectedArgs);
            _mockArgService.Setup(s => s.TryGetArg(args, 1)).Returns($"{_pre}foo");

            var actual = _service.ProcessArgs(args);

            Assert.AreEqual(expected, actual.Command);
            _mockArgService.Verify(s => s.SetArguments(paramList), Times.Once());
            CollectionAssert.AreEqual(expectedArgs, actual.Arguments);
        }

        [TestMethod]
        public void ProcessArgs_DoesNotSetsArgument_WhenIsHelpCall()
        {
            var paramList = new List<string>
            {
                $"{_pre}foo",
                "bar",
                $"{_pre}bar",
                "foo"
            };
            var args = new List<string>{ "foo" };
            args.AddRange(paramList);
            args.Add(_help);
            var expectedArgs = new List<CommandLineArgument>{ new CommandLineArgument()};

            var actual = _service.ProcessArgs(args);

            _mockArgService.Verify(s => s.SetArguments(It.IsAny<List<string>>()), Times.Never());
        }
    }
}
