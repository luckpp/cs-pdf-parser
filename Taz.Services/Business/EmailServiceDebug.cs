using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taz.Services.Interfaces;

namespace Taz.Services.Business
{
    public class EmailServiceDebug : IEmailService
    {
        public void SendEmail(string text)
        {
            Console.WriteLine($"[EmailServiceDebug] Sending email: [{text}]");
        }
    }
}
