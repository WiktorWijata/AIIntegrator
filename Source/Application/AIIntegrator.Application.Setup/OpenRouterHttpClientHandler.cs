using Microsoft.Extensions.Options;

namespace AIIntegrator.Application.Setup;

public class OpenRouterHttpClientHandler : HttpClientHandler
{
    private readonly AISettings aiSettings;

    public OpenRouterHttpClientHandler(IOptions<AISettings> options)
    {
        this.aiSettings = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content != null)
        {
            request.Headers.Add("Authorization", $"Bearer {this.aiSettings.ApiKey}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
