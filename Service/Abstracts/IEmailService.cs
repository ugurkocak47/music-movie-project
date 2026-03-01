namespace Service.Abstracts
{
    public interface IEmailService
    {
        Task SendResetEmail(string resetEmailLink, string userEmail);
    }
}
