using System.Reflection;
using System.Text;
using System.Text.Json;
using AIIntegrator.Api.Contracts.Models;
using AIIntegrator.Application.Commands;
using AIIntegrator.Application.Templates;
using MediatR;

namespace AIIntegrator.Application.CommandHandlers;

public class SetContextCommandHandler : IRequestHandler<SetContextCommand>
{
    public async Task Handle(SetContextCommand request, CancellationToken cancellationToken)
    {
        var directoryPath = Path.Combine("Contexts", request.System);
        var promptFileName = Path.Combine(directoryPath, $"context.txt");
        var contextFileName = Path.Combine(directoryPath, $"context.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(promptFileName) || File.Exists(contextFileName))
        {
            File.Delete(promptFileName);
            File.Delete(contextFileName);
        }

        var contextJson = JsonSerializer.Serialize(request.Context, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(contextFileName, contextJson, Encoding.UTF8, cancellationToken);

        var prompt = this.PreparePrompt(request.Context);
        await File.WriteAllTextAsync(promptFileName, prompt, Encoding.UTF8, cancellationToken);
    }

    private string PreparePrompt(Context context)
    {
        var promptTemplate = this.GetPromptTemplate();
        var prompt = promptTemplate.Replace(ConstsMnemonics.Message, context.Message);
        prompt = prompt.Replace(ConstsMnemonics.AvailableFunctions, this.GetAvailableFunctions(context));
        return prompt.ToString();
    }

    private string GetPromptTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream(ConstsTemplates.ContextTemplate))
        {
            if (stream == null)
            {
                throw new FileNotFoundException("Resource not found", ConstsTemplates.ContextTemplate);
            }

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    private string GetAvailableFunctions(Context context)
    {
        var availableFunctions = new StringBuilder();
        var count = 0;
        foreach (var function in context.Functions)
        {
            count++;
            availableFunctions.AppendLine($"{count}. Function: {function.Name} ");
            availableFunctions.AppendLine($"   Description: {function.Description} ");
            availableFunctions.AppendLine($"   Parameters: ");
            if (function.Parameters != null && function.Parameters.Any())
            {
                foreach (var parameter in function.Parameters)
                {
                    availableFunctions.AppendLine($"    - {parameter.Name} - ({parameter.Type}): {parameter.Description}");
                }
            }
            availableFunctions.AppendLine();
        }
        return availableFunctions.ToString();
    }
}
