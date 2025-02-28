using AIIntegrator.Api.Contracts.Models;
using MediatR;

namespace AIIntegrator.Application.Queries;

public class GetContextQuery : IRequest<Context>
{
    public GetContextQuery(string system)
    {
        this.System = system;
    }

    public string System { get; set; }
}
