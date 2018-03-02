using Microsoft.AspNetCore.Identity;
using System;

namespace Auth.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {        
        public string Civility { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ApiKey { get; set; }

        public bool IsActive { get; set; }
        public DateTime LastLoggedOn { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        public bool IsRootUser { get; set; }
        public bool IsPasswordTemporary { get; set; }

        public string OrganisationId { get; set; }   
        
        public virtual OrganisationAccount OrganisationAccount { get; set; }
    }
}
