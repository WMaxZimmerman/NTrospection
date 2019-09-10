using NTrospection.CLI.Core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTrospection.CLI.Services
{
    public interface IArgumentProcessorService
    {
        bool ProcessArguments(IReadOnlyList<string> args, Assembly ProjectAssembly);
    }
    
    public class ArgumentProcessorService: IArgumentProcessorService
    {
        private ISettings _settings;
        private IControllerService _controllerService;
        private IArgumentService _argService;
        private IConsoleService _console;

        public ArgumentProcessorService(ISettings settings = null,
                                        IControllerService controllerService = null,
                                        IArgumentService argService = null,
                                        IConsoleService console = null)
        {
            _settings = settings ?? new Settings();
            _controllerService = controllerService ?? new ControllerService();
            _argService = argService ?? new ArgumentService();
            _console = console ?? new ConsoleService();
        }

        public bool ProcessArguments(IReadOnlyList<string> args, Assembly ProjectAssembly)
        {
            var controllers = _controllerService.GetControllers(ProjectAssembly);
            var arguments = _argService.ProcessArgs(args);

            if (arguments.IsHelpCall)
            {
                var controller = controllers.FirstOrDefault(c => c.Name == arguments.Controller);
                if (controller == null)
                {
                    foreach (var c in controllers)
                    {
                        _controllerService.OutputDocumentation(c);
                    }
                    return true;
                }

                _controllerService.DocumentCommand(controller, arguments.Command);
                return true;
            }
            else
            {
                var controller = controllers.FirstOrDefault(c => c.Name == arguments.Controller);
                if (controller == null)
                {
                    _console.WriteLine($"'{arguments.Controller}' is not a valid controller. Use '{_settings.HelpString()}' to see available controllers.");
                    return false;
                }

                var invoke = _controllerService.ExecuteCommand(controller, arguments.Command, arguments.Arguments);

                foreach (var message in invoke.Messages)
                {
                    _console.WriteLine(message);
                }

                return invoke.WasSuccess;
            }
        }
    }
}
