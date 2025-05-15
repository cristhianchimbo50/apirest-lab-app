using System.Text.Json.Serialization;

namespace ApiRest_LabWebApp.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public string CorreoUsuario { get; set; } // ❗ Necesario si usas como ClaimTypes.Name
        public bool EsContraseñaTemporal { get; set; }
    }




}
