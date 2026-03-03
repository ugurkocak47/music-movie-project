using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.AppRoles
{
    public class AssignRoleToUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsAssigned { get; set; }
    }
}
