using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_EmailService
{
    public class EmailService : IEmailService
    {
        public bool SendEmail(
            string toEmail,
            string subject,
            string body)
        {
            // Gmail implementation will be added here
            return false;
        }
    }
}
