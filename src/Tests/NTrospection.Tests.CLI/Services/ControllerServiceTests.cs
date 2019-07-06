using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Core;
using NTrospection.CLI.Models;
using NTrospection.CLI.Services;
using NTrospection.Tests.CLI.App.Controllers;

namespace NTrospection.Tests.CLI.Services
{
    [TestClass]
    public class ControllerServiceTests
    {
        private ControllerService _service;

        private Mock<ISettings> _mockSettings;
        private Mock<IConsoleService> _mockConsole;
        private Mock<ICommandService> _mockCommandService;
        
        [TestInitialize]
        public void Init()
        {
            _mockSettings = new Mock<ISettings>();
            _mockConsole = new Mock<IConsoleService>();
            _mockCommandService = new Mock<ICommandService>();
            
            _service = new ControllerService(_mockSettings.Object,
                                             _mockConsole.Object,
                                             _mockCommandService.Object);
        }

        [TestMethod]
        public void OutputDocumentation_DoesNotCallConsole_WhenNoAttributes()
        {
            var controller = new Controller(typeof(NonController));

            _service.OutputDocumentation(controller);

            _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void OutputDocumentation_DoesCallConsole_WhenAttributes()
        {
            var expectedMessage = "fake - This is a test description.";
            var controller = new Controller(typeof(FakeController));

            _service.OutputDocumentation(controller);

            _mockConsole.Verify(c => c.WriteLine(expectedMessage), Times.Once());
        }

        [TestMethod]
        public void DocumentCommand_DoesNotCallCommandService_WhenNoCommands()
        {
            var controller = new Controller(typeof(NoCommandController));

            _service.DocumentCommand(controller, "something");

            _mockCommandService.Verify(s => s.OutputDocumentation(It.IsAny<MethodInfo>()), Times.Never());
        }

        [TestMethod]
        public void DocumentCommand_DoesCallCommandService_WhenProvidedValidCommandName()
        {
            var controller = new Controller(typeof(DocumentationController));

            _service.DocumentCommand(controller, "example");

            _mockCommandService.Verify(s => s.OutputDocumentation(It.IsAny<MethodInfo>()), Times.Once());
        }

        [TestMethod]
        public void DocumentCommand_DoesCallCommandServiceForAllCommand_WhenProvidedInvalidCommand()
        {
            var expectedCount = 5;
            var controller = new Controller(typeof(FakeController));

            _service.DocumentCommand(controller, "invalid");

            _mockCommandService.Verify(s => s.OutputDocumentation(It.IsAny<MethodInfo>()), Times.Exactly(expectedCount));
        }

        [TestMethod]
        public void GetControllerName_ReturnsNull_WhenNotPassedValidController()
        {
            string expected = null;
            var controller = new Controller(typeof(NonController));

            var actual = _service.GetControllerName(controller);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetControllerName_ReturnsNameOfController_WhenPassedValidController()
        {
            string expected = "fake";
            var controller = new Controller(typeof(FakeController));

            var actual = _service.GetControllerName(controller);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteCommand_ReturnsCommandResponseFromCommandService_WhenPassedValidCommand()
        {
            var expectedArgs = new List<CommandLineArgument>();
            var controller = new Controller(typeof(FakeController));
            var expected = new CommandResponse
            {
                WasSuccess = true,
                Messages = new List<string>()
            };
            _mockCommandService.Setup(s => s.Invoke(It.IsAny<MethodInfo>(), expectedArgs)).Returns(expected);

            var actual = _service.ExecuteCommand(controller, "Fake", expectedArgs);

            _mockCommandService.Verify(s => s.Invoke(It.IsAny<MethodInfo>(), expectedArgs), Times.Once());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteCommand_ReturnsFailedCommandResponse_WhenPassedInvalidCommand()
        {
            var expectedCommand = "invalid";
            var expectedHelpString = "?";
            _mockSettings.Setup(s => s.HelpString()).Returns(expectedHelpString);
            
            var expectedArgs = new List<CommandLineArgument>();
            var controller = new Controller(typeof(DocumentationController));
            var expected = new CommandResponse
            {
                WasSuccess = false,
                Messages = new List<string>
                {
                    $"'{expectedCommand}' is not a valid command.  Use '{expectedHelpString}' to see available commands."
                }
            };
            _mockCommandService.Setup(s => s.Invoke(It.IsAny<MethodInfo>(), expectedArgs)).Returns(expected);

            var actual = _service.ExecuteCommand(controller, expectedCommand, expectedArgs);

            _mockCommandService.Verify(s => s.Invoke(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()), Times.Never());
            Assert.AreEqual(expected.WasSuccess, actual.WasSuccess);
            CollectionAssert.AreEqual(expected.Messages, actual.Messages);
        }

        [TestMethod]
        public void ExecuteCommand_ReturnsDefaultCommandResponseFromCommandService_WhenPassedInvalidCommandAndDefaultExists()
        {
            var expectedCommand = "invalid";
            var expectedCommandDefault = "TestMethod";
            var expectedHelpString = "?";
            _mockSettings.Setup(s => s.HelpString()).Returns(expectedHelpString);
            
            var expectedArgs = new List<CommandLineArgument>();
            var controller = new Controller(typeof(DefaultCommandController));
            var expected = new CommandResponse
            {
                WasSuccess = true,
                Messages = new List<string>()
            };
            _mockCommandService.Setup(s => s.Invoke(It.Is<MethodInfo>( i => i.Name == expectedCommandDefault),
                                                    expectedArgs)).Returns(expected);

            var actual = _service.ExecuteCommand(controller, expectedCommand, expectedArgs);

            _mockCommandService.Verify(s => s.Invoke(It.Is<MethodInfo>( i => i.Name == expectedCommandDefault),
                                                     expectedArgs), Times.Once());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetCommandMethods_ReturnsInstanceMethods_WhenPassedValidControllerWithInstanceCommands()
        {
            var controller = new Controller(typeof(FakeInstanceController));
            var expectedCount = 3;

            var actual = _service.GetCommandMethods(controller);

            Assert.AreEqual(expectedCount, actual.Count);
        }

        [TestMethod]
        public void GetCommandMethods_ReturnsStaticMethods_WhenPassedValidControllerWithStaticCommands()
        {
            var controller = new Controller(typeof(FakeStaticController));
            var expectedCount = 3;

            var actual = _service.GetCommandMethods(controller);

            Assert.AreEqual(expectedCount, actual.Count);
        }

        [TestMethod]
        public void GetCommandMethods_ReturnsStaticAndInstanceMethods_WhenPassedValidControllerWithBothCommandTypes()
        {
            var controller = new Controller(typeof(FakeController));
            var expectedCount = 5;

            var actual = _service.GetCommandMethods(controller);

            Assert.AreEqual(expectedCount, actual.Count);
        }

        [TestMethod]
        public void GetCommandMethods_ReturnsDefaultMethod_WhenPassedValidControllerWithDefaultCommand()
        {
            var controller = new Controller(typeof(DefaultCommandController));
            var expectedCount = 1;

            var actual = _service.GetCommandMethods(controller);

            Assert.AreEqual(expectedCount, actual.Count);
        }

        [TestMethod]
        public void GetDefaultCommandMethod_ReturnsDefaultMethod_WhenPassedValidControllerWithDefaultCommand()
        {
            var controller = new Controller(typeof(DefaultCommandController));

            var actual = _service.GetDefaultCommandMethod(controller);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetDefaultCommandMethod_Returns_WhenPassedControllerWithoutDefault()
        {
            var controller = new Controller(typeof(FakeController));

            var actual = _service.GetDefaultCommandMethod(controller);

            Assert.IsNull(actual);
        }
    }
}
