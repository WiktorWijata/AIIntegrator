using System.Threading.Tasks;
using AIIntegrator.ExternalContracts.Models.Response;
using AIIntegrator.ExternalContracts.Models.Request;
using Refit;

namespace AIIntegrator.ExternalContracts
{
    public interface IOpenRouterApi
    {
        [Post("/chat/completions")]
        Task<ChatResponse> SendMessageAsync(ChatRequest chatRequest);
    }
}
