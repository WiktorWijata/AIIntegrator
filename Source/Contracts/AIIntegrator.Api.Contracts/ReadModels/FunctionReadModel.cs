namespace AIIntegrator.Api.Contracts.ReadModels
{
    public class FunctionReadModel
    {
        public string Name { get; set; }
        public FunctionParameterReadModel[] Parameters { get; set; }
    }
}