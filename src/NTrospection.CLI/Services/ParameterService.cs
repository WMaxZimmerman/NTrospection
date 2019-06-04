using System;
using System.Collections;
using System.Collections.Generic;

namespace NTrospection.CLI.Services
{
    public interface IParameterService
    {
	dynamic GetParamValue(string value, Type type);
    }
    
    public class ParameterService: IParameterService
    {        
        public ParameterService()
        {
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
    }
}
