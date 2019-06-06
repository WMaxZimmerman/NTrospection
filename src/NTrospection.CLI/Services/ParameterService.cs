using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NTrospection.CLI.Core;
using NTrospection.CLI.Attributes;

namespace NTrospection.CLI.Services
{
    public interface IParameterService
    {
	dynamic GetParamValue(string value, Type type);
    }
    
    public class ParameterService: IParameterService
    {
	private ISettings _settings;
	
        public ParameterService(ISettings settings = null)
        {
	    _settings = settings == null ? new Settings() : settings;
        }

	public dynamic GetParamValue(string value, Type type)
        {
	    if (Nullable.GetUnderlyingType(type) != null)
		type = Nullable.GetUnderlyingType(type);

            if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }

            dynamic pVal = Convert.ChangeType(value, type);
            return pVal;
        }

	public bool IsEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type.Name != "String";
        }

	public string GetAliasString(ParameterInfo pi)
	{
	    var aliasString = "";
	    var aliasChar = pi.GetCustomAttribute<CliParameter>()?.Alias;
	    if (aliasChar != null && aliasChar != default(char)) aliasString = $" | {_settings.ArgumentPrefix()}{aliasChar}";

	    return aliasString;
	}

	public string GetPriorityString(ParameterInfo pi)
	{
	    var output = pi.HasDefaultValue ? "Optional" : "Required";

	    if (pi.HasDefaultValue && _settings.ParamDetail() == "detailed")
		output += $" with a default value of {pi.DefaultValue ?? "null"}";
	    
	    return output;
	}

	// public List<string> GetParameterDocumentation(ParameterInfo cp)
        // {
	//     var output = new List<string>();
        //     var priorityString = cp.HasDefaultValue ? "Optional" : "Required";
        //     if (cp.HasDefaultValue && Settings.ParamDetail == "detailed") priorityString += $" with a default value of {cp.DefaultValue}";
        //     var type = cp.ParameterType;
        //     var isEnumerable = IsEnumerable(type);
        //     if (isEnumerable)
        //     {
        //         if (type.GetGenericArguments().Length <= 0)
        //         {
        //             type = type.GetElementType();
        //         }
        //         else
        //         {
        //             type = type.GenericTypeArguments[0];
        //         }
        //     }

        //     type = Nullable.GetUnderlyingType(type) ?? type;

        //     var typeString = $"{(isEnumerable ? "List of " : "")}{type.Name}";
        //     var aliasString = "";
        //     var descripitionString = "";
        //     if (cp.GetCustomAttribute<CliParameter>() != null)
        //     {
        //         if (cp.GetCustomAttribute<CliParameter>().Alias != default(char)) aliasString = $" | {Settings.ArgumentPrefix}{cp.GetCustomAttribute<CliParameter>().Alias}";
        //         if (cp.GetCustomAttribute<CliParameter>().Description != null) descripitionString = $"Description: {cp.GetCustomAttribute<CliParameter>().Description}";
        //     }
        //     var docString = $"{Settings.ArgumentPrefix}{cp.Name}{aliasString} ({typeString}): This parameter is {priorityString}";

        //     if (type.IsEnum)
        //     {
        //         var names = type.GetEnumNames();
        //         docString += $" and must be {(isEnumerable ? "a collection of " : "")}one of the following ({string.Join(", ", names)}).";
        //     }
        //     else
        //     {
        //         docString += ".";
        //     }

        //     output.Add(docString);
        //     if (descripitionString != "") output.Add(descripitionString);
	    
	//     return output;
        // }
    }
}
