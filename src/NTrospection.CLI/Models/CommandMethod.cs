using NTrospection.CLI.Core;
using NTrospection.CLI.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NTrospection.CLI.Services;

namespace NTrospection.CLI.Models
{
    public class CommandMethod
    {
        public string Name { get; set; }
        public MethodInfo Info { get; set; }
        public MethodParameters Parameters { get; set; }

	private ISettings _settings;
	private IParameterService _parameterService;
        
        public CommandMethod(MethodInfo info, ISettings settings = null, IParameterService parameterService = null)
        {
	    _settings = settings ?? new Settings();
	    _parameterService = parameterService ?? new ParameterService();
	    
            Info = info;
            Name = GetCommandName();
        }
        
        public CommandResponse Invoke(List<CommandLineArgument> args)
        {
	    var response = new CommandResponse();

            try
            {
                var paramList = GetParams(args);
                if (paramList == null)
		{
		    response.WasSuccess = false;
		}
		else
		{
		    if (Info.IsStatic)
		    {
			Info.Invoke(null, BindingFlags.Static, null, paramList, null);
		    }
		    else
		    {
			//Execute as if not static
			var target = Activator.CreateInstance(Info.DeclaringType);
			Info.Invoke(target, BindingFlags.Instance, null, paramList, null);
		    }

		    response.WasSuccess =  true;
		}
            }
            catch (TargetInvocationException e)
            {
                var inner = e.InnerException;
                response.Messages.Add("An error occurred while executing the command.");
                if (inner != null)
		{
		    response.Messages.Add($"Message: {inner.Message}");
		    response.Messages.Add($"Stack Trace: {inner.StackTrace.Trim()}");
		}
		
                response.WasSuccess =  false;
            }
            catch (Exception)
            {
                response.Messages.Add("An error occurred while attempting to execute the command.");
                response.Messages.Add("This is most likely due to invalid arguments.");
                response.Messages.Add($"Please verify the command usage with '{_settings.HelpString()}' and try again.");
                response.WasSuccess =  false;
            }

	    return response;
        }

        public void SetParameters(List<CommandLineArgument> args)
        {
            var methodParams = new MethodParameters();

            foreach (var argument in args.Where(a => Info.GetParameters().All(p => p.Name != a.Command && p.GetCustomAttribute<CliParameter>()?.Alias.ToString() != a.Command)))
            {
                methodParams.Errors.Add($"The parameter '{argument.Command}' is not a valid parameter.");
            }

            foreach (var parameter in Info.GetParameters())
            {
                var wasFound = false;
                foreach (var argument in args)
                {
                    if (argument.Command.ToLower() != parameter.Name.ToLower())
                    {
                        var paramAttribute = parameter.GetCustomAttribute<CliParameter>();
                        if (paramAttribute != null)
                        {
                            if (paramAttribute.Alias.ToString().ToLower() != argument.Command.ToLower()) continue;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    wasFound = true;

                    var type = parameter.ParameterType;

                    if (typeof(IEnumerable).IsAssignableFrom(type) && type.Name != "String")
                    {
                        if (type.GetGenericArguments().Length <= 0)
                        {
                            var underType = type.GetElementType();
                            var listType = typeof(List<>).MakeGenericType(underType);
                            var list = (IList)Activator.CreateInstance(listType);

                            foreach (var value in argument.Values)
                            {
                                list.Add(_parameterService.GetParamValue(value, underType));
                            }

                            var array = Array.CreateInstance(underType, list.Count);
                            list.CopyTo(array, 0);

                            methodParams.Parameters.Add(array);
                        }
                        else
                        {
                            var underType = type.GenericTypeArguments[0];
                            var listType = typeof(List<>).MakeGenericType(underType);
                            var list = (IList)Activator.CreateInstance(listType);

                            foreach (var value in argument.Values)
                            {
                                list.Add(_parameterService.GetParamValue(value, underType));
                            }

                            methodParams.Parameters.Add(list);
                        }
                    }
                    else if (type.Name == "Boolean")
                    {
                        if (argument.Values.Count <= 0) argument.Values.Add("true");
                        dynamic val = _parameterService.GetParamValue(argument.Values[0], parameter.ParameterType);
                        methodParams.Parameters.Add(val);
                    }
                    else
                    {
                        dynamic val = _parameterService.GetParamValue(argument.Values[0], parameter.ParameterType);
                        methodParams.Parameters.Add(val);
                    }
                }

                if (!wasFound)
                {
                    if (parameter.HasDefaultValue)
                    {
                        methodParams.Parameters.Add(parameter.DefaultValue);
                    }
                    else
                    {
                        methodParams.Errors.Add($"The parameter '{parameter.Name}' must be specified.");
                    }
                }
            }

            Parameters = methodParams;
        }
        
        public void OutputDocumentation()
        {
            var attrs = Attribute.GetCustomAttributes(Info);

            foreach (var attr in attrs)
            {
                if (!(attr is CliCommand)) continue;
                var a = (CliCommand)attr;

                Console.WriteLine();
                Console.WriteLine($"{a.Name}");
                Console.WriteLine($"Description: {a.Description}");
                var commandParams = Info.GetParameters();
                if (commandParams.Length > 0)
                {
                    Console.WriteLine($"Parameters:");
                    foreach (var cp in commandParams)
                    {
                        var paramDocs = _parameterService.GetParameterDocumentation(cp);
			foreach (var pd in paramDocs)
			{
			    Console.WriteLine(pd);
			}
                    }
                }
            }
        }

        public object[] GetParams(List<CommandLineArgument> args)
        {
            SetParameters(args);

            foreach (var error in Parameters.Errors)
            {
                Console.WriteLine(error);
            }

            return Parameters.Errors.Any() ? null : Parameters.Parameters.ToArray();
        }

        private string GetCommandName()
        {
            var attribute = (CliCommand)Attribute.GetCustomAttributes(Info)?
		.FirstOrDefault(a => a is CliCommand);

            return attribute?.Name;
        }
    }
}
