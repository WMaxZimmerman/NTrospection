using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Models;
using NTrospection.CLI.Services;
using NTrospection.Tests.CLI.App.Controllers;

namespace NTrospection.Tests.CLI.Services
{
    [TestClass]
    public class MethodServiceTests
    {
        private Mock<IParameterService> _mockParameterService;
        private MethodService _service;
        private CommandMethod _command;
        private CommandMethod _nonCommand;
        private CommandMethod _collectionCommand;

        [TestInitialize]
        public void Init()
        {
            _mockParameterService = new Mock<IParameterService>();
            _service = new MethodService(_mockParameterService.Object);

            _command = new CommandMethod(typeof(MethodServiceTests).GetMethod("FakeMethod"));
            _nonCommand = new CommandMethod(typeof(MethodServiceTests).GetMethod("FakeMethodNonCommand"));
            _collectionCommand = new CommandMethod(typeof(MethodServiceTests).GetMethod("FakeMethodWithCollections"));
        }

        // === Fake Method For Reflection ===
        [CliCommand("Fake", "Fake Method For Reflection")]
        public void FakeMethod(
                       [CliParameter('f', "")]int foo,
                       string bar,
                       [CliParameter("")]bool foobar = false)
        {

        }

        // === Fake Method For Reflection ===
        public void FakeMethodNonCommand()
        {

        }

