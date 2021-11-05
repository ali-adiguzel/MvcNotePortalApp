using MvcNotePortalApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcNotePortalApp.DataAccessLayer.EntityFramework;

namespace MvcNotePortalApp.BusinessLayer
{
    public class Test
    {
        private Repository<NotePortalUser> repo_users = new Repository<NotePortalUser>();
        private Repository<Category> repo_categories = new Repository<Category>();
        private Repository<Comment> repo_comments = new Repository<Comment>();
        private Repository<Liked> repo_likes = new Repository<Liked>();
        private Repository<Note> repo_notes = new Repository<Note>();

        public Test()
        {
            //DataAccessLayer.DatabaseContext db = new DataAccessLayer.DatabaseContext();
            ////db.NotePortalUsers.ToList();          <-----

            ////db.Database.CreateIfNotExists();      <-----

            //db.Categories.ToList();
        }

        public void InsertTest()
        {
            repo_users.Insert(new NotePortalUser()
            {
                Name = "Dnm",
                Surname = "Test",
                Email = "xxx@yyy.com",
                UserName = "abc",
                Password = "xxx",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                CreatedDate = DateTime.Now.AddDays(-3),
                ModifiedDate = DateTime.Now.AddDays(-1),
                ModifiedUserName = "abc"
            });
        }

        public void UpdateTest()
        {
            NotePortalUser user = repo_users.Find(x => x.Name == "Dnm");

            if (user != null)
            {
                user.UserName = "asdasa";
                int result = repo_users.Update(user);
            }
        }

        public void DeleteTest()
        {
            NotePortalUser user = repo_users.Find(x => x.Name == "Dnm");

            if (user != null)
            {
                int result = repo_users.Delete(user);
            }
            
        }

        public void CommentTest()
        {
            NotePortalUser user = repo_users.Find(x => x.Id == 1);
            Note note = repo_notes.Find(x => x.Id == 3);

            Comment comment = new Comment()
            {
                Text = "Test yorumu",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedUserName = "Exalted",
                Note = note,
                Owner = user
            };

            repo_comments.Insert(comment);
        }
    }
}