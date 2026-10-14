using TechStore.Web.Extensions;
using TechStore.Web.Services;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<InMemoryOrderStore>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// En desarrollo no se reutilizan claves locales incompatibles de Data Protection.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
}

var app = builder.Build();
var logAfterStaticFiles = builder.Configuration.GetValue<bool>("RequestLogging:AfterStaticFiles");
var adminKey = builder.Configuration["AdminAccess:Key"] ?? "nexa-2026";

// Al colocar el logger aquí, también registra los archivos estáticos.
if (!logAfterStaticFiles)
{
    app.UseRequestLog();
}

// Middleware de demostración: sin clave, responde 403 y corta la canalización.
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

app.UseStaticFiles();

// Alternativa para comparar qué ocurre al ubicar el logger tras StaticFiles.
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

app.UseRouting();
app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{action=Index}/{id?}",
    defaults: new { controller = "Admin" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Fallback para solicitudes que no coinciden con una ruta MVC.
app.MapFallback(async context =>
{
    context.Response.StatusCode = StatusCodes.Status404NotFound;
    context.Response.ContentType = "text/plain; charset=utf-8";
    await context.Response.WriteAsync("Página no encontrada");
});

app.Run();

public partial class Program;
