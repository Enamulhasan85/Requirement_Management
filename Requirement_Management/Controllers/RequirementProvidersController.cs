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
    public class RequirementProvidersController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: RequirementProviders
        public ActionResult Index()
        {
            return View(db.RequirementProvider.ToList());
        }

        // GET: RequirementProviders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequirementProvider requirementProvider = db.RequirementProvider.Find(id);
            if (requirementProvider == null)
            {
                return HttpNotFound();
            }
            return View(requirementProvider);
        }

        // GET: RequirementProviders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RequirementProviders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Contact,Email")] RequirementProvider requirementProvider)
        {
            if (ModelState.IsValid)
            {
                db.RequirementProvider.Add(requirementProvider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(requirementProvider);
        }

        // GET: RequirementProviders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequirementProvider requirementProvider = db.RequirementProvider.Find(id);
            if (requirementProvider == null)
            {
                return HttpNotFound();
            }
            return View(requirementProvider);
        }

        // POST: RequirementProviders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Contact,Email")] RequirementProvider requirementProvider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requirementProvider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(requirementProvider);
        }

        // GET: RequirementProviders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequirementProvider requirementProvider = db.RequirementProvider.Find(id);
            if (requirementProvider == null)
            {
                return HttpNotFound();
            }
            return View(requirementProvider);
        }

        // POST: RequirementProviders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RequirementProvider requirementProvider = db.RequirementProvider.Find(id);
            db.RequirementProvider.Remove(requirementProvider);
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
