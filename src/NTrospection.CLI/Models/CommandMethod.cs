using NTrospection.CLI.Core;
using NTrospection.CLI.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NTrospection.CLI.Services;

namespace NTrospection.CLI.Models
{
    public class CommandMethod
    {
        public string Name { get; set; }
        public MethodInfo Info { get; set; }
        public MethodParameters Parameters { get; set; }

        private IMethodService _methodService;

        public CommandMethod(MethodInfo info, IMethodService methodService = null)
        {
            _methodService = methodService ?? new MethodService();
            
            Info = info;
            Name = _methodService.GetCommandName(Info);
        }
    }
}
