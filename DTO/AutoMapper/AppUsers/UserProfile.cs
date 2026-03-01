using AutoMapper;
using Dto.AppUsers;
using Dto.AppRoles;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.AutoMapper.AppUsers
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterUserDto, AppUser>().ReverseMap();
            CreateMap<ListUserDto, AppUser>().ReverseMap();
            CreateMap<EditUserDto, AppUser>().ReverseMap();
        }
    }
}
