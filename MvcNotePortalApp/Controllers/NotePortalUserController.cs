using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcNotePortalApp.BusinessLayer;
using MvcNotePortalApp.BusinessLayer.Result;
using MvcNotePortalApp.Entities;
using MvcNotePortalApp.Filters;

namespace MvcNotePortalApp.Controllers
{
    [Auth]
    [AuthAdmin]
    [Excptn]
    public class NotePortalUserController : Controller
    {
        private NotePortalUserManager npum = new NotePortalUserManager();

        public ActionResult Index()
        {
            return View(npum.List());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotePortalUser notePortalUser = npum.Find(x => x.Id == id.Value);
            if (notePortalUser == null)
            {
                return HttpNotFound();
            }
            return View(notePortalUser);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NotePortalUser notePortalUser)
        {
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("ModifiedUserName");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<NotePortalUser> res = npum.Insert(notePortalUser);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(notePortalUser);
                }

                return RedirectToAction("Index");
            }

            return View(notePortalUser);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotePortalUser notePortalUser = npum.Find(x => x.Id == id.Value);
            if (notePortalUser == null)
            {
                return HttpNotFound();
            }
            return View(notePortalUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NotePortalUser notePortalUser)
        {
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("ModifiedUserName");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<NotePortalUser> res = npum.Update(notePortalUser);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(notePortalUser);
                }

                return RedirectToAction("Index");
            }

            return View(notePortalUser);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotePortalUser notePortalUser = npum.Find(x => x.Id == id.Value);
            if (notePortalUser == null)
            {
                return HttpNotFound();
            }
            return View(notePortalUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NotePortalUser notePortalUser = npum.Find(x => x.Id == id);
            npum.Delete(notePortalUser);
            
            return RedirectToAction("Index");
        }
    }
}
