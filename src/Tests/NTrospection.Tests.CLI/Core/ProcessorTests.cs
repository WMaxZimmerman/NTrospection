using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Core;
using NTrospection.CLI.Models;
using NTrospection.CLI.Services;
using NTrospection.Tests.CLI.App.Controllers;

namespace NTrospection.Tests.CLI.Core
{
    [TestClass]
    public class ProcessorTests
    {
        private Processor _processor;

        private Mock<ISettings> _settings;
        private Mock<IControllerService> _controllerService;
        private Mock<IConsoleService> _console;
        private Mock<IArgumentProcessorService> _mockArgProcessor;
        private Mock<ILooper> _mockLooper;

        [TestInitialize]
        public void Init()
        {
            Environment.ExitCode = 0;
            _settings = new Mock<ISettings>();
            _controllerService = new Mock<IControllerService>();
            _console = new Mock<IConsoleService>();
            _mockArgProcessor = new Mock<IArgumentProcessorService>();
            _mockLooper = new Mock<ILooper>();
            
            _processor = new Processor(_settings.Object,
                                       _controllerService.Object,
                                       _console.Object,
                                       _mockArgProcessor.Object,
                                       _mockLooper.Object);
        }

        [TestMethod]
        public void GetAllControllers_ReturnsEmptyList_WhenNoTypesReturned()
        {
            var expected = new List<ControllerVm>();
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(m => m.GetTypes()).Returns(new Type[]{});

            var actual = _processor.GetAllControllers(mockAssembly.Object).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAllControllers_ReturnsEmptyList_WhenNoCliControllersReturned()
        {
            var expected = new List<ControllerVm>();
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(m => m.GetTypes()).Returns(new Type[]
                    {
                        typeof(NonController),
                        typeof(ProcessorTests)
                    });

            var actual = _processor.GetAllControllers(mockAssembly.Object).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAllControllers_ReturnsListOfControllers_WhenCliControllers()
        {
            var expected = new List<ControllerVm>()
                {
                    new ControllerVm(typeof(FakeController)),
                    new ControllerVm(typeof(DefaultCommandController))
                };
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(m => m.GetTypes()).Returns(new Type[]
                    {
                        typeof(FakeController),
                        typeof(DefaultCommandController)
                    });

            var actual = _processor.GetAllControllers(mockAssembly.Object).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAllControllers_OnlyReturnsListOfControllers_WhenCliControllers()
        {
            var expected = new List<ControllerVm>()
                {
                    new ControllerVm(typeof(FakeController)),
                    new ControllerVm(typeof(DefaultCommandController))
                };
            var mockAssembly = new Mock<Assembly>();
            mockAssembly.Setup(m => m.GetTypes()).Returns(new Type[]
                    {
                        typeof(FakeController),
                        typeof(DefaultCommandController),
                        typeof(NonController)
                    });

            var actual = _processor.GetAllControllers(mockAssembly.Object).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ProcessArguments_CallsProcessArguments_WhenArgsLengthGreaterThanZero()
        {
            var args = new string[]{ "something" };
            var expectedAssembly = Assembly.GetAssembly(typeof(ProcessorTests));
            
            _processor.ProcessArguments(args);

            _mockArgProcessor.Verify(p => p.ProcessArguments(args, expectedAssembly));
        }

        [TestMethod]
        public void ProcessArguments_SetsExitCodeToOne_WhenWasSuccessFalse()
        {
            var args = new string[]{ "something" };
            var assembly = Assembly.GetAssembly(typeof(ProcessorTests));
            var wasSuccess = false;
            var expectedCode = 1;
            _mockArgProcessor.Setup(p => p.ProcessArguments(args, assembly)).Returns(wasSuccess);

            _processor.ProcessArguments(args);

            Assert.AreEqual(expectedCode, Environment.ExitCode);
        }

        [TestMethod]
        public void ProcessArguments_SetsExitCodeToZero_WhenWasSuccessTrue()
        {
            var args = new string[]{ "something" };
            var assembly = Assembly.GetAssembly(typeof(ProcessorTests));
            var wasSuccess = true;
            var expectedCode = 0;
            _mockArgProcessor.Setup(p => p.ProcessArguments(args, assembly)).Returns(wasSuccess);
            
            _processor.ProcessArguments(args);

            Assert.AreEqual(expectedCode, Environment.ExitCode);
        }

        [TestMethod]
        public void ProcessArguments_SetsExitCodeToOne_And_CallsWriteLine_ArgsLengthZero_And_ApplicationLoopNotEnabled()
        {
            var args = new string[]{};
            var assembly = Assembly.GetAssembly(typeof(ProcessorTests));
            var wasSuccess = true;
            var expectedCode = 1;
            var help = "?";
            var applicationLoop = false;
            var expectedMessage = $"Please enter a controller. Use '{help}' to see available controllers.";
            _mockArgProcessor.Setup(p => p.ProcessArguments(args, assembly)).Returns(wasSuccess);
            _settings.Setup(s => s.HelpString()).Returns(help);
            _settings.Setup(s => s.ApplicationLoopEnabled()).Returns(applicationLoop);
            
            
            _processor.ProcessArguments(args);

            Assert.AreEqual(expectedCode, Environment.ExitCode);
        }

        [TestMethod]
        public void ProcessArguments_CallsApplicationLoop_WhenArgsLengthZero_And_ApplicationLoopEnabled()
        {
            var args = new string[]{};
            var applicationLoop = true;
            var expectedAssembly = Assembly.GetAssembly(typeof(ProcessorTests));
            _settings.Setup(s => s.ApplicationLoopEnabled()).Returns(applicationLoop);
            
            _processor.ProcessArguments(args);

            _mockLooper.Verify(l => l.ApplicationLoop(expectedAssembly), Times.Once);
        }
    }
}
