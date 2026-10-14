using TechStore.Web.Middleware;

namespace TechStore.Web.Extensions;

public static class RequestLogMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLog(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<RequestLogMiddleware>();
    }
}
