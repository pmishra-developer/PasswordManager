
using Configurator.Database;
using Configurator.Database.Entities;
using Configurator.Repositories.Contracts;

namespace Configurator.Repositories
{
    public class ApplicationRepository : BaseRepository<Application>, IApplicationRepository
    {
        private new readonly ConfiguratorContext _context;

        public ApplicationRepository(ConfiguratorContext context) : base(context)
        {
            _context = context;
        }
    }
}
