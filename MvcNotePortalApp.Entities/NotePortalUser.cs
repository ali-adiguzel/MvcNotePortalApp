using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcNotePortalApp.Entities
{
    [Table("NotePortalUsers")]
    public class NotePortalUser : MyEntityBase
    {
        [DisplayName("İsim"), StringLength(30, ErrorMessage = "{0} en fazla {1} karakter uzunluğunda olmalıdır.")]
        public string Name { get; set; }

        [DisplayName("Soyisim"), StringLength(50, ErrorMessage = "{0} en fazla {1} karakter uzunluğunda olmalıdır.")]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı adı"), Required, StringLength(25, ErrorMessage = "{0} en fazla {1} karakter uzunluğunda olmalıdır.")]
        public string UserName { get; set; }

        [DisplayName("E-posta"), Required, StringLength(75, ErrorMessage = "{0} en fazla {1} karakter uzunluğunda olmalıdır.")]
        public string Email { get; set; }

        [DisplayName("Şifre"), Required, StringLength(30, ErrorMessage = "{0} en fazla {1} karakter uzunluğunda olmalıdır.")]
        public string Password { get; set; }

        [DisplayName("Profil Resmi"), StringLength(50, ErrorMessage = "{0} en fazla {1} karakter uzunluğunda olmalıdır."), ScaffoldColumn(false)]
        public string ProfileImageFile { get; set; }

        [DisplayName("Aktif mi?")]
        public bool IsActive { get; set; }

        [DisplayName("Admin mi?")]
        public bool IsAdmin { get; set; }

        [Required, ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }

        public virtual List<Note> Notes { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }
    }
}
