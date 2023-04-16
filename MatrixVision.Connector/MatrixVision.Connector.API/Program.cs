using MatrixVision.Connector.API.App;
using MatrixVision.Connector.Core;

var builder = WebApplication.CreateBuilder(args);

ConfigurationAccessor.Configuration = builder.Configuration;

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapCameraEndpoints();

app.Run();
