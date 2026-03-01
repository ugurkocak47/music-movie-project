using AutoMapper;
using Dto.AppRoles;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.AutoMapper.AppRoles
{
    public class RoleProfile:Profile
    {
        public RoleProfile()
        {
            CreateMap<CreateRoleDto, AppRole>().ReverseMap();
            CreateMap<UpdateRoleDto, AppRole>().ReverseMap();
            CreateMap<ListRoleDto, AppRole>().ReverseMap();
            CreateMap<AssignRoleToUserDto, AppRole>().ReverseMap();
        }
    }
}
