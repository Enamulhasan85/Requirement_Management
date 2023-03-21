using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Requirement_Management.Models;
using Requirement_Management.CustomAuthentication;

namespace Requirement_Management.Controllers
{
    [CustomAuthorize(Roles = "systemadmin, admin, normal user")]
    public class SoftwareCategoriesController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: SoftwareCategories
        public ActionResult Index(string msg)
        {
            ViewBag.Message = msg;
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
            if (db.RequirementDetail.Where(r => r.SoftCategoryId == id).Count() != 0)
            {
                return RedirectToAction("Index", new { msg = "Delete Software and RequirementDetails Under this Software Category" });
            }
            if (db.Software.Where(r => r.Software_CategoryId == id).Count() != 0)
            {
                return RedirectToAction("Index", new { msg = "Delete Software and RequirementDetails Under this Software Category" });
            }
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
