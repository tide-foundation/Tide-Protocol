using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tide.Ork.Classes
{
    public interface IEmailClient {
        bool SendEmail(string recipient, string recipientEmail, string subject, string content);
    }

    
}
