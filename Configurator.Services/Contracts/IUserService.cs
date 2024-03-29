﻿using Configurator.ViewModel;

namespace Configurator.Services.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetUsersAsync();
        Task<UserViewModel> GetUserAsync(int id);
        Task<int> AddUserAsync(UserViewModel user);
        Task UpdateUserAsync(UserViewModel user);
        Task DeleteUserAsync(int id);
    }
}
