using NTrospection.CLI.Attributes;
using NTrospection.CLI.Models;
using NTrospection.CLI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTrospection.CLI.Core
{
    public interface IProcessor
    {
        IEnumerable<ControllerVm> GetAllControllers(Assembly callingAssembly);
        void ProcessArguments(string[] args);
    }
    
    public class Processor: IProcessor
    {
        private ISettings _settings;
        private IControllerService _controllerService;
        private IConsoleService _console;
        private IArgumentProcessorService _argumentProcessor;
        private ILooper _looper;

        public Processor(ISettings settings = null,
                         IControllerService controllerService = null,
                         IConsoleService console = null,
                         IArgumentProcessorService argumentProcessor = null,
                         ILooper looper = null)
        {
            _settings = settings ?? new Settings();
            _controllerService = controllerService ?? new ControllerService();
            _console = console ?? new ConsoleService();
            _argumentProcessor = argumentProcessor ?? new ArgumentProcessorService();
            _looper = looper ?? new Looper();
        }

        public IEnumerable<ControllerVm> GetAllControllers(Assembly callingAssembly)
        {
            var controllerList = callingAssembly.GetTypes()
                .Where(t => Attribute.GetCustomAttributes(t).Any(a => a is CliController))
                .Select(t => new ControllerVm(t));

            return controllerList;
        }

        public void ProcessArguments(string[] args)
        {
            if (args.Length == 0)
            {
                if (_settings.ApplicationLoopEnabled())
                {
                    _looper.ApplicationLoop(Assembly.GetCallingAssembly());
                }
                else
                {
                    _console.WriteLine($"Please enter a controller. Use '{_settings.HelpString()}' to see available controllers.");
                    Environment.ExitCode = 1;
                }
                return;
            }

            var wasSuccess = _argumentProcessor.ProcessArguments(args, Assembly.GetCallingAssembly());
            if (wasSuccess == false)
            {
                Environment.ExitCode = 1;
            }
        }
    }
}
