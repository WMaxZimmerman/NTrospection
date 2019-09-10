using NTrospection.CLI.Core;
using NTrospection.CLI.Models;
using System;
using System.Collections.Generic;

namespace NTrospection.CLI.Repositories
{
    public interface IArgumentRepository
    {
        string TryGetArg(IReadOnlyList<string> args, int index);
        IEnumerable<CommandLineArgument> SetArguments(IEnumerable<string> args);
    }

    public class ArgumentRepository: IArgumentRepository
    {
        private ISettings _settings;

        public ArgumentRepository(ISettings settings = null)
        {
            _settings = settings ?? new Settings();
        }

        public string TryGetArg(IReadOnlyList<string> args, int index)
        {
            try
            {
                var arg = args[index];
                return arg == _settings.HelpString() ? null : arg;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<CommandLineArgument> SetArguments(IEnumerable<string> args)
        {
            var arguments = new List<CommandLineArgument>();
            CommandLineArgument tempArg = null;

            foreach (var argument in args)
            {
                if (argument.StartsWith(_settings.ArgumentPrefix()))
                {
                    if (tempArg != null) arguments.Add(tempArg);
                    tempArg = new CommandLineArgument
                    {
                        Command = argument.Replace(_settings.ArgumentPrefix(), ""),
                        Order = arguments.Count + 1
                    };
                }
                else
                {
                    if (tempArg == null) continue;
                    tempArg.Values.Add(argument);
                }
            }
            
            arguments.Add(tempArg);

            return arguments;
        }
    }
}
