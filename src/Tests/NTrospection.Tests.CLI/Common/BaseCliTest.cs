using NTrospection.CLI.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NTrospection.Tests.CLI.Common
{
    public class BaseCliTest
    {
        protected StringWriter consoleMock;
        protected StringBuilder mockConsole = new StringBuilder();
        protected string helpString = "?";
        protected string argPre = "--";
	protected Processor _processor;

	protected Mock<ISettings> _settings;

        public BaseCliTest()
        {
            consoleMock = new StringWriter(mockConsole);
            Console.SetOut(consoleMock);

	    _settings = new Mock<ISettings>();
	    _processor = new Processor();
        }

        protected string ConvertConsoleLinesToString(List<string> lines, bool startingNewLine = false, bool endingNewLine = true)
        {
            var consoleString = string.Join(Environment.NewLine, lines);
            if (endingNewLine) consoleString += Environment.NewLine;
            if (startingNewLine) consoleString = Environment.NewLine + consoleString;
            return consoleString;
        }

	protected static string GetStackTraceForException(string relativeNamespace, string className, string methodName, string paramString, int line)
	{
	    var dirSep = Path.DirectorySeparatorChar;
	    var projPath = $@"NTrospection.Tests.CLI{dirSep}{relativeNamespace}{dirSep}{className}";
	    var namespacePath = projPath.Replace(dirSep, '.');
	    var myPath = Directory.GetParent((new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);
	    var stackPath = myPath.Parent.Parent.Parent.Parent.FullName + $@"{dirSep}{projPath}.cs";
	    
	    var stackTrace = @"Stack Trace: at " +
		$@"{namespacePath}.{methodName}({paramString}) " +
		$@"in {stackPath}:line {line}";

	    return stackTrace;
	}

	protected void AssertCollectionsAreEqual(List<string> expected, List<string> actual)
	{
	    Assert.AreEqual(expected.Count, actual.Count);

	    for (var i = 0; i < expected.Count; i++)
	    {
		Assert.AreEqual(expected[i], actual[i]);
	    }
	}
    }
}
