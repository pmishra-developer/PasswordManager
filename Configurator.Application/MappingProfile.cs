using AutoMapper;
using Configurator.Database.Entities;
using Configurator.ViewModel;

namespace Configurator.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<Device, DeviceViewModel>().ReverseMap();
            CreateMap<Subscription, SubscriptionViewModel>().ReverseMap();
        }
    }
}
