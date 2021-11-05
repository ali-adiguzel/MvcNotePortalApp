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
    [Table("Notes")]
    public class Note : MyEntityBase
    {
        [DisplayName("Not Başlığı"), Required, StringLength(75)]
        public string Title { get; set; }

        [DisplayName("Not"), Required, StringLength(2500)]
        public string Text { get; set; }

        [DisplayName("Taslak mı?")]
        public bool IsDraft { get; set; }

        [DisplayName("Beğenilme"), ScaffoldColumn(false)]
        public int LikeCount { get; set; }

        [DisplayName("Kategori")]
        public int CategoryId { get; set; }
        public virtual NotePortalUser Owner { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual Category Category { get; set; }
        public virtual List<Liked> Likes { get; set; }

        public Note()
        {
            Comments = new List<Comment>();
            Likes = new List<Liked>();
        }
    }
}
