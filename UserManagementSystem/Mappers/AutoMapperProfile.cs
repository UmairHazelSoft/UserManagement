using AutoMapper;
using UserManagementSystem.DTOs;
using UserManagementSystem.Models.Identity;

namespace UserManagementSystem.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUserDto, User>().ReverseMap();
            CreateMap<UpdateUserDto, User>().ReverseMap();
            CreateMap<User, UserResponseDto>().ReverseMap();
            CreateMap<UserReadDto, UserResponseDto>().ReverseMap();
            CreateMap<User, UserReadDto>().ReverseMap();
            CreateMap<ApplicationUser, UserResponseDto>();
            CreateMap<ApplicationUser, UpdateUserDto>().ReverseMap();


        }
    }
}
