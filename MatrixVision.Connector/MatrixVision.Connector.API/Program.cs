using MatrixVision.Connector.Core;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddScoped<MVConnector>();

var app = builder.Build();

app.MapGet("/test", (HttpContext context, MVConnector connector) =>
{
    connector.Test();

    return Results.Ok();
});

app.Run();
