using System.Collections.Generic;
using System.Linq;

namespace NTrospection.CLI.Models
{
    public class ProcessedArguments
    {
        public string Controller { get; set; }
        public string Command { get; set; }
        public List<CommandLineArgument> Arguments { get; set; }
        public bool IsHelpCall { get; set; }
        
        public ProcessedArguments()
        {
            Controller = null;
            Command = null;
            Arguments = new List<CommandLineArgument>();
        }

        public override bool Equals(object o)
        {
            return o is ProcessedArguments ? Equals((ProcessedArguments) o) : false;
        }

        private bool Equals(ProcessedArguments o)
        {
            return this.Controller == o.Controller
                && this.Command == o.Command
                && this.Arguments.SequenceEqual(o.Arguments)
                && this.IsHelpCall == o.IsHelpCall;
        }
    }
}
