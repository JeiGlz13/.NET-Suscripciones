using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO.Facturas;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/facturas")]
    public class Facturas: CustomBaseController        
    {
        private readonly ApplicationDbContext _context;
        public Facturas(
            ApplicationDbContext context
        )
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pagar(PagarFacturaDTO pagarFacturaDTO)
        {
            var facturaDB = await _context.Facturas
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == pagarFacturaDTO.FacturaId);

            if (facturaDB == null)
            {
                return NotFound();
            }

            if (facturaDB.Pagada)
            {
                return BadRequest("La factura ya ha sido pagada");
            }

            // Logica pasarela de pago

            facturaDB.Pagada = true;
            await _context.SaveChangesAsync();

            var hayFacturasPendientesVencidas = await _context.Facturas
                .AnyAsync(x => x.UsuarioId == facturaDB.UsuarioId
                && !x.Pagada
                && x.FechaLimitePago < DateTime.Today);

            if (!hayFacturasPendientesVencidas)
            {
                facturaDB.Usuario.NoHaPagado = false;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
