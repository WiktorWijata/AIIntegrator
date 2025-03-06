using AIIntegrator.Application;
using AIIntegrator.Application.Commands;
using AIIntegrator.Application.Setup;
using AIIntegrator.ExternalContracts;
using Microsoft.Extensions.Options;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:63601")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.Configure<AISettings>(builder.Configuration.GetSection("OpenRouter.AI"));

builder.Services.AddTransient<OpenRouterHttpClientHandler>();
builder.Services.AddRefitClient<IOpenRouterApi>()
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        var settings = serviceProvider.GetRequiredService<IOptions<AISettings>>().Value;
        client.BaseAddress = new Uri(settings.Url);
    })
    .ConfigurePrimaryHttpMessageHandler<OpenRouterHttpClientHandler>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendChatMessageCommand).Assembly));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
