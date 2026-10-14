using TechStore.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var logAfterStaticFiles = builder.Configuration.GetValue<bool>("RequestLogging:AfterStaticFiles");
var adminKey = builder.Configuration["AdminAccess:Key"] ?? "nexa-2026";

if (!logAfterStaticFiles)
{
    app.UseRequestLog();
}

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/admin") &&
        context.Request.Query["clave"] != adminKey)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Acceso denegado");
        return;
    }

    await next(context);
});

app.UseDefaultFiles();
app.UseStaticFiles();

if (logAfterStaticFiles)
{
    app.UseRequestLog();
}

app.Map("/estado", statusApp =>
{
    statusApp.Run(async context =>
    {
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Nexa está operativo");
    });
});

app.Map("/admin", adminApp =>
{
    adminApp.Run(async context =>
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.SendFileAsync(
            Path.Combine(app.Environment.WebRootPath, "admin.html"));
    });
});

app.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status404NotFound;
    context.Response.ContentType = "text/plain; charset=utf-8";
    await context.Response.WriteAsync("Página no encontrada");
});

app.Run();

public partial class Program;
