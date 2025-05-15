using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ApiRest_LabWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly BdLabContext _context;
        private readonly IConfiguration _configuration;

        public UsuariosController(BdLabContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            Console.WriteLine("🔍 Conexión actual de la API REST:");
            Console.WriteLine(_context.Database.GetDbConnection().ConnectionString);
        }


        //Endpoint: POST /api/usuarios/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            Console.WriteLine($"🛠 BODY request: {request.CorreoUsuario} - {request.Clave}");

            var correo = request.CorreoUsuario.Trim().ToLower();
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.CorreoUsuario.ToLower() == correo);

            if (usuario == null)
                return Unauthorized("Correo no registrado");

            var claveOk = BCrypt.Net.BCrypt.Verify(request.Clave, usuario.ClaveUsuario);
            if (!claveOk)
                return Unauthorized("Contraseña incorrecta");

            if (!(usuario.EstadoRegistro ?? false))
                return Unauthorized("Usuario inactivo");


            var token = GenerarTokenJwt(usuario);
            Console.WriteLine("✅ Token generado desde API: " + token);


            return Ok(new LoginResponseDto
            {
                Token = GenerarTokenJwt(usuario),
                Nombre = usuario.Nombre,
                CorreoUsuario = usuario.CorreoUsuario, // <- AÑADIR
                Rol = usuario.Rol,
                EsContraseñaTemporal = usuario.EsContraseñaTemporal ?? true

            });



        }

        private string GenerarTokenJwt(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.CorreoUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest();

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }

        // Endpoint protegido para verificar si el token es válido y coincide con la base de datos
        [Authorize]
        [HttpGet("verificar-token")]
        public async Task<IActionResult> VerificarToken()
        {
            var correo = User.Identity?.Name?.Trim().ToLower();
            var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value?.Trim().ToLower();

            Console.WriteLine($"🔍 Correo del token: '{correo}'");
            Console.WriteLine($"🔍 Rol del token: '{rol}'");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.CorreoUsuario.Trim().ToLower() == correo &&
                u.Rol.Trim().ToLower() == rol &&
                u.EstadoRegistro == true);

            if (usuario == null)
            {
                Console.WriteLine("❌ Token inválido o desactualizado: no coincide con base.");
                return BadRequest("Token inválido o desactualizado.");
            }

            Console.WriteLine("✅ Token válido. Usuario confirmado: " + usuario.Nombre);
            return Ok("Token válido.");
        }


    }
}
