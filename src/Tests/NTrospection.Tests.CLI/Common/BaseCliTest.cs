using NTrospection.CLI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

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
