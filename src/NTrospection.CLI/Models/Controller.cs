using NTrospection.CLI.Common;
using NTrospection.CLI.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTrospection.CLI.Models
{
    public class Controller
    {
        public string Name { get; set; }
        public Type ClassType { get; set; }
        public List<CommandMethod> Methods { get; set; }
        public CommandMethod DefaultMethod { get; set; }
        
        public Controller(Type type)
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
        }
        
        public bool ExecuteCommand(string commandName, List<CommandLineArgument> args)
        {
            var command = Methods.FirstOrDefault(c => c.Name == commandName);
            if (command == null)
            {
                if (DefaultMethod != null) return DefaultMethod.Invoke(args);

                Console.WriteLine($"'{commandName}' is not a valid command.  Use '{Settings.HelpString}' to see available commands.");
                return false;
            }

            return command.Invoke(args);
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
                    method.OutputDocumentation();
                }
                return;
            }

            command.OutputDocumentation();
        }

        private string GetControllerName()
        {
            var attribute = (CliController)Attribute.GetCustomAttributes(ClassType)
                .FirstOrDefault(a => a is CliController);

            return attribute?.Name;
        }
    }
}
