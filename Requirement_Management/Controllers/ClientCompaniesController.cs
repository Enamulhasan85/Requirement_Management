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
    public class ClientCompaniesController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: ClientCompanies
        public ActionResult Index(string msg)
        {
            ViewBag.Message = msg;
            return View(db.ClientCompany.ToList());
        }

        // GET: ClientCompanies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientCompany clientCompany = db.ClientCompany.Find(id);
            if (clientCompany == null)
            {
                return HttpNotFound();
            }
            return View(clientCompany);
        }

        // GET: ClientCompanies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,Contact")] ClientCompany clientCompany)
        {
            if (ModelState.IsValid)
            {
                db.ClientCompany.Add(clientCompany);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientCompany);
        }

        // GET: ClientCompanies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientCompany clientCompany = db.ClientCompany.Find(id);
            if (clientCompany == null)
            {
                return HttpNotFound();
            }
            return View(clientCompany);
        }

        // POST: ClientCompanies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Contact")] ClientCompany clientCompany)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientCompany).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientCompany);
        }

        // GET: ClientCompanies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientCompany clientCompany = db.ClientCompany.Find(id);
            if (clientCompany == null)
            {
                return HttpNotFound();
            }
            return View(clientCompany);
        }

        // POST: ClientCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if(db.Project.Where(r => r.CompanyId == id).Count() != 0 || db.Requirement.Where(r => r.CompanyId == id).Count() != 0)
            {
                return RedirectToAction("Index", new { msg = "Delete Projects and Requirements Under this Company" });
            }
            ClientCompany clientCompany = db.ClientCompany.Find(id);
            db.ClientCompany.Remove(clientCompany);
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
