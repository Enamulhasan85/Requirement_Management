using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Requirement_Management.Models;
using Requirement_Management.ViewModels;

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
            var softwares = db.Software.Select(c => new {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            ViewBag.SoftwareId = new MultiSelectList(softwares, "Id", "Name");
            //ViewBag.SoftwareId = new SelectList(db.Software, "Id", "Name");
            return View();
        }

        // POST: Requirements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(RequirementView reqView)
        {
            Requirement requirement = new Requirement();
            requirement.Title = reqView.Title;
            requirement.Date = reqView.Date;
            requirement.EntryDate = DateTime.Now;
            requirement.CompanyId = reqView.CompanyId;
            requirement.ReqProviderId = reqView.ReqProviderId;

            db.Requirement.Add(requirement);
            db.SaveChanges();
            
            foreach (var info in reqView.ReqDetail)
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

                char[] separators = new char[] { ',', ' ' };

                string[] softwares = (info.ReqSoftwareId).Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var soft in softwares)
                {
                    RequirementSoftware Software = new RequirementSoftware();
                    Software.SoftwareId = short.Parse(soft);
                    Software.RequirementDetailId = row.Id;
                    db.RequirementSoftware.Add(Software);
                    db.SaveChanges();
                }

            }


            return Json(new { Id = requirement.Id }, JsonRequestBehavior.AllowGet);
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
            RequirementView reqView = new RequirementView();
            reqView.Id = requirement.Id;
            reqView.Title = requirement.Title;
            reqView.Date = requirement.Date;

            List<RequirementDetail> reqDetail = db.RequirementDetail.Where(r => r.ReqId == requirement.Id).ToList();

            foreach (var info in reqDetail)
            {
                RequirementDetailView reqdetail = new RequirementDetailView();
                reqdetail.Id = info.Id;
                reqdetail.Requirement = info.Requirement;
                reqdetail.Description = info.Description;
                reqdetail.ReqTypeId = info.ReqTypeId;
                reqdetail.ReqTypeName = info.ReqType.Name;
                reqdetail.ReqId = requirement.Id;
                reqdetail.Status = info.Status;
                reqdetail.StarMarked = info.StarMarked;
                
                List<RequirementSoftware> reqSoftware = db.RequirementSoftware.Where(r => r.RequirementDetailId == info.Id).ToList();

                string sa = "";
                string softwareName = "";
                foreach (var soft in reqSoftware)
                {
                    sa += soft.SoftwareId + ", ";
                    softwareName += soft.Software.Name + ", ";
                }
                sa = sa.Remove(sa.Length - 1, 1);
                sa = sa.Remove(sa.Length - 1, 1);
                softwareName = softwareName.Remove(softwareName.Length - 1, 1);
                softwareName = softwareName.Remove(softwareName.Length - 1, 1);
                reqdetail.ReqSoftwareId += sa;
                reqdetail.ReqSoftwareName += softwareName;

                reqView.ReqDetail.Add(reqdetail);
            }


            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name", requirement.CompanyId);
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name", requirement.ReqProviderId);
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            var softwares = db.Software.Select(c => new {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            ViewBag.SoftwareId = new MultiSelectList(softwares, "Id", "Name");
            return View(reqView);
        }

        // POST: Requirements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult EditReq(RequirementView reqView)
        {
            Requirement requirement = new Requirement();
            requirement.Id = reqView.Id;
            requirement.Title = reqView.Title;
            requirement.Date = reqView.Date;
            requirement.EntryDate = DateTime.Now;
            requirement.CompanyId = reqView.CompanyId;
            requirement.ReqProviderId = reqView.ReqProviderId;

            db.Entry(requirement).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Id = requirement.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddReqDetail(RequirementDetailView reqdetailView)
        {
            RequirementDetail reqdetail = new RequirementDetail();
            reqdetail.Requirement = reqdetailView.Requirement;
            reqdetail.Description = reqdetailView.Description;
            reqdetail.ReqTypeId = reqdetailView.ReqTypeId;
            reqdetail.ReqId = reqdetailView.ReqId;
            reqdetail.Status = reqdetailView.Status;
            reqdetail.StarMarked = reqdetailView.StarMarked;

            db.RequirementDetail.Add(reqdetail);
            db.SaveChanges();

            char[] separators = new char[] { ',', ' ' };

            string[] softwares = (reqdetailView.ReqSoftwareId).Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var soft in softwares)
            {
                RequirementSoftware Software = new RequirementSoftware();
                Software.SoftwareId = short.Parse(soft);
                Software.RequirementDetailId = reqdetail.Id;
                db.RequirementSoftware.Add(Software);
                db.SaveChanges();
            }

            return Json(new { Id = reqdetail.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditReqDetail(RequirementDetailView reqdetailView)
        {
            RequirementDetail reqdetail = new RequirementDetail();
            reqdetail.Id = reqdetailView.Id;
            reqdetail.Requirement = reqdetailView.Requirement;
            reqdetail.Description = reqdetailView.Description;
            reqdetail.ReqTypeId = reqdetailView.ReqTypeId;
            reqdetail.ReqId = reqdetailView.ReqId;
            reqdetail.Status = reqdetailView.Status;
            reqdetail.StarMarked = reqdetailView.StarMarked;

            db.Entry(reqdetail).State = EntityState.Modified;
            db.SaveChanges();

            List<RequirementSoftware> reqSoftware = db.RequirementSoftware.Where(i => i.RequirementDetailId == reqdetailView.Id).ToList();
            foreach (var soft in reqSoftware)
            {
                db.RequirementSoftware.Remove(soft);
                db.SaveChanges();
            }

            char[] separators = new char[] { ',', ' ' };

            string[] softwares = (reqdetailView.ReqSoftwareId).Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var soft in softwares)
            {
                RequirementSoftware Software = new RequirementSoftware();
                Software.SoftwareId = short.Parse(soft);
                Software.RequirementDetailId = reqdetail.Id;
                db.RequirementSoftware.Add(Software);
                db.SaveChanges();
            }

            return Json(new { Id = reqdetail.Id }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult EditReqDetail(RequirementDetailView reqDetailView)
        //{
        //    RequirementDetail reqDetail = new RequirementDetail();
        //    reqDetail.Id = reqView.Id;
        //    reqDetail.Title = reqView.Title;
        //    reqDetail.Date = reqView.Date;
        //    reqDetail.EntryDate = DateTime.Now;
        //    reqDetail.CompanyId = reqView.CompanyId;
        //    reqDetail.ReqProviderId = reqView.ReqProviderId;

        //    db.Entry(requirement).State = EntityState.Modified;
        //    db.SaveChanges();

        //    return Json(new { Id = requirement.Id }, JsonRequestBehavior.AllowGet);
        //}

        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Requirement requirement)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(requirement).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name", requirement.CompanyId);
        //    ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name", requirement.ReqProviderId);
        //    //ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name", requirement.ReqTypeId);
        //    return View(requirement);
        //}

        [HttpPost]
        public JsonResult DeleteReqDetail(int? id)
        {
            List<RequirementSoftware> reqSoftware = db.RequirementSoftware.Where(i => i.RequirementDetailId == id).ToList();
            foreach (var soft in reqSoftware)
            {
                db.RequirementSoftware.Remove(soft);
                db.SaveChanges();
            }
            RequirementDetail ReqDetail = db.RequirementDetail.Find(id);
            db.RequirementDetail.Remove(ReqDetail);
            db.SaveChanges();

            return Json(new { Id = ReqDetail.Id }, JsonRequestBehavior.AllowGet);
        }

        // POST: Requirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            List<RequirementDetail> reqDetail = db.RequirementDetail.Where(i => i.ReqId == id).ToList();
            foreach (var info in reqDetail)
            {
                List<RequirementSoftware> reqSoftware = db.RequirementSoftware.Where(i => i.RequirementDetailId == info.Id).ToList();
                foreach (var soft in reqSoftware)
                {
                    db.RequirementSoftware.Remove(soft);
                    db.SaveChanges();
                }
                db.RequirementDetail.Remove(info);
                db.SaveChanges();
            }

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
