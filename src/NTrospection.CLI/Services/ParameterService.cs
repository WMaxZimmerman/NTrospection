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

	public string GetTypeString(ParameterInfo pi)
	{
	    var type = pi.ParameterType;
	    
	    if (Nullable.GetUnderlyingType(type) != null)
		type = Nullable.GetUnderlyingType(type);

	    var isEnumerable = IsEnumerable(type);
	    if (isEnumerable)
	    {
		type = type.GetGenericArguments().Length <= 0 ?
		    type.GetElementType() :
		    type.GenericTypeArguments[0];
	    }
	    
	    return $"{(isEnumerable ? "List of " : "")}{type.Name}";
	}

	public string GetDescriptionString(ParameterInfo pi)
	{
	    var descriptionString = "";
	    var description = pi.GetCustomAttribute<CliParameter>()?.Description;
	    if (description != null) descriptionString = $"Description: {description}";

	    return descriptionString;
	}

	public List<string> GetParameterDocumentation(ParameterInfo cp)
        {
	    var output = new List<string>();
	    
            var priorityString = GetPriorityString(cp);
	    var typeString = GetTypeString(cp);
            var aliasString = GetAliasString(cp);
            var descripitionString = GetDescriptionString(cp);
	    
            var docString = $"{_settings.ArgumentPrefix()}{cp.Name}{aliasString} ({typeString}): This parameter is {priorityString}";

            if (cp.ParameterType.IsEnum)
            {
                var names = cp.ParameterType.GetEnumNames();
                docString += $" and must be {(IsEnumerable(cp.ParameterType) ? "a collection of " : "")}one of the following ({string.Join(", ", names)}).";
            }
            else
            {
                docString += ".";
            }

            output.Add(docString);
            if (descripitionString != "") output.Add(descripitionString);
	    
	    return output;
        }
    }
}