        // === Fake Method For Reflection ===
        [CliCommand("Fake", "Fake Method For Reflection")]
        public void FakeMethodWithCollections(List<int> foo, int[] bar)
        {

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
        public void GetParameters_CallsGetParameterValueForEachArgument_WhenPassedValidArgs()
        {
            _mockParameterService.Setup(p => p.GetParameterErrors(It.IsAny<MethodInfo>(),
                                      It.IsAny<List<CommandLineArgument>>()))
            .Returns(new List<string>());
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(pi => pi.Name == "foo"),
                              It.Is<CommandLineArgument>(a => a.Command == "foo")))
            .Returns(true);
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(pi => pi.Name == "bar"),
                              It.Is<CommandLineArgument>(a => a.Command == "bar")))
            .Returns(true);
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(pi => pi.Name == "foobar"),
                              It.Is<CommandLineArgument>(a => a.Command == "foobar")))
            .Returns(true);

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
                    Order = 1,
                    Values = new List<string> { "something" }
                },
                new CommandLineArgument
                {
                    Command = "foobar",
                    Order = 1,
                    Values = new List<string> { "true" }
                }
            };

            var actual = _service.GetParameters(_command.Info, args);

            _mockParameterService
            .Verify(s => s.GetParameterValue(It.IsAny<ParameterInfo>(), It.IsAny<CommandLineArgument>()), Times.Exactly(3));
        }

        [TestMethod]
        public void GetParameters_CallsGetParameterErrors_WhenPassedValidArgs()
        {
            _mockParameterService.Setup(p => p.GetParameterErrors(It.IsAny<MethodInfo>(),
                                      It.IsAny<List<CommandLineArgument>>()))
            .Returns(new List<string>());

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
                    Order = 1,
                    Values = new List<string> { "something" }
                },
                new CommandLineArgument
                {
                    Command = "foobar",
                    Order = 1,
                    Values = new List<string> { "true" }
                }
            };

            var actual = _service.GetParameters(_command.Info, args);

            _mockParameterService.Verify(s => s.GetParameterErrors(_command.Info, args), Times.Once);
        }

        [TestMethod]
        public void GetParameters_AppendsErrors_WhenGetParameterErrorsReutrnsValues()
        {
            var expectedList = new List<string> { "foo", "bar", "foobar" };
            _mockParameterService.Setup(p => p.GetParameterErrors(It.IsAny<MethodInfo>(),
                                      It.IsAny<List<CommandLineArgument>>()))
            .Returns(expectedList);
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.IsAny<ParameterInfo>(),
                              It.IsAny<CommandLineArgument>()))
            .Returns(true);

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
                    Order = 1,
                    Values = new List<string> { "something" }
                },
                new CommandLineArgument
                {
                    Command = "foobar",
                    Order = 1,
                    Values = new List<string> { "true" }
                }
            };

            var actual = _service.GetParameters(_command.Info, args);

            _mockParameterService.Verify(s => s.GetParameterErrors(_command.Info, args), Times.Once);
            CollectionAssert.AreEqual(expectedList, actual.Errors);
        }

        [TestMethod]
        public void GetParameters_AppendsError_WhenGetParameterIsMissing()
        {
            var expectedErrors = new List<string>
        {
        "The parameter 'foo' must be specified"
        };
            _mockParameterService.Setup(p => p.GetParameterErrors(It.IsAny<MethodInfo>(),
                                      It.IsAny<List<CommandLineArgument>>()))
            .Returns(new List<string>());
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(p => p.Name != "foo"),
                              It.IsAny<CommandLineArgument>()))
            .Returns(true);

            var args = new List<CommandLineArgument>
            {
                new CommandLineArgument
                {
                    Command = "bar",
                    Order = 1,
                    Values = new List<string> { "something" }
                },
                new CommandLineArgument
                {
                    Command = "foobar",
                    Order = 1,
                    Values = new List<string> { "true" }
                }
            };

            var actual = _service.GetParameters(_command.Info, args);

            CollectionAssert.AreEqual(expectedErrors, actual.Errors);
        }

        [TestMethod]
        public void GetParameters_AppendsParameter_WhenParameterIsMissingButHasDefault()
        {
            var expectedErrors = new List<string>();
            _mockParameterService.Setup(p => p.GetParameterErrors(It.IsAny<MethodInfo>(),
                                      It.IsAny<List<CommandLineArgument>>()))
            .Returns(new List<string>());
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.IsAny<ParameterInfo>(),
                              It.IsAny<CommandLineArgument>()))
            .Returns(false);
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(p => p.Name == "foo"),
                              It.Is<CommandLineArgument>(c => c.Command == "foo")))
            .Returns(true);
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(p => p.Name == "bar"),
                              It.Is<CommandLineArgument>(c => c.Command == "bar")))
            .Returns(true);

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
                    Order = 1,
                    Values = new List<string> { "something" }
                }
            };

            var actual = _service.GetParameters(_command.Info, args);

            Assert.AreEqual(3, actual.Parameters.Count);
        }

        [TestMethod]
        public void GetParameters_ReturnsNoErrors_WhenParameterLastParameterIsMissingAndHasDefault()
        {
            var expectedErrors = new List<string>();
            _mockParameterService.Setup(p => p.GetParameterErrors(It.IsAny<MethodInfo>(),
                                      It.IsAny<List<CommandLineArgument>>()))
            .Returns(new List<string>());
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(p => p.Name == "foobar"),
                              It.IsAny<CommandLineArgument>()))
            .Returns(false);
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(p => p.Name == "foo"),
                              It.Is<CommandLineArgument>(c => c.Command == "foo")))
            .Returns(true);
            _mockParameterService
            .Setup(s => s.ParameterMatchesArg(It.Is<ParameterInfo>(p => p.Name == "bar"),
                              It.Is<CommandLineArgument>(c => c.Command == "bar")))
            .Returns(true);

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

            Assert.AreEqual(0, actual.Errors.Count);
        }

        [TestMethod]
        public void InvokeStatic_CallsInfoInvokeWithCorrectFlags()
        {
            var mockInfo = new Mock<MethodInfo>();
            var expectedParams = new object[] { "foo" };

            _service.InvokeStatic(mockInfo.Object, expectedParams);

            mockInfo.Verify(i => i.Invoke(null, BindingFlags.Static, null, expectedParams, null));
        }

        [TestMethod]
        public void InvokeInstance_CallsInfoInvokeWithCorrectFlags()
        {
            var mockInfo = new Mock<MethodInfo>();
            mockInfo.SetupGet(i => i.DeclaringType).Returns(typeof(FakeController));
            var expectedParams = new object[] { "foo" };

            _service.InvokeInstance(mockInfo.Object, expectedParams);

            mockInfo.Verify(i => i.Invoke(It.Is<FakeController>(t => t != null), BindingFlags.Instance, null, expectedParams, null));
        }
    }
}
