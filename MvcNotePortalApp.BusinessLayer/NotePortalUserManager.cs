using MvcNotePortalApp.BusinessLayer.Abstract;
using MvcNotePortalApp.BusinessLayer.Result;
using MvcNotePortalApp.Common.Helpers;
using MvcNotePortalApp.DataAccessLayer.EntityFramework;
using MvcNotePortalApp.Entities;
using MvcNotePortalApp.Entities.Messages;
using MvcNotePortalApp.Entities.ValueMoverModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcNotePortalApp.BusinessLayer
{
    public class NotePortalUserManager : ManagerBase<NotePortalUser>
    {
        private Repository<Liked> repo_like = new Repository<Liked>();
        private Repository<Comment> repo_comment = new Repository<Comment>();
        private Repository<Note> repo_note = new Repository<Note>();
        private Repository<Category> repo_category = new Repository<Category>();

        public BusinessLayerResult<NotePortalUser> RegisterUser(RegisterViewModel model)
        {
            #region <ALTERNATIVE>
            //bool IsExist = false;
            //foreach (NotePortalUser user in userList)
            //{
            //    if (user.UserName == model.UserName)
            //    {
            //        IsExist = true;
            //    }

            //    if (user.Email == model.Email)
            //    {
            //        IsExist = true;
            //    }
            //}
            // daha sonra true ya da false olmasına göre hata veya kayıt...
            #endregion

            List<NotePortalUser> userList = List();
            BusinessLayerResult<NotePortalUser> blresult = new BusinessLayerResult<NotePortalUser>();

            NotePortalUser NpUser = Find(x => x.UserName == model.Username || x.Email == model.Email);

            if (NpUser != null)
            {
                if (NpUser.UserName == model.Username)
                {
                    blresult.AddError(ErrorMessage.UsernameAlreadyExists,
                        "Kullanıcı adı zaten kayıtlı.");
                }
                if (NpUser.Email == model.Email)
                {
                    blresult.AddError(ErrorMessage.EmailAlreadyExists,
                        "E-posta zaten kayıtlı.");
                }
            }
            else
            {
                NotePortalUser newUser = new NotePortalUser()
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    ActivateGuid = Guid.NewGuid(),
                    ProfileImageFile = "default_user_pic.png",
                    IsActive = false,
                    IsAdmin = false
                };

                int insertResult = base.Insert(newUser);

                if (insertResult > 0)
                {
                    blresult.Result = Find(x => x.Email == model.Email && x.UserName == model.Username);

                    string siteUrl = ConfigHelper.Get<string>("SiteUrl");
                    string activateUrl = $"{siteUrl}/Home/ActivateUser/{newUser.ActivateGuid}";
                    string body = $"Merhaba {newUser.UserName}, NotePortal'a hoşgeldiniz. Hesabınınızı aktifleşirmek için <a href='{activateUrl}' target='_blank'>tıklayınız</a>.";

                    MailHelper.SendMail(body, newUser.Email, "NotePortal E-posta Aktivasyonu");
                }
            }

            return blresult;
        }

        public BusinessLayerResult<NotePortalUser> LoginUser(LoginViewModel model)
        {
            List<NotePortalUser> userList = List();
            BusinessLayerResult<NotePortalUser> blresult = new BusinessLayerResult<NotePortalUser>();

            NotePortalUser NpUser = Find(x => x.Email == model.Email && x.Password == model.Password);

            if (NpUser != null)
            {
                if (NpUser.IsActive)
                {
                    blresult.Result = NpUser;
                }
                else
                {
                    blresult.AddError(ErrorMessage.UserIsNotActive,
                        "Hesabınız aktif edilmemiş. Lütfen e-postanıza gelen aktivasyon mailindeki linke tıklayarak hesabınızı aktifleştiriniz.");
                }
            }
            else
            {
                blresult.AddError(ErrorMessage.UsernameOrPasswordWrong,
                    "Kullanıcı adı ya da şifre hatalı.");
            }

            return blresult;
        }

        public BusinessLayerResult<NotePortalUser> ActivateUser(Guid guid)
        {
            BusinessLayerResult<NotePortalUser> blresult = new BusinessLayerResult<NotePortalUser>();

            blresult.Result = Find(x => x.ActivateGuid == guid);

            if (blresult.Result != null)
            {
                if (blresult.Result.IsActive)
                {
                    blresult.AddError(ErrorMessage.UserAlreadyActive, "Hesabınız zaten aktif edilmiş...");

                    return blresult;
                }

                blresult.Result.IsActive = true;

                Update(blresult.Result);
            }
            else
            {
                blresult.AddError(ErrorMessage.ActivateCodeInvalid, "Yanlış aktivasyonu kodu, hesabınız aktifleştirilemedi.");
            }

            return blresult;
        }

        public BusinessLayerResult<NotePortalUser> UpdateUser(NotePortalUser updatedUser)
        {
            BusinessLayerResult<NotePortalUser> blresult = new BusinessLayerResult<NotePortalUser>();

            blresult.Result = Find(x => x.Id == updatedUser.Id);

            if (blresult.Result == null)
            {
                blresult.Errors.Add(new ErrorMessageObject { Code = ErrorMessage.UserNotFound, Message = "Kullanıcı bulunamadı." });
            }

            blresult.Result.Name = updatedUser.Name;
            blresult.Result.Surname = updatedUser.Surname;
            blresult.Result.Password = updatedUser.Password;

            if (string.IsNullOrEmpty(updatedUser.ProfileImageFile) == false)
            {
                blresult.Result.ProfileImageFile = updatedUser.ProfileImageFile;
            }

            if (base.Update(blresult.Result) == 0)
            {
                blresult.Errors.Add(new ErrorMessageObject { Code = ErrorMessage.ProfileCouldNotUpdated, Message = "Kullanıcı güncellenemedi." });
            }

            return blresult;
        }

        public BusinessLayerResult<NotePortalUser> RemoveUserById(int id)
        {
            BusinessLayerResult<NotePortalUser> blresult = new BusinessLayerResult<NotePortalUser>();

            NotePortalUser user = Find(x => x.Id == id);

            if (user != null)
            {
                List<Note> userNotes = repo_note.List(x => x.Owner.Id == id);
                foreach (Note note in userNotes)
                {
                    repo_note.Delete(note);
                }

                List<Liked> userLikes = repo_like.List(x => x.LikedUser.Id == id);
                foreach (Liked like in userLikes)
                {
                    repo_like.Delete(like);
                }

                List<Comment> userComments = repo_comment.List(x => x.Owner.Id == id);
                foreach (Comment comment in userComments)
                {
                    repo_comment.Delete(comment);
                }

                if (Delete(user) == 0)
                {
                    blresult.Errors.Add(new ErrorMessageObject { Code = ErrorMessage.UserCouldNotRemove, Message = "Kullanıcı silinemedi." });

                    return blresult;
                }
            }
            else
            {
                blresult.AddError(ErrorMessage.UserNotFound, "Kullanıcı bulunamadı.");
            }

            return blresult;
        }

        public BusinessLayerResult<NotePortalUser> GetUserById(int id)
        {
            BusinessLayerResult<NotePortalUser> blresult = new BusinessLayerResult<NotePortalUser>();

            blresult.Result = Find(x => x.Id == id);

            if (blresult.Result == null)
            {
                blresult.AddError(ErrorMessage.UserNotFound, "Kullanıcı bulunamadı.");
            }

            return blresult;
        }

        public new BusinessLayerResult<NotePortalUser> Insert(NotePortalUser model)
        {
            List<NotePortalUser> userList = List();
            BusinessLayerResult<NotePortalUser> blresult = new BusinessLayerResult<NotePortalUser>();

            NotePortalUser NpUser = Find(x => x.UserName == model.UserName || x.Email == model.Email);

            blresult.Result = model;

            if (NpUser != null)
            {
                if (NpUser.UserName == model.UserName)
                {
                    blresult.AddError(ErrorMessage.UsernameAlreadyExists,
                        "Kullanıcı adı zaten kayıtlı.");
                }
                if (NpUser.Email == model.Email)
                {
                    blresult.AddError(ErrorMessage.EmailAlreadyExists,
                        "E-posta zaten kayıtlı.");
                }
            }
            else
            {
                blresult.Result.ProfileImageFile = "default_user_pic.png";
                blresult.Result.ActivateGuid = Guid.NewGuid();

                NotePortalUser newUser = new NotePortalUser();

                int insertResult = base.Insert(blresult.Result);

                if (insertResult == 0)
                {
                    blresult.AddError(ErrorMessage.UserCouldNotInserted,
                        "Kullanıcı kayıt edilemedi.");
                }
            }

            return blresult;
        }

        public new BusinessLayerResult<NotePortalUser> Update(NotePortalUser updatedUser)
        {
            NotePortalUser db_user = Find(x => x.UserName == updatedUser.UserName || x.Email == updatedUser.Email);
            BusinessLayerResult<NotePortalUser> res = new BusinessLayerResult<NotePortalUser>();
            
            res.Result = updatedUser;

            if (db_user != null && db_user.Id != updatedUser.Id)
            {
                if (db_user.UserName == updatedUser.UserName)
                {
                    res.AddError(ErrorMessage.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == updatedUser.Email)
                {
                    res.AddError(ErrorMessage.EmailAlreadyExists, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == updatedUser.Id);

            res.Result.Email = updatedUser.Email;
            res.Result.Name = updatedUser.Name;
            res.Result.Surname = updatedUser.Surname;
            res.Result.Password = updatedUser.Password;
            res.Result.UserName = updatedUser.UserName;
            res.Result.IsActive = updatedUser.IsActive;
            res.Result.IsAdmin = updatedUser.IsAdmin;

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessage.ProfileCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }

            return res;
        }
    }
}