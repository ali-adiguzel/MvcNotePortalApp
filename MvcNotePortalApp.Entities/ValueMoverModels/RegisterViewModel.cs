using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcNotePortalApp.Entities.ValueMoverModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı doldurulmalıdır."),
         StringLength(25)]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-posta alanı doldurulmalıdır."),
         EmailAddress(ErrorMessage = "Lütfen geçerli bir email adresi giriniz."),
         StringLength(75, ErrorMessage = "E-posta çok uzun.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen şifrenizi giriniz."),
         DataType(DataType.Password),
         StringLength(30, ErrorMessage = "Şifre 30 karakterden uzun olamaz.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Lütfen şifre tekrarını giriniz."),
         DataType(DataType.Password),
         Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string RePassword { get; set; }
    }
}