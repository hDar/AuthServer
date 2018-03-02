using Auth.Data.Access.Base;
using Auth.Domain.Entities.Identity;

namespace Auth.Data.Access
{
    public interface IUnitOfWork
    {

        IBaseRepository<ApplicationUser> ApplicationUser { get; }
        IBaseRepository<OrganisationAccount> OrganisationAccount { get; }
    }
}
