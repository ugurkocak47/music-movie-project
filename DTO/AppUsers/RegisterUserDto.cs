using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.AppUsers
{
    public class RegisterUserDto
    {
        public RegisterUserDto()
        {
            
        }

        public RegisterUserDto(string name, string surname, string userName, string email, string phoneNumber, string password, bool gender)
        {
            Name = name;
            Surname = surname;
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Gender = gender;
        }
        [Required(ErrorMessage = "İsim alanı boş bırakılamaz!")]
        [Display(Name ="*İsim: ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyisim alanı boş bırakılamaz!")]
        [Display(Name = "*Soyisim: ")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Kullanıcı adı alanı boş bırakılamaz!")]
        [Display(Name = "*Kullanıcı Adı: ")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
        [Display(Name = "*Email: ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Telefon numarası alanı boş bırakılamaz!")]
        [Display(Name = "*Telefon Numarası: ")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        [Display(Name = "*Şifre: ")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter içermek zorundadır!")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifreler uyuşmamaktadır!")]
        [Required(ErrorMessage = "Şifre tekrarı alanı boş bırakılamaz!")]
        [Display(Name = "*Şifre Tekrarı: ")]
        public string PasswordConfirm { get; set; }
        [Required(ErrorMessage = "Cinsiyet alanı boş bırakılamaz!")]   
        [Display(Name="*Cinsiyet: ")]
        public bool Gender { get; set; }
    }
}
