﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.AppRoles
{
    public class UpdateRoleDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Rol adı boş bırakılamaz.")]
        [Display(Name = "Rol adı: ")]
        public string Name { get; set; } = null!;
    }
}
