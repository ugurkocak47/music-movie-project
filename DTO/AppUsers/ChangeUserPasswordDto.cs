using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.AppUsers
{
    public class ChangeUserPasswordDto
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Eski şifre alanı boş bırakılamaz.")]
        [Display(Name = "Eski Şifre : ")]
        public string PasswordOld { get; set; } = null;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni şifre alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre : ")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string PasswordNew { get; set; } = null;

        [DataType(DataType.Password)]
        [Compare(nameof(PasswordNew), ErrorMessage = "Şifreler aynı değil.")]
        [Required(ErrorMessage = "Yeni şifre tekrar alanı boş bırakılamaz.")]
        [Display(Name = "Yeni Şifre Tekrar : ")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string PasswordNewConfirm { get; set; } = null;
    }
}
