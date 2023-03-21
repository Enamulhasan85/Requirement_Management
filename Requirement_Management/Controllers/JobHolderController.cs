using Requirement_Management.CustomAuthentication;
using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Requirement_Management.Controllers
{
    [CustomAuthorize(Roles = "systemadmin, admin, normal user")]
    public class JobHolderController : Controller
    {
        // GET: JobHolder

        private RequirementManagementContext db = new RequirementManagementContext();

        public ActionResult Index(string msg)
        {
            ViewBag.Message = msg;
            return View(db.JobHolders.ToList());
        }

        // GET: JobHolder/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: JobHolder/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: JobHolder/Create
        [HttpPost]
        public ActionResult Create(JobHolder Jbh)
        {
            if (ModelState.IsValid)
            {
                db.JobHolders.Add(Jbh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Jbh);
        }

        // GET: JobHolder/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobHolder jobHolder = db.JobHolders.Find(id);
            if (jobHolder == null)
            {
                return HttpNotFound();
            }
            return View(jobHolder);
        }

        // POST: JobHolder/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, JobHolder Jbh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Jbh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Jbh);
        }

        // GET: JobHolder/Delete/5
        public ActionResult Delete(int id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobHolder Jbh = db.JobHolders.Find(id);
            return View(Jbh);
        }

        // POST: JobHolder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (db.ManageRequirement.Where(r => r.JobHolderId == id).Count() != 0)
            {
                return RedirectToAction("Index", new { msg = "Delete ManageRequirements Under this JobHolder" });
            }
            JobHolder Jbh = db.JobHolders.Find(id);
            db.JobHolders.Remove(Jbh);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
