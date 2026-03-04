using Core.Utilities.Results;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.AppRoles;



namespace Service.Abstracts
{
    public interface IRoleService
    {
        public Task<IDataResult<List<ListRoleDto>>> GetAllRolesAsync();
        public Task<IResult> CreateRoleAsync(CreateRoleDto roleDto);
        public Task<IDataResult<UpdateRoleDto>> GetCurrentRoleToUpdateAsync(Guid id);
        public Task<IResult> UpdateRoleAsync(UpdateRoleDto roleDto);
        public Task<IResult> DeleteRoleAsync(Guid id);
        public Task<IDataResult<List<AssignRoleToUserDto>>> GetUserRolesAsync(Guid id);
        public Task<IResult> AssignRoleToUserAsync(Guid userId, List<AssignRoleToUserDto> roleDto);
    }
}
