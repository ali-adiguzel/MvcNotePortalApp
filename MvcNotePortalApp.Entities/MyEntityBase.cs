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
    public class MyEntityBase

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Oluşturulma Tarihi"), Required,
            ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Düzenlenme Tarihi"), Required,
            ScaffoldColumn(false)]
        public DateTime ModifiedDate { get; set; }

        [DisplayName("Düzenleyen Kullanıcı"), Required, StringLength(30),
            ScaffoldColumn(false)]
        public string ModifiedUserName { get; set; }
    }
}
