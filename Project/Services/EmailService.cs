using System.Net;
using System.Net.Mail;

namespace Project.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendServiceRequestConfirmationAsync(
            string toEmail,
            string clientName,
            Guid requestId,
            string serviceType,
            DateTime createdOn)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@bidmotors.com";
                var fromPassword = _configuration["Email:FromPassword"] ?? "";

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(fromEmail, fromPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "BIDMOTORS"),
                    Subject = $"Потвърждение за заявка #{requestId.ToString().Substring(0, 8)}",
                    Body = GetEmailBody(clientName, requestId, serviceType, createdOn),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                // Не хвърляме грешка, за да не спира процеса на създаване на заявката
            }
        }

        private string GetEmailBody(string clientName, Guid requestId, string serviceType, DateTime createdOn)
        {
            var shortId = requestId.ToString().Substring(0, 8).ToUpper();

            return @"<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #1a1a1a; color: #F6D201; padding: 20px; text-align: center; }
        .content { background: #f9f9f9; padding: 30px; border: 1px solid #ddd; }
        .info-row { margin: 15px 0; padding: 10px; background: white; border-left: 4px solid #F6D201; }
        .label { font-weight: bold; color: #555; }
        .footer { text-align: center; margin-top: 20px; padding: 20px; color: #777; font-size: 12px; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>BIDMOTORS</h1>
            <p>Професионален автосервиз и диагностика</p>
        </div>
        
        <div class='content'>
            <h2>Здравейте, " + clientName + @"!</h2>
            <p>Благодарим Ви за доверието! Вашата заявка е приета успешно.</p>
            
            <div class='info-row'>
                <span class='label'>Номер на заявка:</span> #" + shortId + @"
            </div>
            
            <div class='info-row'>
                <span class='label'>Вид услуга:</span> " + serviceType + @"
            </div>
            
            <div class='info-row'>
                <span class='label'>Дата на заявка:</span> " + createdOn.ToString("dd.MM.yyyy HH:mm") + @"
            </div>
            
            <div class='info-row'>
                <span class='label'>Статус:</span> В изчакване
            </div>
            
            <p style='margin-top: 30px;'>
                Нашият екип ще прегледа заявката Ви възможно най-скоро и ще се свържем с Вас на посочения телефон.
            </p>
            
            <p style='color: #666; font-size: 14px;'>
                <strong>Важно:</strong> Можете да проследите статуса на заявката си по всяко време от секция Моите заявки на нашия сайт.
            </p>
        </div>
        
        <div class='footer'>
            <p>BIDMOTORS - Вашият доверен автосервиз</p>
            <p>Този имейл е изпратен автоматично. Моля, не отговаряйте на него.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
