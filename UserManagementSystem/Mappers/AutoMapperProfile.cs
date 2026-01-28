using AutoMapper;
using UserManagementSystem.DTOs;
using UserManagementSystem.Models.Identity;

namespace UserManagementSystem.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserResponseDto>();
            CreateMap<ApplicationUser, UpdateUserDto>().ReverseMap();

        }
    }
}
