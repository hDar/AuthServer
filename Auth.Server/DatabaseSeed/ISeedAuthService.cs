using System;
using System.Threading.Tasks;

namespace Auth.Server.DatabaseSeed
{
    public interface ISeedAuthService
    {
        Task SeedAuthDatabase(IServiceProvider serviceProvider);
    }
}