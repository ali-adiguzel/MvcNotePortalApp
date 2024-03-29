﻿using System;
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
    [Auth, AuthAdmin]
    [Excptn]
    public class CategoryController : Controller
    {
        private CategoryManager cm = new CategoryManager();

        public ActionResult Index()
        {
            return View(cm.List());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = cm.Find(x => x.Id == id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("ModifiedUserName");

            if (ModelState.IsValid)
            {
                cm.Insert(category);

                // insert'ten sonra kategorileri cache'den silelim ki yenilensin
                CacheHelper.RemoveCategoriesFromCache();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            }
            Category category = cm.Find(x => x.Id == id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("ModifiedUserName");

            if (ModelState.IsValid)
            {
                Category cat = cm.Find(x => x.Id == category.Id);
                cat.Title = category.Title;
                cat.Description = category.Description;

                cm.Update(category);

                CacheHelper.RemoveCategoriesFromCache();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = cm.Find(x => x.Id == id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = cm.Find(x => x.Id == id);

            cm.Delete(category);

            CacheHelper.RemoveCategoriesFromCache();

            return RedirectToAction("Index");
        }
    }
}
