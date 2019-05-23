using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NTrospection.Tests.CLI.Common
{
    [TestClass]
    public class ExecutionTests
    {
        private string _directory;
        private const string _executableName = "NTrospection.Fake.CLI.exe";

        [TestInitialize]
        public void Setup()
        {
            _directory = Directory.GetParent((new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath).Parent.Parent.Parent.Parent.FullName + @"\Fakes\NTrospection.Fake.CLI\bin\Debug\netcoreapp2.2\win10-x64\";
        }

        [TestMethod]
        public void ApplicationTerminatesWithFailedStatusWhenInvalidController()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" meth add --firstNum 1 --secondNum 2";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(1, proc.ExitCode);
        }

        [TestMethod]
        public void ApplicationTerminatesWithFailedStatusWhenMissingCommand()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" math";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(1, proc.ExitCode);
        }

        [TestMethod]
        public void ApplicationTerminatesWithFailedStatusWhenInvalidParameters()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" math add --firstNom 1 --secondNum 2";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(1, proc.ExitCode);
        }

        [TestMethod]
        public void ApplicationTerminatesWithFailedStatusWhenMissingParameters()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" math add --firstNum 1";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(1, proc.ExitCode);
        }
    }
}
