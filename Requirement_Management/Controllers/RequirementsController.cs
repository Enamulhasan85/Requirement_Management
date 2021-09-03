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
    public class RequirementsController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: Requirements
        public ActionResult Index()
        {
            var requirement = db.Requirement.Include(r => r.Company).Include(r => r.ReqProvider)/*.Include(r => r.ReqType)*/;
            return View(requirement.ToList());
        }

        // GET: Requirements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requirement requirement = db.Requirement.Find(id);
            if (requirement == null)
            {
                return HttpNotFound();
            }
            return View(requirement);
        }

        // GET: Requirements/Create
        public ActionResult Create()
        {
            //ViewBag.Status = new SelectList()     
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            return View();
        }

        // POST: Requirements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(Requirement requirement, List<RequirementDetail> reqDetails)
        {

            requirement.EntryDate = DateTime.Now;
            db.Requirement.Add(requirement);
            db.SaveChanges();

            foreach (var info in reqDetails)
            {
                RequirementDetail row = new RequirementDetail();
                row.Requirement = info.Requirement;
                row.Description = info.Description;
                row.ReqTypeId = info.ReqTypeId;
                row.ReqId = requirement.Id;
                row.Status = info.Status;
                row.StarMarked = info.StarMarked;
                db.RequirementDetail.Add(row);
                db.SaveChanges();
            }

            return Json(new { Id = requirement.Id }, JsonRequestBehavior.AllowGet);

            //ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name", requirement.CompanyId);
            //ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name", requirement.ReqProviderId);
            ////ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name", requirement.ReqTypeId);
            //return View(requirement);
        }

        //public ActionResult Create(Requirement requirement)
        //{
            
        //    if (ModelState.IsValid)
        //    {
        //        requirement.EntryDate = DateTime.Now;
        //        db.Requirement.Add(requirement);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name", requirement.CompanyId);
        //    ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name", requirement.ReqProviderId);
        //    //ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name", requirement.ReqTypeId);
        //    return View(requirement);
        //}

        // GET: Requirements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requirement requirement = db.Requirement.Find(id);
            if (requirement == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name", requirement.CompanyId);
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name", requirement.ReqProviderId);
            //ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name", requirement.ReqTypeId);
            return View(requirement);
        }

        // POST: Requirements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Date,EntryDate,CompanyId,ReqTypeId,ReqProviderId,Status,StarMarked")] Requirement requirement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requirement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name", requirement.CompanyId);
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name", requirement.ReqProviderId);
            //ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name", requirement.ReqTypeId);
            return View(requirement);
        }

        // GET: Requirements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requirement requirement = db.Requirement.Find(id);
            if (requirement == null)
            {
                return HttpNotFound();
            }
            return View(requirement);
        }

        // POST: Requirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Requirement requirement = db.Requirement.Find(id);
            db.Requirement.Remove(requirement);
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
