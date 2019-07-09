using System;
using System.Configuration;

namespace NTrospection.CLI.Core
{
    public interface ISettings
    {
        string HelpString();
        string ArgumentPrefix();
        string ParamDetail();
        string InputIndicator();
        string ExitString();
        bool ApplicationLoopEnabled();
    }

    public class Settings : ISettings
    {
        public string HelpString => GetAppSetting("helpString") ?? "?";
        public string ArgumentPrefix => GetAppSetting("argumentPrefix") ?? "--";
        public string ParamDetail => GetAppSetting("paramDetail") ?? "simple";
        public string InputIndicator => GetAppSetting("inputIndicator") ?? ">";
        public string ExitString => GetAppSetting("exitString") ?? "exit";
        public bool ApplicationLoopEnabled => GetAppSetting("applicationLoopEnabled") != null && Convert.ToBoolean(GetAppSetting("applicationLoopEnabled"));

        bool ISettings.ApplicationLoopEnabled()
        {
            return GetAppSetting("applicationLoopEnabled") != null && Convert.ToBoolean(GetAppSetting("applicationLoopEnabled"));
        }

        string ISettings.ArgumentPrefix()
        {
            return GetAppSetting("argumentPrefix") ?? "--";
        }

        string ISettings.ExitString()
        {
            return GetAppSetting("exitString") ?? "exit";
        }

        string ISettings.HelpString()
        {
            return GetAppSetting("helpString") ?? "?";
        }

        string ISettings.InputIndicator()
        {
            return GetAppSetting("inputIndicator") ?? ">";
        }

        string ISettings.ParamDetail()
        {
            return GetAppSetting("paramDetail") ?? "simple";
        }

        private string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}
