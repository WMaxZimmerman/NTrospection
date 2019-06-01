using NTrospection.CLI.Common.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace NTrospection.CLI.Models
{
    public class CommandMethodVm
    {
        public string Name { get; set; }
        private MethodInfo Info { get; set; }

        public CommandMethodVm(MethodInfo info)
        {
            Info = info;
            Name = GetCommandName();
        }

        private string GetCommandName()
        {
            var attribute = (CliCommand)Attribute.GetCustomAttributes(Info)
                .FirstOrDefault(a => a is CliCommand);

            return attribute?.Name;
        }
    }
}
