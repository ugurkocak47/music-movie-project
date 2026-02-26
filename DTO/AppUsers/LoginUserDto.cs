using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.AppUsers
{
    public class LoginUserDto
    {
        public LoginUserDto()
        {
            
        }
        public LoginUserDto(string email, string password)
        {
            Email = email;
            Password = password;
        }
        [Required(ErrorMessage = "Lütfen bir Email adresi giriniz.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
        [Display(Name = "Şifre")]
        [PasswordPropertyText(true)]
        public string Password { get; set; } = null!;


        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
