using MediatR;

namespace AIIntegrator.Application.Queries;

public class GetPromptQuery : IRequest<string>
{
    public GetPromptQuery(string system)
    {
        this.System = system;
    }

    public string System { get; set; }
}
