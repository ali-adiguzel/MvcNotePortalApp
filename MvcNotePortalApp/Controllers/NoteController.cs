using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcNotePortalApp.BusinessLayer;
using MvcNotePortalApp.Entities;
using MvcNotePortalApp.Filters;
using MvcNotePortalApp.Models;

namespace MvcNotePortalApp.Controllers
{
    [Excptn]
    public class NoteController : Controller
    {
        NoteManager nm = new NoteManager();
        CategoryManager cm = new CategoryManager();
        LikedManager lm = new LikedManager();
        CommentManager comMn = new CommentManager();

        [Auth]
        public ActionResult Index()
        {
            NotePortalUser user = CurrentSession.User;

            List<Note> userNotes = user.Notes;

            return View(userNotes.OrderByDescending(x => x.ModifiedDate).ToList());
        }

        public ActionResult ShowNoteModal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = nm.Find(x => x.Id == id);

            if (note == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialNote", note);
        }

        [Auth]
        public ActionResult MyLikedNotes()
        {
            var notes = lm.ListQueryable().Include("LikedUser").Include("Note").Where(
                x => x.LikedUser.Id == CurrentSession.User.Id).Select(x => x.Note).Include("Category").OrderByDescending(x => x.ModifiedDate);

            return View("Index", notes.ToList());
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(cm.List(), "Id", "Title");

            return View();
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note)
        {
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("ModifiedUserName");

            if (ModelState.IsValid)
            {
                note.Owner = CurrentSession.User;

                nm.Insert(note);

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);

            return View(note);
        }

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = nm.Find(x => x.Id == id.Value);

            if (note == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);

            return View(note);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Note note)
        {
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("ModifiedUserName");

            if (ModelState.IsValid)
            {
                Note db_note = nm.Find(x => x.Id == note.Id);
                db_note.IsDraft = note.IsDraft;
                db_note.CategoryId = note.CategoryId;
                db_note.Text = note.Text;
                db_note.Title = note.Title;

                nm.Update(db_note);

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);

            return View(note);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = nm.Find(x => x.Id == id.Value);

            if (note == null)
            {
                return HttpNotFound();
            }

            return View(note);
        }

        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = nm.Find(x => x.Id == id);

            nm.Delete(note);

            return RedirectToAction("Index");
        }

        [Auth]
        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            List<int> likedNotesIds = lm.List(
                x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Note.Id)).Select(
                x => x.Note.Id).ToList();

            return Json(new { result = likedNotesIds });
        }

        [Auth]
        [HttpPost]
        public ActionResult SetLikeStatus(int noteId, bool liked)
        {
            int res = 0;

            Liked like = lm.Find(x => x.Note.Id == noteId && x.LikedUser.Id == CurrentSession.User.Id);

            Note note = nm.Find(x => x.Id == noteId);

            if (like != null && liked == false)
            {
                res = lm.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = lm.Insert(new Liked
                {
                    LikedUser = CurrentSession.User,
                    Note = note
                });
            }

            if (res > 0)
            {
                if (liked)
                {
                    note.LikeCount++;
                }
                else
                {
                    note.LikeCount--;
                }

                res = nm.Update(note);

                return Json(new { hasError = false, errorMessage = string.Empty, result = note.LikeCount });
            }

            return Json(new { hasError = true, errorMessage = "Beğenme işlemi gerçekleştirilemedi.", result = note.LikeCount });
        }
    }
}
