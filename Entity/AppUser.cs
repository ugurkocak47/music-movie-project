using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Entity
{
    public class AppUser:IdentityUser<Guid>
    {
        [StringLength(250)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Surname { get; set; }
        public bool Gender { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
