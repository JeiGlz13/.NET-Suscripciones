using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Servicios
{
    public class EmitirFacturasHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer timer;

        public EmitirFacturasHostedService(
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(ProcesarFacturas, null, TimeSpan.Zero,TimeSpan.FromDays(1) );
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            return Task.CompletedTask;
        }

        private void ProcesarFacturas(object state)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                EstablecerMalaPaga(context);
                EmitirFacturas(context);
            }
        }

        private static void EstablecerMalaPaga(ApplicationDbContext context)
        {
            context.Database.ExecuteSqlRaw("exec SP_EstablecerMalaPaga");
        }

        private static void EmitirFacturas(ApplicationDbContext context)
        {
            var hoy = DateTime.Today;
            var fechaComparacion = hoy.AddMonths(-1);
            var facturasDelMesEmitidas = context.FacturasEmitidas
                .Any(x => x.Year == fechaComparacion.Year && x.Mes == fechaComparacion.Month);

            if (!facturasDelMesEmitidas)
            {
                var fechaInicio = new DateTime(fechaComparacion.Year, fechaComparacion.Month, 1);
                var fechaFin = fechaInicio.AddMonths(1);
                context.Database.ExecuteSqlInterpolated($"exec SP_CreacionFacturas {fechaInicio.ToString("yyyy-MM-dd")}, {fechaFin.ToString("yyyy-MM-dd")}");
            }
        }
    }
}
