using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MvcNotePortalApp.Entities;

namespace MvcNotePortalApp.DataAccessLayer.EntityFramework
{
    public class MyInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            NotePortalUser admin = new NotePortalUser()
            {
                Name = "Ali",
                Surname = "Adıgüzel",
                Email = "exalted305@gmail.com",
                Password = "12345678",
                ProfileImageFile = "default_user_pic.png",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                UserName = "Exalted",

                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now.AddMinutes(5),
                ModifiedUserName = "Exalted"
            };

            NotePortalUser standartUser = new NotePortalUser()
            {
                Name = "Mustafa",
                Surname = "Adıgüzel",
                Email = "ali.adiguzel@yahoo.com",
                Password = "123456",
                ProfileImageFile = "default_user_pic.png",
                IsActive = true,
                IsAdmin = false,
                UserName = "mistik",
                ActivateGuid = Guid.NewGuid(),

                CreatedDate = DateTime.Now.AddHours(1),
                ModifiedDate = DateTime.Now.AddMinutes(65),
                ModifiedUserName = "Exalted"
            };

            context.NotePortalUsers.Add(admin);
            context.NotePortalUsers.Add(standartUser);

            for (int i = 0; i < 8; i++)
            {
                NotePortalUser user = new NotePortalUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    Password = "123",
                    ProfileImageFile = "default_user_pic.png",
                    IsActive = true,
                    IsAdmin = false,
                    UserName = $"user{i}",
                    ActivateGuid = Guid.NewGuid(),

                    CreatedDate = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedDate = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUserName = $"user{i}"
                };

                context.NotePortalUsers.Add(user);
            }

            context.SaveChanges();

            List<NotePortalUser> userList = context.NotePortalUsers.ToList();

            for (int i = 0; i < 10; i++)
            {
                Category cat = new Category()
                {
                    Title = FakeData.PlaceData.GetCity(),
                    Description = FakeData.TextData.GetSentence(),

                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ModifiedUserName = "Exalted"
                };

                context.Categories.Add(cat);

                for (int k = 0; k < FakeData.NumberData.GetNumber(5, 10); k++)
                {
                    NotePortalUser owner = userList[FakeData.NumberData.GetNumber(0, userList.Count - 1)];

                    Note note = new Note()
                    {
                        Title = FakeData.NameData.GetCompanyName(),
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 15)),
                        Category = cat,
                        IsDraft = false,
                        LikeCount = FakeData.NumberData.GetNumber(0, 10),
                        Owner = owner,

                        CreatedDate = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedDate = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUserName = owner.UserName
                    };

                    cat.Notes.Add(note);

                    for (int j = 0; j < FakeData.NumberData.GetNumber(0, 10); j++)
                    {
                        NotePortalUser commentOwner = userList[FakeData.NumberData.GetNumber(0, userList.Count - 1)];

                        Comment comment = new Comment()
                        {
                            Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 3)),
                            Note = note,
                            Owner = commentOwner,

                            CreatedDate = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedDate = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUserName = commentOwner.UserName
                        };

                        note.Comments.Add(comment);
                    }


                    for (int m = 0; m < note.LikeCount; m++)
                    {
                        Liked liked = new Liked()
                        {
                            LikedUser = userList[m]
                        };

                        note.Likes.Add(liked);
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
