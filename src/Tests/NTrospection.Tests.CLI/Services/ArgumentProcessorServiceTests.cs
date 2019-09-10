using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Core;
using NTrospection.CLI.Models;
using NTrospection.CLI.Services;

namespace NTrospection.Tests.CLI.Services
{
    [TestClass]
    public class ArgumentProcessorServiceTests
    {
        private ArgumentProcessorService _service;

        private Mock<ISettings> _mockSettings;
        private Mock<IArgumentService> _mockArgService;
        private Mock<IControllerService> _mockControllerService;
        private Mock<ProcessedArguments> _mockArguments;
        private Mock<IConsoleService> _mockConsole;

        private string _help = "?";
        private string _pre = "-";
        
        [TestInitialize]
        public void Init()
        {
            _mockSettings = new Mock<ISettings>();
            _mockArgService = new Mock<IArgumentService>();
            _mockControllerService = new Mock<IControllerService>();
            _mockArguments = new Mock<ProcessedArguments>();
            _mockConsole = new Mock<IConsoleService>();
            
            _service = new ArgumentProcessorService(_mockSettings.Object,
                                                    _mockControllerService.Object,
                                                    _mockArgService.Object,
                                                    _mockConsole.Object);

            _mockSettings.Setup(s => s.HelpString()).Returns(_help);
            _mockSettings.Setup(s => s.ArgumentPrefix()).Returns(_pre);

            
            _mockArgService.Setup(s => s.ProcessArgs(It.IsAny<IReadOnlyList<string>>())).Returns(_mockArguments.Object);
        }

        [TestMethod]
        public void ProcessArguments_CallsGetControllers()
        {
            var args = new List<string>();
            var assembly = new Mock<Assembly>();
            
            _service.ProcessArguments(args, assembly.Object);

            _mockControllerService.Verify(m => m.GetControllers(assembly.Object), Times.Once());
        }

        [TestMethod]
        public void ProcessArguments_CallsProcessArgs()
        {
            var args = new List<string>();
            var assembly = new Mock<Assembly>();
            
            _service.ProcessArguments(args, assembly.Object);

            _mockArgService.Verify(m => m.ProcessArgs(args), Times.Once());
        }

        [TestMethod]
        public void ProcessArguments_ReturnsTrue_WhenIsHelpCall()
        {
            var args = new List<string>();
            var assembly = new Mock<Assembly>();
            var expected = true;
            _mockArgService.Setup(s => s.ProcessArgs(It.IsAny<IReadOnlyList<string>>()))
                .Returns(new ProcessedArguments{IsHelpCall = true});
            
            var actual = _service.ProcessArguments(args, assembly.Object);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ProcessArguments_CallsDocumentCommand_And_ReturnsTrue_WhenIsHelpCall_AndControllerGiven()
        {
            var args = new List<string>();
            var assembly = new Mock<Assembly>();
            var expected = true;
            var controllerName = "something";
            var expectedCommand = "command";
            var expectedController = new Controller{ Name = controllerName };
            var controllers = new List<Controller>{ expectedController };
            _mockArgService.Setup(s => s.ProcessArgs(It.IsAny<IReadOnlyList<string>>()))
                .Returns(new ProcessedArguments
                        {
                            IsHelpCall = true,
                            Controller = controllerName,
                            Command = expectedCommand
                        });
            _mockControllerService.Setup(s => s.GetControllers(assembly.Object)).Returns(controllers);
            
            var actual = _service.ProcessArguments(args, assembly.Object);

            Assert.AreEqual(expected, actual);
            _mockControllerService.Verify(s => s.DocumentCommand(expectedController, expectedCommand), Times.Once);
        }

        [TestMethod]
        public void ProcessArguments_CallsOutputDocumentationForEachController_And_ReturnsTrue_WhenIsHelpCall_AndControllerNotGiven()
        {
            var args = new List<string>();
            var assembly = new Mock<Assembly>();
            var expected = true;
            var controllers = new List<Controller>
            {
                new Controller{ Name = "something" },
                new Controller{ Name = "something else" },
                new Controller{ Name = "something different" }
            };
            _mockArgService.Setup(s => s.ProcessArgs(It.IsAny<IReadOnlyList<string>>()))
                .Returns(new ProcessedArguments{ IsHelpCall = true, Controller = null });
            _mockControllerService.Setup(s => s.GetControllers(assembly.Object)).Returns(controllers);
            
            var actual = _service.ProcessArguments(args, assembly.Object);

            Assert.AreEqual(expected, actual);
            _mockControllerService.Verify(s => s.OutputDocumentation(It.IsAny<Controller>()), Times.Exactly(controllers.Count));
            foreach (var c in controllers)
            {
                _mockControllerService.Verify(s => s.OutputDocumentation(c), Times.Once);
            }
            _mockControllerService.Verify(s => s.DocumentCommand(It.IsAny<Controller>(), It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void ProcessArguments_CallsWriteLine_And_ReturnsFalse_WhenIsNotHelpCall_And_ControllerNotFound()
        {
            var args = new List<string>();
            var assembly = new Mock<Assembly>();
            var expected = false;
            var expectedController = "controller";
            var controllers = new List<Controller>();
            _mockArgService.Setup(s => s.ProcessArgs(It.IsAny<IReadOnlyList<string>>()))
                .Returns(new ProcessedArguments{ IsHelpCall = false, Controller = expectedController });
            _mockControllerService.Setup(s => s.GetControllers(assembly.Object)).Returns(controllers);
            var expectedMessage = $"'{expectedController}' is not a valid controller. Use '{_help}' to see available controllers.";
            
            var actual = _service.ProcessArguments(args, assembly.Object);

            Assert.AreEqual(expected, actual);
            _mockConsole.Verify(c => c.WriteLine(expectedMessage), Times.Once);
        }

        [TestMethod]
        public void ProcessArguments_CallsExecuteCommand_AndWriteLineForEachMessage_And_ReturnsInvokeStatus_WhenIsNotHelpCall_And_ControllerFound()
        {
            var args = new List<string>();
            var assembly = new Mock<Assembly>();
            var expected = true;
            var controllerName = "controller";
            var expectedCommand = "command";
            var expectedController = new Controller{ Name = controllerName };
            var expectedArgs = new List<CommandLineArgument>();
            var expectedMessages = new List<string>
            {
                "message 1",
                "message 2",
                "message 3",
            };
            var controllers = new List<Controller>{ expectedController };
            var invoke = new CommandResponse
            {
                Messages = expectedMessages,
                WasSuccess = expected
            };
            
            _mockArgService.Setup(s => s.ProcessArgs(It.IsAny<IReadOnlyList<string>>()))
                .Returns(new ProcessedArguments
                        {
                            IsHelpCall = false,
                            Controller = controllerName,
                            Command = expectedCommand,
                            Arguments = expectedArgs
                        });
            _mockControllerService.Setup(s => s.GetControllers(assembly.Object)).Returns(controllers);
            _mockControllerService.Setup(s => s.ExecuteCommand(expectedController, expectedCommand, expectedArgs)).Returns(invoke);
            
            var actual = _service.ProcessArguments(args, assembly.Object);

            _mockControllerService.Verify(c => c.ExecuteCommand(expectedController, expectedCommand, expectedArgs), Times.Once);
            _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Exactly(expectedMessages.Count));
            // _mockConsole.Verify(c => c.WriteLine(expectedMessage), Times.Once);
            Assert.AreEqual(expected, actual);
        }
    }
}
