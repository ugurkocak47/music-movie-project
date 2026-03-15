using Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.AppUsers
{
    public class EditUserDto
    {

        [Required(ErrorMessage = "Kullanıcı Ad alanı boş bırakılamaz.")]
        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Gerçek bir email adresi giriniz.")]
        [Required(ErrorMessage = "E-Posta alanı boş bırakılamaz.")]
        [Display(Name = "E-Posta :")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefon Numarası alanı boş bırakılamaz.")]
        [Display(Name = "Telefon Numarası :")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "Cinsiyet :")]
        public bool Gender { get; set; }
    }
}
