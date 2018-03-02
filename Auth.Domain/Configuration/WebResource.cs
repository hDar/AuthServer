using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.Configuration
{
    public class WebResource
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Secret { get; set; }
        public string Url { get; set; }
    }
}
