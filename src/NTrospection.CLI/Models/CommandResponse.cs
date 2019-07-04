using System.Collections.Generic;

namespace NTrospection.CLI.Models
{
    public class CommandResponse
    {
        public bool WasSuccess { get; set; }
        public List<string> Messages { get; set; }
        
        public CommandResponse()
        {
            Messages = new List<string>();
        }
    }
}
