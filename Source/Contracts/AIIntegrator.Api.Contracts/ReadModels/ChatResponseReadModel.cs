namespace AIIntegrator.Api.Contracts.ReadModels
{
    public class ChatResponseReadModel
    {
        public string UserMessage { get; set; }
        public string Response { get; set; }
        
        public FunctionReadModel Function { get; set; }
    }
}
