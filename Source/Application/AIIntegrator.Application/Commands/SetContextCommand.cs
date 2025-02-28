using AIIntegrator.Api.Contracts.Models;
using MediatR;

namespace AIIntegrator.Application.Commands;

public class SetContextCommand : IRequest
{
    public SetContextCommand(string system, Context context)
    {
        this.System = system;
        this.Context = context;
    }

    public Context Context { get; set; }
    public string System { get; set; }
}
