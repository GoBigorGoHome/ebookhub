using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ebookhub.Data
{
    public class TokenOptions
    {
        public string SiteUrl { get; set; }
        public string Key { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int ExpiryMinutes { get; set; }
        public string TokenType { get; set; }
    }
}
