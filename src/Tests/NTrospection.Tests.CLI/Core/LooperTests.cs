using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Core;
using NTrospection.CLI.Services;

namespace NTrospection.Tests.CLI.Core
{
    [TestClass]
    public class LooperTests
    {
        private Looper _looper;

        private Mock<ISettings> _settings;
        private Mock<IConsoleService> _console;
        private Mock<IArgumentProcessorService> _mockArgProcessor;

        const string _indicator = ">";
        const string _help = "?";
        const string _exit = "exit";

        [TestInitialize]
        public void Init()
        {
            _settings = new Mock<ISettings>();
            _console = new Mock<IConsoleService>();
            _mockArgProcessor = new Mock<IArgumentProcessorService>();
            
            _looper = new Looper(_settings.Object,
                                 _console.Object,
                                 _mockArgProcessor.Object);

            _settings.Setup(s => s.InputIndicator()).Returns(_indicator);
            _settings.Setup(s => s.HelpString()).Returns(_help);
            _settings.Setup(s => s.ExitString()).Returns(_exit);
        }

        [TestMethod]
        public void ApplicationLoop_CallsWriteWithIndicator()
        {
            var assembly = Assembly.GetAssembly(typeof(LooperTests));
            var expectedMessage = $"{_indicator} ";
            _console.Setup(c => c.ReadLine()).Returns(_exit);
            
            _looper.ApplicationLoop(assembly);

            _console.Verify(c => c.Write(expectedMessage), Times.Once());
        }
    }
}
