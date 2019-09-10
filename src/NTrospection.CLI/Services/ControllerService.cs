using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Core;
using NTrospection.CLI.Models;

namespace NTrospection.CLI.Services
{
    public interface IControllerService
    {
        IEnumerable<Controller> GetControllers(Assembly callingAssembly);
        List<CommandMethod> GetCommandMethods(Controller controller);
        CommandMethod GetDefaultCommandMethod(Controller controller);
        CommandResponse ExecuteCommand(Controller controller, string commandName, List<CommandLineArgument> args);
        void OutputDocumentation(Controller controller);
        void DocumentCommand(Controller controller, string commandName);
        string GetControllerName(Controller controller);
    }

    public class ControllerService : IControllerService
    {
        private ISettings _settings;
        private IConsoleService _console;
        private ICommandService _commandService;

        public ControllerService(ISettings settings = null,
                                 IConsoleService console = null,
                                 ICommandService commandService = null)
        {
            _settings = settings ?? new Settings();
            _console = console ?? new ConsoleService();
            _commandService = commandService ?? new CommandService();
        }

        public IEnumerable<Controller> GetControllers(Assembly callingAssembly)
        {
            var controllerList = callingAssembly.GetTypes()
                .Where(t => Attribute.GetCustomAttributes(t).Any(a => a is CliController))
                .Select(t => new Controller(t));

            return controllerList;
        }

        public List<CommandMethod> GetCommandMethods(Controller controller)
        {
            var methods = new List<CommandMethod>();
            methods = controller.ClassType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => Attribute.GetCustomAttributes(m).Any(a => a is CliCommand))
                .Select(c => new CommandMethod(c)).ToList();

            methods.AddRange(controller.ClassType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => Attribute.GetCustomAttributes(m).Any(a => a is CliCommand))
                .Select(c => new CommandMethod(c)).ToList());

            var defaultMethod = controller.ClassType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                      .Where(m => Attribute.GetCustomAttributes(m).Any(a => a is CliDefaultCommand))
                                      .Select(c => new CommandMethod(c)).FirstOrDefault();

            if (defaultMethod != null) methods.Add(defaultMethod);
            return methods;
        }

        public CommandMethod GetDefaultCommandMethod(Controller controller)
        {
            var defaultMethod = controller.ClassType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                      .Where(m => Attribute.GetCustomAttributes(m).Any(a => a is CliDefaultCommand))
                                      .Select(c => new CommandMethod(c)).FirstOrDefault();

            return defaultMethod;
        }

        public CommandResponse ExecuteCommand(Controller controller, string commandName, List<CommandLineArgument> args)
        {
            var command = controller.Methods.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
            {
                if (controller.DefaultMethod != null) return _commandService.Invoke(controller.DefaultMethod.Info, args);

                return new CommandResponse
                {
                    WasSuccess = false,
                    Messages = new List<string>
                    {
                        $"'{commandName}' is not a valid command.  Use '{_settings.HelpString()}' to see available commands."
                    }
                };
            }

            return _commandService.Invoke(command.Info, args);
        }

        public void OutputDocumentation(Controller controller)
        {
            var attribute = (CliController)Attribute.GetCustomAttributes(controller.ClassType)
                .FirstOrDefault(a => a is CliController);

            if (attribute != null) _console.WriteLine($"{attribute.Name} - {attribute.Description}");;
        }

        public void DocumentCommand(Controller controller, string commandName)
        {
            var command = controller.Methods.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
            {
                foreach(var method in controller.Methods)
                {
                    _commandService.OutputDocumentation(method.Info);
                }
                return;
            }

            _commandService.OutputDocumentation(command.Info);
        }

        public string GetControllerName(Controller controller)
        {
            var attribute = (CliController)Attribute.GetCustomAttributes(controller.ClassType)
                .FirstOrDefault(a => a is CliController);

            return attribute?.Name;
        }
    }
}
