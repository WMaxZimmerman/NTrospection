using NTrospection.CLI.Services;
using System.Reflection;

namespace NTrospection.CLI.Core
{
    public interface ILooper
    {
        void ApplicationLoop(Assembly projectAssembly);
    }
    
    public class Looper: ILooper
    {
        private ISettings _settings;
        private IConsoleService _console;
        private IArgumentProcessorService _argumentProcessor;

        public Looper(ISettings settings = null,
                         IConsoleService console = null,
                         IArgumentProcessorService argumentProcessor = null)
        {
            _settings = settings ?? new Settings();
            _console = console ?? new ConsoleService();
            _argumentProcessor = argumentProcessor ?? new ArgumentProcessorService();
        }

        public void ApplicationLoop(Assembly projectAssembly)
        {
            _console.Write(_settings.InputIndicator() + " ");
            var input = CommandLine.GetCommandLineArgs(_console.ReadLine());

            while (input.Length == 0 || input[0] != _settings.ExitString())
            {
                if (input.Length > 0) _argumentProcessor.ProcessArguments(input, projectAssembly);
                _console.WriteLine("");
                _console.Write(_settings.InputIndicator() + " ");
                input = CommandLine.GetCommandLineArgs(_console.ReadLine());
            }
        }
    }
}
