using MatrixVision.Connector.Core;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddScoped<MVConnector>();

var app = builder.Build();

app.MapGet("/test", (MVConnector connector) =>
{
    connector.Test();

    return Results.Ok();
});

app.Run();
