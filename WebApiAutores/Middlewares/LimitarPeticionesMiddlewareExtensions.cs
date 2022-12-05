using WebApiAutores;
using WebApiAutores.DTO.LlavesDTOs;

namespace WebApiAutores.Middlewares
{
    public static class LimitarPeticionesMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitarPeticionesMiddleware>();
        }
    }

}
public class LimitarPeticionesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public LimitarPeticionesMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
    {
        var limitarPeticionesConfiguracion = new LimitarPeticionesConfiguracion();
        _configuration.GetSection("limitarPeticiones").Bind(limitarPeticionesConfiguracion);

        var llaveStringValues = httpContext.Request.Headers["X-Api-Key"];
        if (llaveStringValues.Count == 0)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Debe proveer la llave en el header X-Api-Key");
            return;
        }
        await _next(httpContext);
    }
}
