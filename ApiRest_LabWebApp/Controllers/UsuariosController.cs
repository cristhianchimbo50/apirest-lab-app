using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiRest_LabWebApp.Models;
using ApiRest_LabWebApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using ApiRest_LabWebApp.Services;

namespace ApiRest_LabWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly BdLabContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UsuariosController(BdLabContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // Login: Público
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            try
            {
                Console.WriteLine($"BODY request: {request.CorreoUsuario} - {request.Clave}");

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

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    Nombre = usuario.Nombre,
                    CorreoUsuario = usuario.CorreoUsuario,
                    Rol = usuario.Rol,
                    EsContraseñaTemporal = usuario.EsContraseñaTemporal ?? true,
                    IdUsuario = usuario.IdUsuario
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en login: " + ex.Message);
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        private string GenerarTokenJwt(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.CorreoUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("idUsuario", usuario.IdUsuario.ToString())
            };

            // Leer JWT desde variables de entorno
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

            if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
                throw new InvalidOperationException("JWT mal configurado. Verifica las variables de entorno.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Solo administrador puede ver todos los usuarios
        [HttpGet]
        [Authorize(Roles = "administrador")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // Solo administrador puede ver info de cualquier usuario
        [HttpGet("{id}")]
        [Authorize(Roles = "administrador")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            return usuario;
        }

        // Solo administrador puede crear usuario
        [HttpPost]
        [Authorize(Roles = "administrador")]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        // Solo administrador puede editar usuario
        [HttpPut("{id}")]
        [Authorize(Roles = "administrador")]
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

        // Solo administrador puede eliminar usuario
        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
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

        // Cualquier autenticado puede verificar su token
        [HttpGet("verificar-token")]
        [Authorize]
        public async Task<IActionResult> VerificarToken()
        {
            var correo = User.Identity?.Name?.Trim().ToLower();
            var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value?.Trim().ToLower();

            Console.WriteLine($"Correo del token: '{correo}'");
            Console.WriteLine($"Rol del token: '{rol}'");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.CorreoUsuario.Trim().ToLower() == correo &&
                u.Rol.Trim().ToLower() == rol &&
                u.EstadoRegistro == true);

            if (usuario == null)
            {
                Console.WriteLine("Token inválido o desactualizado: no coincide con base.");
                return BadRequest("Token inválido o desactualizado.");
            }

            Console.WriteLine("Token válido. Usuario confirmado: " + usuario.Nombre);
            return Ok("Token válido.");
        }

        private static string GenerarClaveTemporal()
        {
            return Guid.NewGuid().ToString("N")[..8]; // Ej: clave de 8 caracteres
        }

        // Solo administrador puede registrar usuario
        [HttpPost("registrar")]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] CrearUsuarioDto nuevo)
        {
            if (await _context.Usuarios.AnyAsync(u => u.CorreoUsuario.ToLower() == nuevo.CorreoUsuario.ToLower()))
                return BadRequest("El correo ya está registrado.");

            var claveTemporal = GenerarClaveTemporal();
            var claveHash = BCrypt.Net.BCrypt.HashPassword(claveTemporal);

            var usuario = new Usuario
            {
                Nombre = nuevo.Nombre,
                CorreoUsuario = nuevo.CorreoUsuario,
                ClaveUsuario = claveHash,
                Rol = nuevo.Rol,
                EstadoRegistro = true,
                EsContraseñaTemporal = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Aquí luego se enviará un correo real
            await _emailService.SendTemporaryPasswordEmailAsync(nuevo.CorreoUsuario, claveTemporal);

            return Ok("Usuario registrado correctamente.");
        }

        // Usuario autenticado puede cambiar su clave
        [HttpPut("cambiar-clave")]
        [Authorize]
        public async Task<IActionResult> CambiarClave([FromBody] CambiarClaveDto dto)
        {
            var correoToken = User.Identity?.Name?.Trim().ToLower();
            if (correoToken != dto.CorreoUsuario.Trim().ToLower())
                return Forbid("No puede cambiar la contraseña de otro usuario.");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoUsuario.ToLower() == dto.CorreoUsuario.ToLower());

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            usuario.ClaveUsuario = BCrypt.Net.BCrypt.HashPassword(dto.NuevaClave);
            usuario.EsContraseñaTemporal = false;

            await _context.SaveChangesAsync();

            return Ok("Contraseña actualizada.");
        }
    }
}
