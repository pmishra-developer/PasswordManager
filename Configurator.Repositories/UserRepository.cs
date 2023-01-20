using Configurator.Database;
using Configurator.Database.Entities;
using Configurator.Repositories.Contracts;

namespace Configurator.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private new readonly ConfiguratorContext _context;

        public UserRepository(ConfiguratorContext context) : base(context)
        {
            _context = context;
        }
    }
}
