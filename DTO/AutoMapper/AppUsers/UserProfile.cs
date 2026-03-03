using AutoMapper;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.AppUsers;

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
