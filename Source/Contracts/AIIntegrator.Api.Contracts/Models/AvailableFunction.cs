using System.Collections;
using System.Collections.Generic;

namespace AIIntegrator.Api.Contracts.Models
{
    public class AvailableFunction
    {
        public string Name { get; set; }
        public string Description { get; set; }   
        public IEnumerable<FunctionParameter> Parameters { get; set; }
    }
}
