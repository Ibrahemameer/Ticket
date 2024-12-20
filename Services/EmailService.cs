using System.Net.Mail;
using System.Net;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.StaticFiles;

namespace RhinoTicketingSystem.Services
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            _senderEmail = "rhinoticket@gmail.com";
            _senderName = "RhinoTicketing System";

            _smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(_senderEmail, configuration["EmailSettings:AppPassword"])
            };
        }

        public async Task SendEmailAsync(string to, string cc, string subject, string body, List<Attachment> attachments)
        {
            try
            {
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("rhinoticket@gmail.com", "RhinoTicketing System");
                    mailMessage.To.Add(to);

                    if (!string.IsNullOrEmpty(cc))
                    {
                        mailMessage.CC.Add(cc);
                    }

                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            mailMessage.Attachments.Add(attachment);
                        }
                    }

                    await _smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }


    }

}
