using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController: ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ServicioLlaves _servicioLlaves;


        public CuentasController(
            UserManager<Usuario> userManager, 
            IConfiguration configuration,
            SignInManager<Usuario> signInManager,
            ServicioLlaves servicioLlaves
        )
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _servicioLlaves = servicioLlaves;
        }

        [HttpPost("registrar", Name = "registrarUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new Usuario
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email,
            };
            var resultado = await _userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                await _servicioLlaves.CrearLlave(usuario.Id, TipoLlave.Gratuita);
                return await ConstruirToken(credencialesUsuario, usuario.Id);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login", Name = "iniciarSesion")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await _signInManager.PasswordSignInAsync(credencialesUsuario.Email, 
                credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                var usuario = await _userManager.FindByEmailAsync(credencialesUsuario.Email);
                return await ConstruirToken(credencialesUsuario, usuario.Id);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }

        [HttpGet("RenovarToken", Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> RenovarToken()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var idClaim = HttpContext.User.Claims.Where(claim => claim.Type == "id").FirstOrDefault();
            var usuarioId = idClaim.Value;

            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = email
            };

            return await ConstruirToken(credencialesUsuario, usuarioId);
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario, string usuarioId)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email),
                new Claim("id", usuarioId)
            };

            var usuario = await _userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsBD = await _userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsBD);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, 
                claims: claims, expires: expiracion, 
                signingCredentials: creds);


            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        [HttpPost("HacerAdmin", Name = "hacerAdmin")]
        public async Task<IActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(editarAdminDTO.Email);
            await _userManager.AddClaimAsync(usuario, new Claim("esAdmmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoverAdmin", Name = "removerAdmin")]
        public async Task<IActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(editarAdminDTO.Email);
            await _userManager.AddClaimAsync(usuario, new Claim("esAdmmin", "1"));
            return NoContent();
        }
    }
}
