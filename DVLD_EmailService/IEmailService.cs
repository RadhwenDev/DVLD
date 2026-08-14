using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_EmailService
{
    public interface IEmailService
    {
        bool SendEmail(
            string toEmail,
            string subject,
            string body);
    }
}
