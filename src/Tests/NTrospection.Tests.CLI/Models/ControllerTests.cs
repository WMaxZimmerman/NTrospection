using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Models;
using NTrospection.CLI.Services;

namespace NTrospection.Tests.CLI.Models
{
    [TestClass]
    public class ControllerTests
    {
        private Controller _controller;

        private Mock<IControllerService> _service;
        
        [TestInitialize]
        public void Init()
        {
            _service = new Mock<IControllerService>();
        }

        [TestMethod]
        public void Constructor_CallsServiceAndSetsValues()
        {
            var expectedType = typeof(ControllerTests);
            var expectedName = "Name";
            var expectedDefault = GetCommandMethod();
            var expectedMethods = new List<CommandMethod>
            {
                GetCommandMethod(),
                GetCommandMethod(),
                GetCommandMethod(),
                expectedDefault
            };

            _service.Setup(s => s.GetControllerName(It.IsAny<Controller>())).Returns(expectedName);
            _service.Setup(s => s.GetDefaultCommandMethod(It.IsAny<Controller>())).Returns(expectedDefault);
            _service.Setup(s => s.GetCommandMethods(It.IsAny<Controller>())).Returns(expectedMethods);
            
            _controller = new Controller(expectedType, _service.Object);

            _service.Verify(s => s.GetControllerName(_controller), Times.Once());
            _service.Verify(s => s.GetCommandMethods(_controller), Times.Once());
            _service.Verify(s => s.GetDefaultCommandMethod(_controller), Times.Once());

            Assert.AreEqual(expectedType, _controller.ClassType);
            Assert.AreEqual(expectedName, _controller.Name);
            Assert.AreEqual(expectedDefault, _controller.DefaultMethod);
            CollectionAssert.AreEqual(expectedMethods, _controller.Methods);
        }

        private CommandMethod GetCommandMethod()
        {
            var _mockMethodService = new Mock<IMethodService>();
            _mockMethodService.Setup(s => s.GetCommandName(It.IsAny<MethodInfo>())).Returns("Fake");
            
            return new CommandMethod(null, _mockMethodService.Object);
        }
    }
}
