
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTO.LlavesDTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/llavesapi")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LlavesAPIController: CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ServicioLlaves _servicioLlaves;

        public LlavesAPIController(
                ApplicationDbContext context,
                IMapper mapper,
                ServicioLlaves servicioLlaves
        )
        {
            _context = context;
            _mapper = mapper;
            _servicioLlaves = servicioLlaves;
        }

        [HttpGet]
        public async Task<List<LlaveDTO>> MisLlaves()
        {
            var usuarioId = ObtenerUsuarioId();
            var llaves = await _context.LlaveAPIs.Where(x => x.UsuarioId == usuarioId).ToListAsync();

            return _mapper.Map<List<LlaveDTO>>(llaves);
        }

        [HttpPost]
        public async Task<ActionResult> CrearLlave(CrearLlaveDTO crearLlaveDTO)
        {
            var usuarioId = ObtenerUsuarioId();

            if (crearLlaveDTO.TipoLlave == TipoLlave.Gratuita)
            {
                var freeKeyAlreadyExist = await _context
                    .LlaveAPIs
                    .AnyAsync(x => x.UsuarioId == usuarioId && x.TipoLlave == TipoLlave.Gratuita);

                if (freeKeyAlreadyExist)
                {
                    return BadRequest("El usuario ya tiene una llave gratuita");
                }
            }

            await _servicioLlaves.CrearLlave(usuarioId, crearLlaveDTO.TipoLlave);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> ActualizarLlave(ActualizarLlaveDTO actualizarllaveDTO)
        {
            var usuarioId = ObtenerUsuarioId();
            var llaveDB = await _context
                .LlaveAPIs
                .FirstOrDefaultAsync(x => x.Id == actualizarllaveDTO.Id);

            if (llaveDB == null) { return NotFound(); }

            if (usuarioId != llaveDB.UsuarioId)
            {
                return Forbid();
            }

            if (actualizarllaveDTO.ActualizarLlave)
            {
                llaveDB.Llave = _servicioLlaves.GenerarLlave();
            }

            llaveDB.Activa = actualizarllaveDTO.Activa;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
