using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ebookhub.Controllers
{
    public class SmtpOptions
    {
        public string FromAddress { get; set; }
        public string FromAddressTitle { get; set; }
        public string ToAddress { get; set; }
        public string ToAdressTitle { get; set; }
        public string SubjectLine { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpServerPort { get; set; }
        public string Password { get; set; }
        public string Certificate { get; set; }
        public string CertificatePassword { get; set; }
        public string ServiceAccountMail { get; set; }
    }
}
