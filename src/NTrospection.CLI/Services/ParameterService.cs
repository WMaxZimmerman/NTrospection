using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NTrospection.CLI.Core;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Models;
using System.Linq;

namespace NTrospection.CLI.Services
{
    public interface IParameterService
    {
	dynamic GetParamValue(string value, Type type);
	bool IsEnumerable(Type type);
	string GetAliasString(ParameterInfo pi);
	string GetPriorityString(ParameterInfo pi);
	string GetTypeString(ParameterInfo pi);
	string GetDescriptionString(ParameterInfo pi);
	string GetDocString(ParameterInfo pi, string priority, string type, string alias);
	List<string> GetParameterDocumentation(ParameterInfo cp);
	List<string> GetParameterErrors(MethodInfo info, List<CommandLineArgument> args);
	bool ParameterMatchesArg(ParameterInfo pi, CommandLineArgument arg);
	dynamic GetCollectionValues(ParameterInfo pi, CommandLineArgument arg);
	bool GetBoolValue(ParameterInfo pi, CommandLineArgument arg);
	dynamic GetParameterValue(ParameterInfo pi, CommandLineArgument arg);
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
	    if (aliasChar != null && aliasChar != default(char))
		aliasString = $" | {_settings.ArgumentPrefix()}{aliasChar}";

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

	public string GetDocString(ParameterInfo pi, string priority, string type, string alias)
	{
	    var paramType = pi.ParameterType;

	    var collectionString = "";
	    if (IsEnumerable(paramType))
	    {
		collectionString = "a collection of ";
		paramType = paramType.GetGenericArguments().Length <= 0 ?
		    paramType.GetElementType() :
		    paramType.GenericTypeArguments[0];
	    }
	 
	    var docString = $"{_settings.ArgumentPrefix()}{pi.Name}{alias} ({type}): This parameter is {priority}";   
            if (paramType.IsEnum)
            {
                var names = paramType.GetEnumNames();
                docString += $" and must be {collectionString}one of the following ({string.Join(", ", names)})";
            }

	    return docString;
	}

	public List<string> GetParameterDocumentation(ParameterInfo cp)
        {
	    var output = new List<string>();
	    
            var priorityString = GetPriorityString(cp);
	    var typeString = GetTypeString(cp);
            var aliasString = GetAliasString(cp);
            var docString = GetDocString(cp, priorityString, typeString, aliasString);
            var descriptionString = GetDescriptionString(cp);

            output.Add(docString);
            if (descriptionString != "") output.Add(descriptionString);
	    
	    return output;
        }

	

	public List<string> GetParameterErrors(MethodInfo info, List<CommandLineArgument> args)
	{
	    var errors = new List<string>();
	    var arguments = args.Where(a => info.GetParameters()
				       .All(p => p.Name != a.Command
					    && p.GetCustomAttribute<CliParameter>()?.Alias.ToString() != a.Command));
	    
	    foreach (var argument in arguments)
            {
                errors.Add($"The parameter '{argument.Command}' is not a valid parameter");
            }

	    return errors;
	}

	public bool ParameterMatchesArg(ParameterInfo pi, CommandLineArgument arg)
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

	public dynamic GetCollectionValues(ParameterInfo pi, CommandLineArgument arg)
	{
	    var type = pi.ParameterType;

	    var isArray = type.GetGenericArguments().Length <= 0;
	    var underType = isArray ? type.GetElementType() : type.GenericTypeArguments[0];
	    var listType = typeof(List<>).MakeGenericType(underType);
	    var list = (IList)Activator.CreateInstance(listType);

	    foreach (var value in arg.Values)
	    {
		list.Add(GetParamValue(value, underType));
	    }
	    
	    var array = Array.CreateInstance(underType, list.Count);
	    list.CopyTo(array, 0);
	    
	    return isArray ? Convert.ChangeType(array, type) : list;
	}

	public bool GetBoolValue(ParameterInfo pi, CommandLineArgument arg)
	{
	    if (arg.Values.Count <= 0) arg.Values.Add("true");
	    bool val = GetParamValue(arg.Values[0], pi.ParameterType);
	    return val;
	}

	public dynamic GetParameterValue(ParameterInfo pi, CommandLineArgument arg)
	{
	    var type = pi.ParameterType;
	    dynamic val = null;

	    if (typeof(IEnumerable).IsAssignableFrom(type) && type.Name != "String")
	    {
	    	val = GetCollectionValues(pi, arg);
	    }
	    else if (type.Name == "Boolean")
	    {
	    	val = GetBoolValue(pi, arg);
	    }
	    else
	    {
	    	val = GetParamValue(arg.Values[0], pi.ParameterType);
	    }
	    
	    return val;
	}
    }
}
