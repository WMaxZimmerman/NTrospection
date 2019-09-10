using NTrospection.CLI.Core;
using NTrospection.CLI.Models;
using NTrospection.CLI.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace NTrospection.CLI.Services
{
    public interface IArgumentService
    {
        ProcessedArguments ProcessArgs(IReadOnlyList<string> args);
    }
    
    public class ArgumentService: IArgumentService
    {
        private ISettings _settings;
        private IArgumentRepository _argService;

        public ArgumentService(ISettings settings = null,
                               IArgumentRepository argService = null)
        {
            _settings = settings ?? new Settings();
            _argService = argService ?? new ArgumentRepository();
        }

        public ProcessedArguments ProcessArgs(IReadOnlyList<string> args)
        {
            var processedArguments = new ProcessedArguments();
            var argsStart = 2;

            if (args.Count == 0) return processedArguments;

            processedArguments.IsHelpCall = args[args.Count - 1] == _settings.HelpString();
            processedArguments.Controller = _argService.TryGetArg(args, 0);
            processedArguments.Command = _argService.TryGetArg(args, 1);

            if (processedArguments.Command?.StartsWith(_settings.ArgumentPrefix()) == true)
            {
                argsStart = 1;
                processedArguments.Command = null;
            }

            if (args.Count > argsStart && !processedArguments.IsHelpCall)
            {
                var argsList = args.ToList();
                argsList.RemoveRange(0, argsStart);
                processedArguments.Arguments = _argService.SetArguments(argsList).ToList();
            }

            return processedArguments;
        }
    }
}
