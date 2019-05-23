using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI.Common;
using NTrospection.CLI.Common.Models;
using NTrospection.Tests.CLI.Common.Controllers;

namespace NTrospection.Tests.CLI.Common
{
    [TestClass]
    public class ProcessorTests
    {
        [TestMethod]
        public void GetAllControllersCorectlyReturnsAllControllers()
        {
            var actualControllers = Processor.GetAllControllers(Assembly.GetExecutingAssembly()).ToList();
            var expectedControllers = new List<ControllerVm>
            {
                new ControllerVm(typeof(DefaultCommandController)),
                new ControllerVm(typeof(DocumentationController)),
                new ControllerVm(typeof(ExecutionController))
            };

            Assert.AreEqual(expectedControllers.Count, actualControllers.Count);

            for (var i = 0; i < expectedControllers.Count; i++)
            {
                var expectedController = expectedControllers[i];
                var actualController = actualControllers[i];

                Assert.AreEqual(expectedController.Name, actualController.Name);

                var expectedMethods = expectedController.Methods;
                var actualMethods = actualController.Methods;

                Assert.AreEqual(expectedMethods.Count, actualMethods.Count);

                for (var j = 0; j < expectedMethods.Count; j++)
                {
                    var expectedMethod = expectedMethods[j];
                    var actualMethod = actualMethods[j];

                    Assert.AreEqual(expectedMethod.Name, actualMethod.Name);
                }
            }
        }
    }
}
