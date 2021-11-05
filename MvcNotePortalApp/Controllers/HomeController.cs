using MvcNotePortalApp.BusinessLayer;
using MvcNotePortalApp.BusinessLayer.Result;
using MvcNotePortalApp.Entities;
using MvcNotePortalApp.Entities.Messages;
using MvcNotePortalApp.Entities.ValueMoverModels;
using MvcNotePortalApp.Filters;
using MvcNotePortalApp.Models;
using MvcNotePortalApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MvcNotePortalApp.Controllers
{
    [Excptn]
    public class HomeController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        CategoryManager categoryManager = new CategoryManager();
        private NotePortalUserManager npum = new NotePortalUserManager();

        // GET: Home & LastNotes
        public ActionResult Index(int? id)
        {
            #region Test codes
            BusinessLayer.Test test = new BusinessLayer.Test();

            //test.InsertTest();
            //test.UpdateTest();
            //test.DeleteTest();

            //test.CommentTest();
            #endregion

            //---------------------------------------------------------

            if (id == null)
            {
                List<Note> notes = noteManager.List(x => x.IsDraft == false);

                return View(notes);
            }
            else
            {
                Category cat = categoryManager.Find(x => x.Id == id);

                if (cat == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.catId = id;

                List<Note> filteredNotes = noteManager.List(x => x.Category.Id == id && x.IsDraft == false);

                return View(filteredNotes.OrderByDescending(x => x.CreatedDate).ToList());
            }
        }

        public ActionResult LastNotes()
        {
            List<Note> notes = noteManager.List();

            ViewBag.PageStat = "LastNotes";

            return View("Index", notes.OrderByDescending(x => x.CreatedDate).ToList());
        }

        public ActionResult MostLikeds()
        {
            ViewBag.PageStat = "MostLikes";

            List<Note> notes = noteManager.List();

            return View("Index", notes.OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            ViewBag.PageStat = "About";

            return View();
        }

        [Auth]
        public ActionResult MyNotes(int id)
        {
            ViewBag.PageStat = "MyNotes";

            List<Note> notes = noteManager.List();

            return View("Index", notes.FindAll(x => x.Owner.Id == id));
        }

        [Auth]
        public ActionResult ProfilePage()
        {
            NotePortalUser currentUser = new NotePortalUser();

            currentUser = CurrentSession.User;

            if (currentUser == null)
            {
                ErrorViewModel errModel = new ErrorViewModel()
                {
                    Title = "Kullanıcı bulunamadı.",
                    Items = { new ErrorMessageObject { Message = "Lütfen giriş yapınız." } }
                };

                return View("Error", errModel);
            }
            else
            {
                BusinessLayerResult<NotePortalUser> userResult = npum.GetUserById(currentUser.Id);

                if (userResult.Errors.Count > 0)
                {
                    ErrorViewModel errModel = new ErrorViewModel()
                    {
                        Title = "Hata oluştu!",
                        Items = userResult.Errors
                    };

                    return View("Error", errModel);
                }

                return View(userResult.Result);
            }
        }

        [Auth]
        public ActionResult EditProfile()
        {
            NotePortalUser user = CurrentSession.User as NotePortalUser;

            return View(user);
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(NotePortalUser user, HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("Email");
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("Username");

            if (ModelState.IsValid)
            {
                // dosya null değilse ve formatı jpg, png veya jpeg ise..
                if (ProfileImage.ContentType != null &&
                    (ProfileImage.ContentType == "image/jpg" ||
                     ProfileImage.ContentType == "image/jpeg" ||
                     ProfileImage.ContentType == "image/png"))
                {
                    // dosyamızı bir formata göre isimlendirelim;
                    string filename = $"user_{user.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    // user_3.jpg tarzı bir şey olacak

                    // dosyayı yolumuza kaydedelim;
                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));

                    // dosyayı kullanıcının prop'una kaydedelim
                    user.ProfileImageFile = filename;
                }

                BusinessLayerResult<NotePortalUser> blresult = npum.UpdateUser(user);

                if (blresult.Errors.Count > 0)
                {
                    ErrorViewModel errModel = new ErrorViewModel()
                    {
                        Title = "Kullanıcı güncellenemedi",
                        Items = blresult.Errors,
                        RedirectingUrl = "/Home/EditProfile",
                        RedirectingTimeout = 10000
                    };

                    return View("Error", errModel);
                }

                CurrentSession.Set<NotePortalUser>("login", blresult.Result);
                // profil güncellendiği için session güncellendi.

                return RedirectToAction("ProfilePage");
            }

            return View(user);
        }

        [Auth]
        public ActionResult RemoveProfile()
        {
            NotePortalUser currentUser = CurrentSession.User as NotePortalUser;

            BusinessLayerResult<NotePortalUser> blresult = new BusinessLayerResult<NotePortalUser>();

            blresult = npum.RemoveUserById(currentUser.Id);

            if (blresult.Errors.Count > 0)
            {
                ErrorViewModel errModel = new ErrorViewModel()
                {
                    Title = "Kullanıcı silinemedi.",
                    Items = blresult.Errors,
                    RedirectingUrl = "/Home/ProfilePage"
                };

                return View(errModel);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<NotePortalUser> userResult = npum.LoginUser(model);

                if (userResult.Result == null)
                {
                    userResult.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(model);
                }
                else
                {
                    CurrentSession.Set<NotePortalUser>("login", userResult.Result);

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                #region control prototype for mvc
                //if (model.Username == "aaa")
                //{
                //    ModelState.AddModelError("Username", "Bu kullanıcı zaten mevcut.");
                //    hasError = true;
                //}

                //if (model.Email == "abc@xyz.com")
                //{
                //    ModelState.AddModelError("Email", "Bu email zaten kayıtlı.");
                //    hasError = true;
                //}

                #endregion

                #region try-catch alternative
                //try
                //{
                //    NotePortalUser user = mpum.RegisterUser(model);
                //}
                //catch (Exception ex)
                //{
                //    ModelState.AddModelError("", ex.Message);
                //}
                #endregion

                BusinessLayerResult<NotePortalUser> res = npum.RegisterUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    // listedeki her eleman için bir action<param> gerçekleştirir.

                    return View(model);
                }

                OkViewModel okModel = new OkViewModel()
                {
                    Title = "Kayıt başarılı!",
                    Items = new List<string> { "Lütfen e-postanıza gönderdiğimiz aktivasyon link'ine tıklayarak hesabınızı aktif ediniz. Hesabınızı aktif etmeden not yazamaz, yorum yapamaz ve favori oluşturamazsınız." },
                };

                return View("Ok", okModel);

            }

            return View(model);

            #region last check for mvc alternative
            //if (hasError == true)
            //{
            //    return View(model);
            //}
            //else
            //{
            //    return RedirectToAction("RegisterSuccess");
            //}

            #endregion
        }

        public ActionResult ActivateUser(Guid id)
        {
            BusinessLayerResult<NotePortalUser> result = npum.ActivateUser(id);

            if (result.Errors.Count > 0)
            {
                ErrorViewModel errModel = new ErrorViewModel();
                TempData["errors"] = result.Errors;

                errModel.Title = "Hesabınız aktif edilemedi...";
                errModel.RedirectingTimeout = 5000;

                foreach (ErrorMessageObject msg in result.Errors)
                {
                    errModel.Items.Add(msg);
                }

                return View("Error", errModel);
            }

            OkViewModel okModel = new OkViewModel()
            {
                Title = "Hesabınız aktifleştirildi!",
                Items = { "Artık NotePortal'ı kullanmaya başlayabilirisiniz!.." },
                RedirectingTimeout = 5000
            };

            return View("Ok", okModel);
        }

        public ActionResult TestNotify()
        {
            InfoViewModel okModel = new InfoViewModel();

            ErrorMessageObject emo = new ErrorMessageObject()
            {
                Code = ErrorMessage.UserIsNotActive,
                Message = "Kullanıcı aktif değilmiş..."
            };

            ErrorMessageObject emo2 = new ErrorMessageObject()
            {
                Code = ErrorMessage.EmailAlreadyExists,
                Message = "Eposta zaten varmış..."
            };

            okModel.Header = "Yönleniyoruz...";
            okModel.Title = "Info test edildi... Test başarılı!";
            okModel.Items.Add("Bilgi veriliyor...");
            okModel.Items.Add("Bilgilendiniz!");
            okModel.IsRedirecting = true;
            okModel.RedirectingTimeout = 10000;

            return View("Info", okModel);
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}