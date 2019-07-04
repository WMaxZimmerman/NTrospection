using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Models;

namespace NTrospection.CLI.Services
{
    public interface IMethodService
    {
        string GetCommandName(MethodInfo info);
        MethodParameters GetParameters(MethodInfo info, List<CommandLineArgument> args);
        void InvokeStatic(MethodInfo info, object[] parameters);
        void InvokeInstance(MethodInfo info, object[] parameters);
    }

    public class MethodService : IMethodService
    {
        private IParameterService _parameterService;

        public MethodService(IParameterService parameterService = null)
        {
            _parameterService = parameterService ?? new ParameterService();
        }

        public string GetCommandName(MethodInfo info)
        {
            var attribute = (CliCommand)Attribute.GetCustomAttributes(info)
                    .FirstOrDefault(a => a is CliCommand);

            return attribute?.Name;
        }

        public void InvokeStatic(MethodInfo info, object[] parameters)
        {
            info.Invoke(null, BindingFlags.Static, null, parameters, null);
        }
        
        public void InvokeInstance(MethodInfo info, object[] parameters)
        {
            var target = Activator.CreateInstance(info.DeclaringType);
            info.Invoke(target, BindingFlags.Instance, null, parameters, null);
        }

        public MethodParameters GetParameters(MethodInfo info, List<CommandLineArgument> args)
        {
            var methodParams = new MethodParameters();

            var errors = _parameterService.GetParameterErrors(info, args);
            methodParams.Errors.AddRange(errors);

            foreach (var parameter in info.GetParameters())
            {
                var wasFound = false;
                foreach (var argument in args)
                {
                    var found = _parameterService.ParameterMatchesArg(parameter, argument);
                    if (!found) continue;
                    wasFound = true;

                    methodParams.Parameters.Add(_parameterService.GetParameterValue(parameter, argument));
                }

                if (!wasFound)
                {
                    if (parameter.HasDefaultValue)
                    {
                        methodParams.Parameters.Add(parameter.DefaultValue);
                    }
                    else
                    {
                        methodParams.Errors.Add($"The parameter '{parameter.Name}' must be specified");
                    }
                }
            }

            return methodParams;
        }
    }
}
