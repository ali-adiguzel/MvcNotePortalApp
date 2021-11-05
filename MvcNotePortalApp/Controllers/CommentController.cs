using MvcNotePortalApp.BusinessLayer;
using MvcNotePortalApp.Entities;
using MvcNotePortalApp.Filters;
using MvcNotePortalApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MvcNotePortalApp.Controllers
{
    [Excptn]
    public class CommentController : Controller
    {
        private NoteManager nm = new NoteManager();
        private CommentManager comMn = new CommentManager();

        public ActionResult ShowNoteComments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Note note = nm.Find(x => x.Id == id); <-- alternative

            Note note = nm.ListQueryable().Include("Comments").FirstOrDefault(x => x.Id == id);

            if (note == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialComments", note.Comments.OrderByDescending(x => x.ModifiedDate).ToList());
        }

        [Auth]
        [HttpPost]
        public ActionResult Create(Comment comment, int? noteId)

            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("ModifiedUserName");

            if (ModelState.IsValid)
            {
                if (noteId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Note note = nm.Find(x => x.Id == noteId);

                if (note == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                comment.Note = note; 
                comment.Owner = CurrentSession.User; 

                if (comMn.Insert(comment) > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        [HttpPost]
        public ActionResult Edit(int? id, string text)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = comMn.Find(x => x.Id == id);

            if (comment == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            comment.Text = text;
            if (comMn.Update(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = comMn.Find(x => x.Id == id);

            if (comment == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (comMn.Delete(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }
}