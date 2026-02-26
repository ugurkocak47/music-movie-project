using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.AppUsers
{
    public class UserResetPasswordDto
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre : ")]
        public string Password { get; set; } = null!;





        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmemektedir.")]
        [Required(ErrorMessage = "Şifre Tekrar alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre Tekrar : ")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
