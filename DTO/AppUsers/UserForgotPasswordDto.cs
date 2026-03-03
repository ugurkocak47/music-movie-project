using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.AppUsers
{
    public class UserForgotPasswordDto
    {
        [Required(ErrorMessage = "Lütfen bir Email adresi giriniz.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = null!;
    }
}
