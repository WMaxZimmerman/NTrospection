using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NTrospection.Tests.CLI.Common
{
    [TestClass]
    public class ExecutionTests
    {
        private static string _directory;
        private const string _executableName = "NTrospection.Fake.CLI.exe";

        [ClassInitialize]
        public static void Init(TestContext test)
        {
            var projPath = @"\Fakes\NTrospection.Fake.CLI\";
            var exePath = @"bin\Debug\net5.0\win10-x64\";
	    
            var myPath = Directory.GetParent((new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);
            var parentDir = myPath.Parent.Parent.Parent.Parent.FullName;
	    
            _directory = parentDir + projPath + exePath;

            var pi = new ProcessStartInfo();
            pi.FileName = "CMD.exe";
            pi.WorkingDirectory = parentDir + projPath;
            pi.Arguments = $"/C dotnet publish -c Debug -r win10-x64";
            var proc = Process.Start(pi);
            proc.WaitForExit();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void ApplicationTerminatesWithFailedStatusWhenInvalidController()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" meth add --firstNum 1 --secondNum 2";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(1, proc.ExitCode);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void ApplicationTerminatesWithFailedStatusWhenMissingCommand()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" math";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(1, proc.ExitCode);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void ApplicationTerminatesWithFailedStatusWhenInvalidParameters()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" math add --firstNom 1 --secondNum 2";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(1, proc.ExitCode);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void ApplicationTerminatesWithFailedStatusWhenMissingParameters()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" math add --firstNum 1";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(1, proc.ExitCode);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void ApplicationTerminatesWithSuccessStatusCalledCorrectly()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" math add --firstNum 1 --secondNum 2";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(0, proc.ExitCode);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void ApplicationTerminatesWithSuccessStatusWhenDocumentation()
        {
            string strCmdText;
            strCmdText = $"/C \"{_directory}{_executableName}\" math ?";
            var proc = Process.Start("CMD.exe", strCmdText);
            proc.WaitForExit();

            Assert.AreEqual(0, proc.ExitCode);
        }
    }
}
