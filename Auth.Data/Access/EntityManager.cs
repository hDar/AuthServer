using Auth.Data.Context;
using Auth.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Data.Access
{
    public class EntityManager
    {
        private readonly IdentityContext _context;

        public EntityManager(IdentityContext context) {
            _context = context;
        }

        public async Task<ApplicationUser> GetById(String userId) {
            var user = _context.Users
                            .Include(orgAccount => orgAccount.OrganisationAccount);

            return await user.FirstOrDefaultAsync(u => u.Id==userId);
                            
        }
    }
}
