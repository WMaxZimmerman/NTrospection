using NTrospection.CLI.Core;
using NTrospection.CLI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NTrospection.CLI.Services;

namespace NTrospection.CLI.Models
{
    public class Controller
    {
        public string Name { get; set; }
        public Type ClassType { get; set; }
        public List<CommandMethod> Methods { get; set; }
        public CommandMethod DefaultMethod { get; set; }

        private ISettings _settings;
        private ICommandService _commandService;

        public Controller(Type type,
              ISettings settings = null,
              ICommandService commandService = null)
        {
            ClassType = type;
            Name = GetControllerName();
            Methods = ClassType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => Attribute.GetCustomAttributes(m).Any(a => a is CliCommand))
                .Select(c => new CommandMethod(c)).ToList();

            Methods.AddRange(ClassType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => Attribute.GetCustomAttributes(m).Any(a => a is CliCommand))
                .Select(c => new CommandMethod(c)).ToList());

            DefaultMethod = ClassType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                      .Where(m => Attribute.GetCustomAttributes(m).Any(a => a is CliDefaultCommand))
                                      .Select(c => new CommandMethod(c)).FirstOrDefault();

            if (DefaultMethod != null) Methods.Add(DefaultMethod);

            _settings = settings ?? new Settings();
            _commandService = commandService ?? new CommandService();
        }

        public CommandResponse ExecuteCommand(string commandName, List<CommandLineArgument> args)
        {
            var command = Methods.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
            {
                if (DefaultMethod != null) return _commandService.Invoke(DefaultMethod.Info, args);

                var response = new CommandResponse();
                response.Messages.Add($"'{commandName}' is not a valid command.  Use '{_settings.HelpString()}' to see available commands.");
                response.WasSuccess = false;
                return response;
            }

            return _commandService.Invoke(command.Info, args);
        }

        public void OutputDocumentation()
        {
            var attrs = Attribute.GetCustomAttributes(ClassType);
            foreach (var attr in attrs)
            {
                if (!(attr is CliController)) continue;
                var a = (CliController)attr;

                Console.WriteLine($"{a.Name} - {a.Description}");
            }
        }

        public void DocumentCommand(string commandName)
        {
            var command = Methods.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
            {
                foreach (var method in Methods)
                {
                    _commandService.OutputDocumentation(method.Info);
                }
                return;
            }

            _commandService.OutputDocumentation(command.Info);
        }

        private string GetControllerName()
        {
            var attribute = (CliController)Attribute.GetCustomAttributes(ClassType)
                .FirstOrDefault(a => a is CliController);

            return attribute?.Name;
        }
    }
}
