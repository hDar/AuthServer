using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.Entities.Identity
{
    public class OrganisationAccount : BaseEntity
    {       
        public string Name { get; set; }
        public string Region { get; set; }

        public string Data { get; set; }
        //public virtual IEnumerable<OrgnisationServices> Services { get; set; }
        public virtual IEnumerable<ApplicationUser> Users { get; set; }

    }



    //public class OrgnisationServices : BaseEntity
    //{        
    //    public string OrganisationAccountId { get; set; }
    //    public string ServiceId { get; set; }

    //    public Boolean isActive { get; set; }
    //}
}
