using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO.RestriccionesDominio;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/restriccionesDominio")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesDominioController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;

        public RestriccionesDominioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CrearRestriccionesDominioDTO crearRestriccionesDominio)
        {
            var llaveDB = await _context.LlaveAPIs
                .FirstOrDefaultAsync(x => x.Id == crearRestriccionesDominio.LlaveId);

            if (llaveDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuarioId();

            if (llaveDB.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            var restriccionesDominio = new RestriccionesDominio()
            {
                LlaveId = crearRestriccionesDominio.LlaveId,
                Dominio = crearRestriccionesDominio.Dominio
            };

            _context.Add(restriccionesDominio);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ActualizarRestriccionDominioDTO actualizarRestriccionDominio)
        {
            var restriccionDB = await _context.RestriccionesDominios
                .Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuarioId();

            if (restriccionDB.Llave.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            restriccionDB.Dominio = actualizarRestriccionDominio.Dominio;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDB = await _context.RestriccionesDominios
                .Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuarioId();

            if (usuarioId != restriccionDB.Llave.UsuarioId)
            {
                return Forbid();
            }

            _context.Remove(restriccionDB);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
