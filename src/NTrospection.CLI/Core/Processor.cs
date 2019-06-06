using NTrospection.CLI.Attributes;
using NTrospection.CLI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTrospection.CLI.Core
{
    public class Processor
    {
	private ISettings _settings;
	
	public Processor(ISettings settings = null)
	{
	    _settings = settings == null ? new Settings() : settings;
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
                    ApplicationLoop(Assembly.GetCallingAssembly());
                }
                else
                {
                    Console.WriteLine($"Please enter a controller.  Use '{_settings.HelpString()}' to see available controllers.");
                    Environment.ExitCode = 1;
                }
                return;
            }

            var wasSuccess = ProcessArguments(args, Assembly.GetCallingAssembly());
            if (wasSuccess == false)
            {
                Environment.ExitCode = 1;
            }
        }

        private void ApplicationLoop(Assembly projectAssembly)
        {
            Console.Write(_settings.InputIndicator() + " ");
            var input = CommandLine.GetCommandLineArgs(Console.ReadLine());

            while (input.Length == 0 || input[0] != _settings.ExitString())
            {
                if (input.Length > 0) ProcessArguments(input, projectAssembly);
                Console.WriteLine();
                Console.Write(_settings.InputIndicator() + " ");
                input = CommandLine.GetCommandLineArgs(Console.ReadLine());
            }
        }

        private bool ProcessArguments(IReadOnlyList<string> args, Assembly ProjectAssembly)
        {
            var controllers = GetControllers(ProjectAssembly);

            var arguments = ProcessArgs(args);

            if (arguments.IsHelpCall)
            {
                var controller = controllers.FirstOrDefault(c => c.Name == arguments.Controller);
                if (controller == null)
                {
                    foreach (var c in controllers)
                    {
                        c.OutputDocumentation();
                    }
                    return true;
                }

                controller.DocumentCommand(arguments.Command);
                return true;
            }
            else
            {
                var controller = controllers.FirstOrDefault(c => c.Name == arguments.Controller);
                if (controller == null)
                {
                    Console.WriteLine($"'{args[0]}' is not a valid controller.  Use '{_settings.HelpString()}' to see available controllers.");
                    return false;
                }

                var invoke = controller.ExecuteCommand(arguments.Command, arguments.Arguments);

		foreach(var message in invoke.Messages)
		{
		    Console.WriteLine(message);
		}
		
                return invoke.WasSuccess;
            }
        }

        private IEnumerable<Controller> GetControllers(Assembly callingAssembly)
        {
            var controllerList = callingAssembly.GetTypes()
                .Where(t => Attribute.GetCustomAttributes(t).Any(a => a is CliController))
                .Select(t => new Controller(t));

            return controllerList;
        }

        private ProcessedArguments ProcessArgs(IReadOnlyList<string> args)
        {
            var processedArguments = new ProcessedArguments();
            var argsStart = 2;

            if (args.Count == 0) return processedArguments;

            processedArguments.IsHelpCall = args[args.Count - 1] == _settings.HelpString();
            processedArguments.Controller = TryGetArg(args, 0);
            processedArguments.Command = TryGetArg(args, 1);

            if (processedArguments.Command?.StartsWith(_settings.ArgumentPrefix()) == true)
            {
                argsStart = 1;
                processedArguments.Command = null;
            }

            if (args.Count > argsStart && !processedArguments.IsHelpCall)
            {
                var argsList = args.ToList();
                argsList.RemoveRange(0, argsStart);
                processedArguments.Arguments = SetArguments(argsList).ToList();
            }

            return processedArguments;
        }

        private string TryGetArg(IReadOnlyList<string> args, int index)
        {
            try
            {
                var arg = args[index];
                return arg == _settings.HelpString() ? null : arg;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IEnumerable<CommandLineArgument> SetArguments(IEnumerable<string> args)
        {
            var arguments = new List<CommandLineArgument>();
            CommandLineArgument tempArg = null;

            foreach (var argument in args)
            {
                if (argument.StartsWith(_settings.ArgumentPrefix()))
                {
                    if (tempArg != null) arguments.Add(tempArg);
                    tempArg = new CommandLineArgument
                    {
                        Command = argument.Replace(_settings.ArgumentPrefix(), ""),
                        Order = arguments.Count + 1
                    };
                }
                else
                {
                    if (tempArg == null) continue;
                    tempArg.Values.Add(argument);
                }
            }
            arguments.Add(tempArg);

            return arguments;
        }
    }
}
