using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PasswordManager.Database.Entities;
using PasswordManager.ViewModel;

namespace PasswordManager.Application
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<UserViewModel, User>();
            CreateMap<User, UserViewModel>();
        }
    }
}
