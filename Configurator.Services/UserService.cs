using AutoMapper;
using Configurator.Database.Entities;
using Configurator.Repositories.Contracts;
using Configurator.Services.Contracts;
using Configurator.ViewModel;

namespace Configurator.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserViewModel>> GetUsersAsync()
        {
            var allUsers = await _userRepository.GetAllAsync();
            return _mapper.Map<List<UserViewModel>>(allUsers);
        }

        public async Task<UserViewModel> GetUserAsync(int id)
        {
            var result = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserViewModel>(result);
        }

        public async Task<int> AddUserAsync(UserViewModel user)
        {
            var newUser = _mapper.Map<UserViewModel, User>(user);
            return await _userRepository.AddAsync(newUser);
        }

        public async Task UpdateUserAsync(UserViewModel user)
        {
            var newUser = _mapper.Map<UserViewModel, User>(user);
            await _userRepository.UpdateAsync(newUser, newUser.Id);
        }

        public async Task DeleteUserAsync(int id)
        {
            var result = await _userRepository.GetByIdAsync(id);
            await _userRepository.RemoveAsync(result, result.Id);
        }
    }
}
