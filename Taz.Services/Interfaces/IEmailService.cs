using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taz.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string text);
    }
}
