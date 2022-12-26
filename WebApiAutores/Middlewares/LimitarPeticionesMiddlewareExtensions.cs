using Microsoft.EntityFrameworkCore;
using WebApiAutores;
using WebApiAutores.DTO.LlavesDTOs;
using WebApiAutores.Entidades;

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

        var ruta = httpContext.Request.Path.ToString();
        var estaEnListaBlanca = limitarPeticionesConfiguracion.ListaBlancaRutas
            .Any(x => ruta.Contains(x));

        if (estaEnListaBlanca)
        {
            await _next(httpContext);
            return;
        }

        var llaveStringValues = httpContext.Request.Headers["X-Api-Key"];
        if (llaveStringValues.Count == 0)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Debe proveer la llave en el header X-Api-Key");
            return;
        }

        if (llaveStringValues.Count > 1)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Solo una llave debe estar presente en el header X-Api-Key");
            return;
        }

        var llave = llaveStringValues[0];

        var llaveDb = await context
            .LlaveAPIs
            .Include(x => x.RestriccionesDominios)
            .Include(x => x.RestriccionesIPs)
            .FirstOrDefaultAsync(x => x.Llave == llave);

        if (llaveDb == null)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("La llave no existe");
            return;
        }

        if (!llaveDb.Activa)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("La llave se encuentra inactiva");
            return;
        }

        if (llaveDb.TipoLlave == TipoLlave.Gratuita)
        {
            var hoy = DateTime.Today;
            var mañana = hoy.AddDays(1);
            var cantidaPeticionesHoy = await context.Peticiones
                .CountAsync(x => x.LlaveId == llaveDb.Id && x.FechaPeticion >= hoy && x.FechaPeticion < mañana);

            if (cantidaPeticionesHoy >= limitarPeticionesConfiguracion.PeticionesPorDiaGratuito)
            {
                httpContext.Response.StatusCode = 429;
                await httpContext.Response.WriteAsync("Ha excedido el límite de peticiones por día para una cuenta gratuita.");
                return;
            }
        }

        var superaRestricciones = PeticionSuperaAlgunaRestriccion(llaveDb, httpContext);

        if (!superaRestricciones)
        {
            httpContext.Response.StatusCode = 403;
            return;
        }

        var peticion = new Peticion()
        {
            LlaveId = llaveDb.Id,
            FechaPeticion = DateTime.UtcNow
        };

        context.Add(peticion);
        await context.SaveChangesAsync();
        await _next(httpContext);
    }


    private bool PeticionSuperaAlgunaRestriccion(LlaveAPI llaveAPI, HttpContext httpContext)
    {
        var hayRestricciones = llaveAPI.RestriccionesDominios.Any() || llaveAPI.RestriccionesDominios.Any();

        if (!hayRestricciones)
        {
            return true;
        }

        var peticionSuperaDominio = PeticionSuperaRestrccionesDominio(llaveAPI.RestriccionesDominios, httpContext);
        var peticionSuperaIP = PeticionSuperaRestriccionesIP(llaveAPI.RestriccionesIPs, httpContext);
        return peticionSuperaDominio || peticionSuperaIP;
    }

    public bool PeticionSuperaRestriccionesIP
    (
        List<RestriccionIP> restriccionIPs,
        HttpContext httpContext
    )
    {
        if (restriccionIPs == null || restriccionIPs.Count == 0)
        {
            return false;
        }

        var IP = httpContext.Connection.RemoteIpAddress.ToString();

        if (IP == string.Empty)
        {
            return false;
        }

        var superaRestriccion = restriccionIPs.Any(x => x.IP == IP);
        return superaRestriccion;
    }

    private bool PeticionSuperaRestrccionesDominio(
        List<RestriccionesDominio> restriccionesDominio,
        HttpContext httpContext
    )
    {
        if (restriccionesDominio == null || restriccionesDominio.Count == 0)
        {
            return false;
        }

        var referer = httpContext.Request.Headers["Referer"].ToString();

        if (referer == String.Empty)
        {
            return false;
        }

        Uri myUri = new Uri(referer);

        string host = myUri.Host;

        var superaRestriccion = restriccionesDominio
            .Any(x => x.Dominio == host);

        return superaRestriccion;
    }


}
