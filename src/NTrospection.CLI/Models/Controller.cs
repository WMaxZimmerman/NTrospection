using System;
using System.Collections.Generic;
using NTrospection.CLI.Services;

namespace NTrospection.CLI.Models
{
    public class Controller
    {
        public string Name { get; set; }
        public Type ClassType { get; set; }
        public List<CommandMethod> Methods { get; set; }
        public CommandMethod DefaultMethod { get; set; }

        private IControllerService _controllerService;

        public Controller(Type type, IControllerService controllerService = null)
        {
            _controllerService = controllerService ?? new ControllerService();
            
            ClassType = type;
            Name = _controllerService.GetControllerName(this);
            Methods = _controllerService.GetCommandMethods(this);
            DefaultMethod = _controllerService.GetDefaultCommandMethod(this);
        }
    }
}
