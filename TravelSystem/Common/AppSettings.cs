using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Common
{
    public class AppSettings
    {
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmailAddress { get; set; }

    }
}
