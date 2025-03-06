var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.AIIntegrator_Api>("aiintegrator-api");
builder.AddProject<Projects.AIIntegrator_Web>("aiintegrator-web");
builder.Build().Run();
