using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Requirement_Management.Models;

namespace Requirement_Management.Controllers
{
    public class SoftwareCategoriesController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: SoftwareCategories
        public ActionResult Index()
        {
            return View(db.SoftwareCategory.ToList());
        }

        // GET: SoftwareCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SoftwareCategory softwareCategory = db.SoftwareCategory.Find(id);
            if (softwareCategory == null)
            {
                return HttpNotFound();
            }
            return View(softwareCategory);
        }

        // GET: SoftwareCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SoftwareCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] SoftwareCategory softwareCategory)
        {
            if (ModelState.IsValid)
            {
                db.SoftwareCategory.Add(softwareCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(softwareCategory);
        }

        // GET: SoftwareCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SoftwareCategory softwareCategory = db.SoftwareCategory.Find(id);
            if (softwareCategory == null)
            {
                return HttpNotFound();
            }
            return View(softwareCategory);
        }

        // POST: SoftwareCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] SoftwareCategory softwareCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(softwareCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(softwareCategory);
        }

        // GET: SoftwareCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SoftwareCategory softwareCategory = db.SoftwareCategory.Find(id);
            if (softwareCategory == null)
            {
                return HttpNotFound();
            }
            return View(softwareCategory);
        }

        // POST: SoftwareCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SoftwareCategory softwareCategory = db.SoftwareCategory.Find(id);
            db.SoftwareCategory.Remove(softwareCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
