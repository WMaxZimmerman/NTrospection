using System.Collections.Generic;
using System.Linq;

namespace NTrospection.CLI.Models
{
    public class CommandLineArgument
    {
        public string Command { get; set; }
        public int Order { get; set; }
        public List<string> Values { get; set; }

        public CommandLineArgument()
        {
            Values = new List<string>();
        }

        public override bool Equals(object o)
        {
            return o is CommandLineArgument ? Equals((CommandLineArgument) o) : false;
        }

        private bool Equals(CommandLineArgument o)
        {
            return this.Command == o.Command
                && this.Order == o.Order
                && this.Values.SequenceEqual(o.Values);
        }
    }
}
