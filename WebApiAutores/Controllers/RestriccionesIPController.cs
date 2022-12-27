using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Controllers.V1;
using WebApiAutores.DTO.RestriccionesIP;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/v1/restriccionesip")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesIPController: CustomBaseController
    {
        private readonly ApplicationDbContext _context;

        public RestriccionesIPController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CrearRestriccionIPDTO crearRestriccionIPDTO)
        {
            var llaveDB = await _context.LlaveAPIs
                .FirstOrDefaultAsync(x => x.Id == crearRestriccionIPDTO.LlaveId);

            if (llaveDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuarioId();

            if (llaveDB.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            var restriccionIP = new RestriccionIP()
            {
                LlaveId = crearRestriccionIPDTO.LlaveId,
                IP = crearRestriccionIPDTO.IP
            };

            _context.Add(restriccionIP);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ActualizarRestriccionIPDTO actualizarRestriccionIPDTO)
        {
            var restriccionDB = await _context.RestriccionIPs
                .Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuarioId();

            if(restriccionDB.Llave.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            restriccionDB.IP = actualizarRestriccionIPDTO.IP;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDB = await _context.RestriccionIPs
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
