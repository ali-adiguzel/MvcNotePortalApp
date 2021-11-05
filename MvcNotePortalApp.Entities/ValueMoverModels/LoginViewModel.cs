using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcNotePortalApp.Entities.ValueMoverModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta alanı doldurulmalıdır."),
         EmailAddress(ErrorMessage = "Lütfen geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen şifrenizi giriniz."), DataType(DataType.Password)]
        public string Password { get; set; }
    }
}