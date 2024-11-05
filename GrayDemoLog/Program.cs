using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "{Protocol} {RequestMethod} {RequestPath}{QueryString} responded {StatusCode} in {Elapsed:0.0000} ms";

    options.EnrichDiagnosticContext = (IDiagnosticContext diagnosticContext, HttpContext httpContext) =>
    {
        diagnosticContext.Set("Protocol", httpContext.Request.Protocol);

        if (httpContext.Request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", Uri.UnescapeDataString(httpContext.Request.QueryString.Value!));
        }
        else
        {
            diagnosticContext.Set("QueryString", "");
        }
    };
});

app.MapGet("/", () => "Hello, World!");

app.Run();
