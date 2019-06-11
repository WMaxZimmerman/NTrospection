using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Models;

namespace NTrospection.CLI.Services
{
    public class CommandService
    {
	private IParameterService _parameterService;
	
        public CommandService(IParameterService parameterService = null)
        {
	    _parameterService = parameterService ?? new ParameterService();
        }

	public string GetCommandName(CommandMethod command)
	{
	    var attribute = (CliCommand)Attribute.GetCustomAttributes(command.Info)
                .FirstOrDefault(a => a is CliCommand);

            return attribute?.Name;
	}

	public List<string> GetParameterErrors(CommandMethod command, List<CommandLineArgument> args)
	{
	    var errors = new List<string>();
	    var arguments = args.Where(a => command.Info.GetParameters()
				       .All(p => p.Name != a.Command && p.GetCustomAttribute<CliParameter>()?.Alias.ToString() != a.Command));
	    
	    foreach (var argument in arguments)
            {
                errors.Add($"The parameter '{argument.Command}' is not a valid parameter");
            }

	    return errors;
	}

	public bool ParameterMatchesArg(CommandMethod command, ParameterInfo pi, CommandLineArgument arg)
	{
	    var isPresent = true;
	    
	    if (arg.Command.ToLower() != pi.Name.ToLower())
	    {
		var paramAttribute = pi.GetCustomAttribute<CliParameter>();
		if (paramAttribute != null)
		{
		    if (paramAttribute.Alias.ToString().ToLower() != arg.Command.ToLower()) isPresent = false;
		}
		else
		{
		    isPresent = false;
		}
	    }
	    
	    return isPresent;
	}

	// public void SetParameters(List<CommandLineArgument> args)
        // {
        //     var methodParams = new MethodParameters();

        //     foreach (var argument in args.Where(a => Info.GetParameters().All(p => p.Name != a.Command && p.GetCustomAttribute<CliParameter>()?.Alias.ToString() != a.Command)))
        //     {
        //         methodParams.Errors.Add($"The parameter '{argument.Command}' is not a valid parameter.");
        //     }

        //     foreach (var parameter in Info.GetParameters())
        //     {
        //         var wasFound = false;
        //         foreach (var argument in args)
        //         {
        //             if (argument.Command.ToLower() != parameter.Name.ToLower())
        //             {
        //                 var paramAttribute = parameter.GetCustomAttribute<CliParameter>();
        //                 if (paramAttribute != null)
        //                 {
        //                     if (paramAttribute.Alias.ToString().ToLower() != argument.Command.ToLower()) continue;
        //                 }
        //                 else
        //                 {
        //                     continue;
        //                 }
        //             }
        //             wasFound = true;

        //             var type = parameter.ParameterType;

        //             if (typeof(IEnumerable).IsAssignableFrom(type) && type.Name != "String")
        //             {
        //                 if (type.GetGenericArguments().Length <= 0)
        //                 {
        //                     var underType = type.GetElementType();
        //                     var listType = typeof(List<>).MakeGenericType(underType);
        //                     var list = (IList)Activator.CreateInstance(listType);

        //                     foreach (var value in argument.Values)
        //                     {
        //                         list.Add(_parameterService.GetParamValue(value, underType));
        //                     }

        //                     var array = Array.CreateInstance(underType, list.Count);
        //                     list.CopyTo(array, 0);

        //                     methodParams.Parameters.Add(array);
        //                 }
        //                 else
        //                 {
        //                     var underType = type.GenericTypeArguments[0];
        //                     var listType = typeof(List<>).MakeGenericType(underType);
        //                     var list = (IList)Activator.CreateInstance(listType);

        //                     foreach (var value in argument.Values)
        //                     {
        //                         list.Add(_parameterService.GetParamValue(value, underType));
        //                     }

        //                     methodParams.Parameters.Add(list);
        //                 }
        //             }
        //             else if (type.Name == "Boolean")
        //             {
        //                 if (argument.Values.Count <= 0) argument.Values.Add("true");
        //                 dynamic val = _parameterService.GetParamValue(argument.Values[0], parameter.ParameterType);
        //                 methodParams.Parameters.Add(val);
        //             }
        //             else
        //             {
        //                 dynamic val = _parameterService.GetParamValue(argument.Values[0], parameter.ParameterType);
        //                 methodParams.Parameters.Add(val);
        //             }
        //         }

        //         if (!wasFound)
        //         {
        //             if (parameter.HasDefaultValue)
        //             {
        //                 methodParams.Parameters.Add(parameter.DefaultValue);
        //             }
        //             else
        //             {
        //                 methodParams.Errors.Add($"The parameter '{parameter.Name}' must be specified.");
        //             }
        //         }
        //     }

        //     Parameters = methodParams;
        // }
    }
}
