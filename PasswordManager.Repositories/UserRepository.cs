using PasswordManager.Database;
using PasswordManager.Database.Entities;
using PasswordManager.Repositories.Contracts;

namespace PasswordManager.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private new readonly PasswordManagerContext _context;

        public UserRepository(PasswordManagerContext context) : base(context)
        {
            _context = context;
        }
    }
}
