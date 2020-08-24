using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taz.Services.Interfaces;

namespace Taz.Services.Business
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string text)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("from@yahoo.com", "From"));
            mailMessage.To.Add(new MailboxAddress("to@yahoo.com", "TO"));
            mailMessage.Subject = "Test";
            mailMessage.Body = new TextPart("plain")
            {
                Text = text
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.mail.yahoo.com", 465, true);
                smtpClient.Authenticate("from", "from_password");
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }
    }
}
