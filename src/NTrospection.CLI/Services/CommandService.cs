using System;
using System.Linq;
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
    }
}
