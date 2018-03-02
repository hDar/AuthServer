using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.Configuration
{
    public class DomainSettings
    {
        public WebResource Client { get; set; }
        public WebResource Api { get; set; }
        public WebResource Auth { get; set; }
    }
}
