using Configurator.Database;
using Configurator.Database.Entities;
using Configurator.Repositories.Contracts;

namespace Configurator.Repositories
{
    public class DeviceRepository : BaseRepository<Device>, IDeviceRepository
    {
        private new readonly ConfiguratorContext _context;

        public DeviceRepository(ConfiguratorContext context) : base(context)
        {
            _context = context;
        }
    }
}
