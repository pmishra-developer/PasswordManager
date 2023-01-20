using Configurator.Database;
using Configurator.Database.Entities;
using Configurator.Repositories.Contracts;

namespace Configurator.Repositories
{
    public class LookupRepository : BaseRepository<LookupData>, ILookupRepository
    {
        private new readonly ConfiguratorContext _context;

        public LookupRepository(ConfiguratorContext context) : base(context)
        {
            _context = context;
        }
    }
}
