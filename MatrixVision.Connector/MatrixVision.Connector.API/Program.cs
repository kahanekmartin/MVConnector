using MatrixVision.Connector.API.App;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapCameraEndpoints();

app.Run();
