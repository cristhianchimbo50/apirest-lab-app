namespace ApiRest_LabWebApp.Services
{
    public interface IEmailService
    {
        Task SendTemporaryPasswordEmailAsync(string toEmail, string temporaryPassword);
    }

}
