using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServicesAbstractions;
using Shared.DTOs.Contact;

namespace Services
{
    public class ContactService(IConfiguration configuration, ILogger<ContactService> logger) : IContactService
    {
        public async Task<bool> SendContactMessageAsync(ContactUsDto model, string? userId = null)
        {
            try
            {
                var emailSettings = configuration.GetSection("MailSettings");
                var email = new MimeMessage();

             
                email.From.Add(new MailboxAddress("SmartFlow Support", emailSettings["Email"]));
                email.To.Add(MailboxAddress.Parse(emailSettings["Email"]));

                email.ReplyTo.Add(MailboxAddress.Parse(model.Email));

                email.Subject = $"New Contact Message from {model.Name}";

                var builder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <div style='font-family: Arial, sans-serif;'>
                            <h3 style='color: #2e6ca4;'>New Inquiry from SmartFlow App</h3>
                            <p><b>Name:</b> {model.Name}</p>
                            <p><b>Email:</b> {model.Email}</p>
                            <p><b>Message:</b><br/>{model.Message}</p>
                            <hr/>
                            <p style='font-size: 0.8em; color: gray;'>User ID: {userId ?? "Guest User"}</p>
                        </div>"
                };

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(emailSettings["Host"], int.Parse(emailSettings["Port"]!), SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(emailSettings["Email"], emailSettings["Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
             
                logger.LogError(ex, "Failed to send email inquiry from {Email}", model.Email);
                return false;
            }
        }
    }
}