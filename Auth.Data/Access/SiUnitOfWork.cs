using Auth.Data.Access.Base;
using Auth.Data.Context;
using Auth.Domain.Entities.Identity;
using System;
using System.Threading.Tasks;

namespace Auth.Data.Access
{
    public class SiUnitOfWork: IUnitOfWork, IDisposable
    {
        #region PRIVATE FIELDS

        private readonly IdentityContext _context;

        private BaseRepository<ApplicationUser> _userRepository;
        private BaseRepository<OrganisationAccount> _organisactionAccountRepository;

        private bool _disposed;

        #endregion


        #region Constructor    

        public SiUnitOfWork(IdentityContext contextInstance)
        {
            if (contextInstance == null)
            {
                throw new NullReferenceException("The specified context is null. Please review database initialization.");
            }

            _context = contextInstance;
        }

        #endregion


        #region Repository getters

        public IBaseRepository<ApplicationUser> ApplicationUser
        {
            get
            {
                return _userRepository ??
                       (_userRepository = new BaseRepository<ApplicationUser>(_context));
            }
        }

        public IBaseRepository<OrganisationAccount> OrganisationAccount
        {
            get
            {
                return _organisactionAccountRepository ??
                       (_organisactionAccountRepository = new BaseRepository<OrganisationAccount>(_context));
            }
        }

        #endregion


        #region Methods implementation

        public int Save()
        {   
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            var saveChangesAsync = await _context.SaveChangesAsync();
            return saveChangesAsync;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        #endregion
    }
}
