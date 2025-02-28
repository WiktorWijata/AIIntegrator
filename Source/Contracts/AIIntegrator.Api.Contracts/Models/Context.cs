using System.Collections.Generic;

namespace AIIntegrator.Api.Contracts.Models
{
    public class Context
    {
        public string Message { get; set; }
        public IEnumerable<AvailableFunction> Functions { get; set; }
    }
}
