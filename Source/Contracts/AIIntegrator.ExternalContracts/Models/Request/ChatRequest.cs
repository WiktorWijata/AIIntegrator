namespace AIIntegrator.ExternalContracts.Models.Request
{
    public class ChatRequest
    {
        public string Model { get; set; }
        public Message[] Messages { get; set; }
        public decimal Temperature { get; set; } = 0.0m;
        public decimal Top_p { get; set; } = 0.0m;
    }
}
