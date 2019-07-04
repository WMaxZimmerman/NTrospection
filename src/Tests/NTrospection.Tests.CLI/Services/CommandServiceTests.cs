using System;
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
    public class CommandServiceTests
    {
        private Mock<IParameterService> _mockParameterService;
        private Mock<IMethodService> _mockMethodService;
        private Mock<IConsoleService> _mockConsoleService;
        private Mock<ISettings> _mockSettings;

        private CommandService _service;
        private CommandMethod _command;
        private CommandMethod _commandStatic;
        private CommandMethod _commandNoParams;
        private CommandMethod _nonCommand;
        private CommandMethod _collectionCommand;

        [TestInitialize]
        public void Init()
        {
            _mockParameterService = new Mock<IParameterService>();
            _mockMethodService = new Mock<IMethodService>();
            _mockConsoleService = new Mock<IConsoleService>();
            _mockSettings = new Mock<ISettings>();
            _service = new CommandService(_mockParameterService.Object,
                                          _mockMethodService.Object,
                                          _mockConsoleService.Object,
                                          _mockSettings.Object);

            _command = new CommandMethod(typeof(FakeController).GetMethod("FakeMethod"));
            _commandStatic = new CommandMethod(typeof(FakeController).GetMethod("FakeStaticMethod"));
            _commandNoParams = new CommandMethod(typeof(FakeController).GetMethod("FakeMethodNoParams"));
            _nonCommand = new CommandMethod(typeof(FakeController).GetMethod("FakeMethodNonCommand"));
            _collectionCommand = new CommandMethod(typeof(FakeController).GetMethod("FakeMethodWithCollections"));
        }

        [TestMethod]
        public void GetCommandName_ReturnsName_WhenAttributeIsProvided()
        {
            var expected = "Fake";
            var actual = _service.GetCommandName(_command.Info);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetCommandName_ReturnsNull_WhenAttributeIsNull()
        {
            string expected = null;
            var actual = _service.GetCommandName(_nonCommand.Info);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetParameters_CallsGetParameters()
        {
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(new MethodParameters());
            var args = new List<CommandLineArgument>
            {
                new CommandLineArgument
                {
                    Command = "foo",
                    Order = 1,
                    Values = new List<string> { "5" }
                },
                new CommandLineArgument
                {
                    Command = "bar",
                    Order = 2,
                    Values = new List<string> { "something" }
                }
            };

            var actual = _service.GetParameters(_command.Info, args);

            _mockMethodService.Verify(s => s.GetParameters(_command.Info, args), Times.Once());
        }

        [TestMethod]
        public void GetParameters_CallsWriteLine_ForEachReturnedError()
        {
            var expectedError1 = "I'm an error";
            var expectedError2 = "You're an error";
            var expectedError3 = "We're all errors";
            var args = new List<CommandLineArgument>();
            var expectedReturn = new MethodParameters
            {
                Errors = new List<string>
                {
                    expectedError1,
                    expectedError2,
                    expectedError3
                }
            };
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(expectedReturn);

            var actual = _service.GetParameters(_command.Info, args);

            _mockConsoleService.Verify(s => s.WriteLine(expectedError1), Times.Once());
            _mockConsoleService.Verify(s => s.WriteLine(expectedError2), Times.Once());
            _mockConsoleService.Verify(s => s.WriteLine(expectedError3), Times.Once());
        }

        [TestMethod]
        public void GetParameters_DoesNotCallsWriteLine_WhenNoErrors()
        {
            var args = new List<CommandLineArgument>();
            var expectedReturn = new MethodParameters
            {
                Errors = new List<string>()
            };
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(expectedReturn);

            var actual = _service.GetParameters(_command.Info, args);

            _mockConsoleService.Verify(s => s.WriteLine(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void GetParameters_ReturnsNull_WhenErrors()
        {
            var expectedError1 = "I'm an error";
            var expectedError2 = "You're an error";
            var expectedError3 = "We're all errors";
            var args = new List<CommandLineArgument>();
            var expectedReturn = new MethodParameters
            {
                Errors = new List<string>
                {
                    expectedError1,
                    expectedError2,
                    expectedError3
                }
            };
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(expectedReturn);

            var actual = _service.GetParameters(_command.Info, args);

            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void GetParameters_ReturnsParameterArray_WhenNoErrors()
        {
            var expectedObject1 = "I'm an object";
            var expectedObject2 = "You're an object";
            var expectedObject3 = "We're all objects";
            var args = new List<CommandLineArgument>();
            var expected = new object[] { expectedObject1, expectedObject2, expectedObject3 };
            var expectedReturn = new MethodParameters
            {
                Parameters = new List<object>
                {
                    expectedObject1,
                    expectedObject2,
                    expectedObject3
                },
                Errors = new List<string>()
            };
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(expectedReturn);

            var actual = _service.GetParameters(_command.Info, args);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OutputDocumentation_CallsWriteLineWithMethodInfo()
        {
            var expectedMessage1 = "";
            var expectedMessage2 = "Fake";
            var expectedMessage3 = "Description: Fake Method For Reflection";
            _mockParameterService.Setup(s => s.GetParameterDocumentation(It.IsAny<ParameterInfo>()))
                .Returns(new List<string>());

            _service.OutputDocumentation(_command.Info);

            _mockConsoleService.Verify(s => s.WriteLine(expectedMessage1), Times.Once());
            _mockConsoleService.Verify(s => s.WriteLine(expectedMessage2), Times.Once());
            _mockConsoleService.Verify(s => s.WriteLine(expectedMessage3), Times.Once());
        }

        [TestMethod]
        public void OutputDocumentation_CallsWriteLine_ForEachParameter()
        {
            var expectedMessage1 = "Parameters:";
            var expectedMessage2 = "I'm a parameter";

            _mockParameterService.Setup(s => s.GetParameterDocumentation(It.IsAny<ParameterInfo>()))
                .Returns(new List<string>
                        {
                            expectedMessage2
                        });

            _service.OutputDocumentation(_command.Info);

            _mockConsoleService.Verify(s => s.WriteLine(expectedMessage1), Times.Once());
            _mockConsoleService.Verify(s => s.WriteLine(expectedMessage2), Times.Exactly(3));
        }

        [TestMethod]
        public void OutputDocumentation_DoesNotCallsWriteLineForParameter_WhenNoParameters()
        {
            var expectedMessage1 = "Parameters:";
            _mockParameterService.Setup(s => s.GetParameterDocumentation(It.IsAny<ParameterInfo>()))
                .Returns(new List<string>());

            _service.OutputDocumentation(_commandNoParams.Info);

            _mockConsoleService.Verify(s => s.WriteLine(expectedMessage1), Times.Never());
        }

        [TestMethod]
        public void OutputDocumentation_DoesNotCallsWriteLine_WhenNotCliCommand()
        {
            _mockParameterService.Setup(s => s.GetParameterDocumentation(It.IsAny<ParameterInfo>()))
                .Returns(new List<string>());

            _service.OutputDocumentation(_nonCommand.Info);

            _mockConsoleService.Verify(s => s.WriteLine(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void Invoke_CallsGetParameters()
        {
            var args = new List<CommandLineArgument>();
            _service.Invoke(_command.Info, args);

            _mockMethodService.Verify(s => s.GetParameters(_command.Info, args), Times.Once);
        }

        [TestMethod]
        public void Invoke_ReturnsFalseForWasSuccess_WhenGetParametersReturnsNull()
        {
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(new MethodParameters{Errors = new List<string>{""}});
            var args = new List<CommandLineArgument>();
            var expected = false;
            var expectedMessages = new List<string>();

            var actual = _service.Invoke(_command.Info, args);

            Assert.AreEqual(expected, actual.WasSuccess);
            CollectionAssert.AreEqual(expectedMessages, actual.Messages);
        }

        [TestMethod]
        public void Invoke_ReturnsTrueForWasSuccess_WhenGetParametersReturnsCorrectParameters()
        {
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(new MethodParameters{ Parameters = new List<object>{ 5, "test", false} });
            var args = new List<CommandLineArgument>();
            var expected = true;
            var expectedMessages = new List<string>();

            var actual = _service.Invoke(_command.Info, args);

            Assert.AreEqual(expected, actual.WasSuccess);
            CollectionAssert.AreEqual(expectedMessages, actual.Messages);
        }

        [TestMethod]
        public void Invoke_CallsInvokeStatic_WhenInfoIsStatic()
        {
            var expectedParams = new List<object>{ 5, "test", false};
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(new MethodParameters{ Parameters = expectedParams});
            var args = new List<CommandLineArgument>();

            var actual = _service.Invoke(_commandStatic.Info, args);

            _mockMethodService.Verify(s => s.InvokeStatic(_commandStatic.Info, It.IsAny<object[]>()), Times.Once);
        }

        [TestMethod]
        public void Invoke_CallsInvokeInstance_WhenInfoIsNotStatic()
        {
            var expectedParams = new List<object>{ 5, "test", false};
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(new MethodParameters{ Parameters = expectedParams});
            var args = new List<CommandLineArgument>();

            var actual = _service.Invoke(_command.Info, args);

            _mockMethodService.Verify(s => s.InvokeInstance(_command.Info, It.IsAny<object[]>()), Times.Once);
        }

        [TestMethod]
        public void Invoke_ReturnsMessagesAndWasSuccessFalse_WhenInvokeThrowsError()
        {
            var expectedHelpString = "?";
            _mockSettings.Setup(s => s.HelpString()).Returns(expectedHelpString);
            var expectedParams = new List<object>{ 5, "test", false};
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(new MethodParameters{ Parameters = expectedParams});
            _mockMethodService.Setup(s => s.InvokeInstance(
                                                           It.IsAny<MethodInfo>(),
                                                           It.IsAny<object[]>())).Throws(new Exception());
            var args = new List<CommandLineArgument>();
            var expected = false;
            var expectedMessages = new List<string>
            {
                "An error occurred while attempting to execute the command.",
                "This is most likely due to invalid arguments.",
                $"Please verify the command usage with '{expectedHelpString}' and try again."
            };

            var actual = _service.Invoke(_command.Info, args);
            
            Assert.AreEqual(expected, actual.WasSuccess);
            CollectionAssert.AreEqual(expectedMessages, actual.Messages);
        }

        [TestMethod]
        public void Invoke_ReturnsMessagesAndWasSuccessFalse_WhenInvokeThrowsTargetInvocationErrorWithoutInnerMessage()
        {
            var expectedException = new TargetInvocationException(null);
            var expectedHelpString = "?";
            _mockSettings.Setup(s => s.HelpString()).Returns(expectedHelpString);
            var expectedParams = new List<object>{ 5, "test", false};
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(new MethodParameters{ Parameters = expectedParams});
            _mockMethodService.Setup(s => s.InvokeInstance(
                                                           It.IsAny<MethodInfo>(),
                                                           It.IsAny<object[]>())).Throws(expectedException);
            var args = new List<CommandLineArgument>();
            var expected = false;
            var expectedMessages = new List<string>
            {
                "An error occurred while executing the command."
            };

            var actual = _service.Invoke(_command.Info, args);
            
            Assert.AreEqual(expected, actual.WasSuccess);
            CollectionAssert.AreEqual(expectedMessages, actual.Messages);
        }

        [TestMethod]
        public void Invoke_ReturnsMessagesAndWasSuccessFalse_WhenInvokeThrowsTargetInvocationErrorWithInnerMessage()
        {
            var expectedMessage = "foo";
            var expectedStackTrace = " bar ";
            var inner = new Mock<Exception>();
            inner.SetupGet(i => i.Message).Returns(expectedMessage);
            inner.SetupGet(i => i.StackTrace).Returns(expectedStackTrace);
            
            var expectedException = new TargetInvocationException(inner.Object);
            var expectedHelpString = "?";
            _mockSettings.Setup(s => s.HelpString()).Returns(expectedHelpString);
            var expectedParams = new List<object>{ 5, "test", false};
            _mockMethodService.Setup(s => s.GetParameters(It.IsAny<MethodInfo>(), It.IsAny<List<CommandLineArgument>>()))
                .Returns(new MethodParameters{ Parameters = expectedParams});
            _mockMethodService.Setup(s => s.InvokeInstance(
                                                           It.IsAny<MethodInfo>(),
                                                           It.IsAny<object[]>())).Throws(expectedException);
            var args = new List<CommandLineArgument>();
            var expected = false;
            var expectedMessages = new List<string>
            {
                "An error occurred while executing the command.",
                $"Message: {expectedMessage}",
                $"Stack Trace: {expectedStackTrace.Trim()}"
            };

            var actual = _service.Invoke(_command.Info, args);
            
            Assert.AreEqual(expected, actual.WasSuccess);
            CollectionAssert.AreEqual(expectedMessages, actual.Messages);
        }
    }
}
