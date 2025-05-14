namespace ApiRest_LabWebApp.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public bool EsContraseñaTemporal { get; set; }
    }

}
