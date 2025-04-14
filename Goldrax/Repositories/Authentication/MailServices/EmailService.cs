using Goldrax.Models.Authentication.MailServiceModels;
using MailKit.Net.Smtp;
using MimeKit;


namespace Goldrax.Repositories.Authentication.MailServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail (Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            //{
            //    Text = message.Content
            //};
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.Content,
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        //sending message
        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            catch { throw; }
            finally {
                client.Disconnect(true);
                client.Dispose();
            }
        }


    }
}
