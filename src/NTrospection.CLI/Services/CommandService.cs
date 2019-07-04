using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Core;
using NTrospection.CLI.Models;

namespace NTrospection.CLI.Services
{
    public interface ICommandService
    {
        string GetCommandName(MethodInfo info);
        object[] GetParameters(MethodInfo info, List<CommandLineArgument> args);
        void OutputDocumentation(MethodInfo info);
        CommandResponse Invoke(MethodInfo info, List<CommandLineArgument> args);
    }

    public class CommandService : ICommandService
    {
        private IParameterService _parameterService;
        private IMethodService _methodService;
        private IConsoleService _consoleService;
        private ISettings _settings;

        public CommandService(IParameterService parameterService = null,
                              IMethodService methodService = null,
                              IConsoleService consoleService = null,
                              ISettings settings = null)
        {
            _parameterService = parameterService ?? new ParameterService();
            _methodService = methodService ?? new MethodService();
            _consoleService = consoleService ?? new ConsoleService();
            _settings = settings ?? new Settings();
        }

        public string GetCommandName(MethodInfo info)
        {
            var attribute = (CliCommand)Attribute.GetCustomAttributes(info)
                    .FirstOrDefault(a => a is CliCommand);

            return attribute?.Name;
        }

        public object[] GetParameters(MethodInfo info, List<CommandLineArgument> args)
        {
            var parameters = _methodService.GetParameters(info, args);

            foreach (var error in parameters.Errors)
            {
                _consoleService.WriteLine(error);
            }

            return parameters.Errors.Any() ? null : parameters.Parameters.ToArray();
        }

        public void OutputDocumentation(MethodInfo info)
        {
            var attribute = (CliCommand)Attribute.GetCustomAttributes(info)?.FirstOrDefault(a => a is CliCommand);
            if (attribute == null) return;

            _consoleService.WriteLine("");
            _consoleService.WriteLine($"{attribute.Name}");
            _consoleService.WriteLine($"Description: {attribute.Description}");
            var commandParams = info.GetParameters();
            if (commandParams.Length > 0)
            {
                _consoleService.WriteLine($"Parameters:");
                foreach (var cp in commandParams)
                {
                    var paramDocs = _parameterService.GetParameterDocumentation(cp);
                    foreach (var pd in paramDocs)
                    {
                        _consoleService.WriteLine(pd);
                    }
                }
            }
        }

        public CommandResponse Invoke(MethodInfo info, List<CommandLineArgument> args)
        {
            var response = new CommandResponse();

            try
            {
                var paramList = GetParameters(info, args);
                if (paramList == null)
                {
                    response.WasSuccess = false;
                }
                else
                {
                    if (info.IsStatic)
                    {
                        _methodService.InvokeStatic(info, paramList);
                    }
                    else
                    {
                        _methodService.InvokeInstance(info, paramList);
                    }

                    response.WasSuccess =  true;
                }
            }
            catch (TargetInvocationException e)
            {
                response.Messages.Add("An error occurred while executing the command.");
                var inner = e.InnerException;
                if (inner != null)
            	{
            	    response.Messages.Add($"Message: {inner.Message}");
            	    response.Messages.Add($"Stack Trace: {inner.StackTrace.Trim()}");
            	}

                response.WasSuccess =  false;
            }
            catch (Exception e)
            {
                response.Messages.Add("An error occurred while attempting to execute the command.");
                response.Messages.Add("This is most likely due to invalid arguments.");
                response.Messages.Add($"Please verify the command usage with '{_settings.HelpString()}' and try again.");
                response.WasSuccess =  false;
            }

            return response;
        }
    }
}
