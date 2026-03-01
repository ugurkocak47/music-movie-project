using Service.Abstracts;
using System.Net;
using System.Net.Mail;

namespace Service.Concretes
{
    public class EmailService : IEmailService
    {


        public async Task SendResetEmail(string resetEmailLink, string userEmail)
        {
            var smtpClient = new SmtpClient();

            smtpClient.Host = "smtp.gmail.com";
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("muhammedugr16@gmail.com", "kmauhgziflqtcvnv");
            smtpClient.EnableSsl = true;
            

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("muhammedugr16@gmail.com");
            mailMessage.To.Add(userEmail);

            mailMessage.Subject = "LocalHost | Reset Password";
            mailMessage.Body = @$"<h4>Şifrenizi yenilemek için aşağıdaki linke tıklayınız.</h4>
               <p><a href='{resetEmailLink}'>Şifre sıfırlama linki</a></p>";

            mailMessage.IsBodyHtml = true;


            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
