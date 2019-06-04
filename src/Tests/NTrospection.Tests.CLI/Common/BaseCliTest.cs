using NTrospection.CLI.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NTrospection.Tests.CLI.Common
{
    public class BaseCliTest
    {
        protected StringWriter consoleMock;
        protected StringBuilder mockConsole = new StringBuilder();
        protected string helpString = Settings.HelpString;
        protected string argPre = Settings.ArgumentPrefix;

        public BaseCliTest()
        {
            consoleMock = new StringWriter(mockConsole);
            Console.SetOut(consoleMock);
            SetApplicationLoopEnabled(false);
            SetParamDetail("simple");
        }

        protected string ConvertConsoleLinesToString(List<string> lines, bool startingNewLine = false, bool endingNewLine = true)
        {
            var consoleString = string.Join(Environment.NewLine, lines);
            if (endingNewLine) consoleString += Environment.NewLine;
            if (startingNewLine) consoleString = Environment.NewLine + consoleString;
            return consoleString;
        }

        protected void SetApplicationLoopEnabled(bool value)
        {
            UpdateConfigValue("applicationLoopEnabled", value.ToString());
        }

        protected void SetParamDetail(string detail)
        {
            UpdateConfigValue("paramDetail", detail);
        }


	protected static string GetStackTraceForException(string relativeNamespace, string className, string methodName, string paramString, int line)
	{
	    var projPath = $@"NTrospection.Tests.CLI\{relativeNamespace}\{className}";
	    var namespacePath = projPath.Replace('\\', '.');
	    var myPath = Directory.GetParent((new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);
	    var stackPath = myPath.Parent.Parent.Parent.Parent.FullName + $@"\{projPath}.cs";
	    
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

        private void UpdateConfigValue(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;

            if (settings[key] != null)
            {
                settings[key].Value = value;
            }
            else
            {
                settings.Add(key, value);
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appsettings");
        }
    }
}
