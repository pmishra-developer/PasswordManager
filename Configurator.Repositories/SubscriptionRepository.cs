using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configurator.Database;
using Configurator.Database.Entities;
using Configurator.Repositories.Contracts;

namespace Configurator.Repositories
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        private new readonly ConfiguratorContext _context;

        public SubscriptionRepository(ConfiguratorContext context) : base(context)
        {
            _context = context;
        }
    }
}
