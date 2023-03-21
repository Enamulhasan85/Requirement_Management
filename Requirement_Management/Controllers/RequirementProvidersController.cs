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
using Requirement_Management.ViewModels;

namespace Requirement_Management.Controllers
{
    [CustomAuthorize(Roles = "systemadmin, admin, normal user")]
    public class RequirementProvidersController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: RequirementProviders
        public ActionResult Index(string msg)
        {
            ViewBag.Message = msg;
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
            ViewBag.CompanyId = new MultiSelectList(db.ClientCompany, "Id", "Name");
            return View();
        }

        // POST: RequirementProviders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReqProviderView requirementProvider)
        {
            if (ModelState.IsValid)
            {
                RequirementProvider reqProvider = new RequirementProvider();
                reqProvider.Name = requirementProvider.Name;
                reqProvider.Contact = requirementProvider.Contact;
                reqProvider.Email = requirementProvider.Email;
                db.RequirementProvider.Add(reqProvider);
                db.SaveChanges();

                if (requirementProvider.CompanyId != null)
                {
                    foreach (var companyid in requirementProvider.CompanyId)
                    {
                        CompanyProvider comProvider = new CompanyProvider();
                        comProvider.ReqProviderId = reqProvider.Id;
                        comProvider.CompanyId = companyid;
                        db.CompanyProvider.Add(comProvider);
                        db.SaveChanges();
                    }
                }
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

            ViewBag.CompanyId = new MultiSelectList(db.ClientCompany, "Id", "Name");
            return View(requirementProvider);
        }

        public JsonResult GetProvidersCompany(int? id)
        {
            var comlist = db.CompanyProvider.Where(i => i.ReqProviderId == id).ToList();
            var companies = comlist.Select(c => new {
                Id = c.CompanyId,
            }).ToList();

            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        // POST: RequirementProviders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReqProviderView requirementProvider)
        {
            if (ModelState.IsValid)
            {
                RequirementProvider reqProvider = new RequirementProvider();
                reqProvider.Id = requirementProvider.Id;
                reqProvider.Name = requirementProvider.Name;
                reqProvider.Contact = requirementProvider.Contact;
                reqProvider.Email = requirementProvider.Email;
                db.Entry(reqProvider).State = EntityState.Modified;
                db.SaveChanges();

                var ComProList = db.CompanyProvider.Where(i => i.ReqProviderId == requirementProvider.Id).ToList();
                foreach (var item in ComProList)
                {
                    db.CompanyProvider.Remove(item);
                    db.SaveChanges();
                }

                if (requirementProvider.CompanyId != null)
                {
                    foreach (var companyid in requirementProvider.CompanyId)
                    {
                        CompanyProvider comProvider = new CompanyProvider();
                        comProvider.ReqProviderId = reqProvider.Id;
                        comProvider.CompanyId = companyid;
                        db.CompanyProvider.Add(comProvider);
                        db.SaveChanges();
                    }
                }
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
            if (db.RequirementDetail.Where(r => r.ReqProviderId == id).Count() != 0)
            {
                return RedirectToAction("Index", new { msg = "Delete RequirementDetails Under this RequirementProvider" });
            }

            var ComProList = db.CompanyProvider.Where(i => i.ReqProviderId == id).ToList();
            foreach(var item in ComProList)
            {
                db.CompanyProvider.Remove(item);
                db.SaveChanges();
            }

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
