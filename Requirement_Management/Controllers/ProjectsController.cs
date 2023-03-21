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
    public class ProjectsController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: Projects
        public ActionResult Index(string msg)
        {
            ViewBag.Message = msg;
            ProjectView project = new ProjectView();
            project.Project = db.Project.Include(p => p.Company).ToList();
            
            return View(project);
        }

        public JsonResult GetManageRecords(int ProjectId)
        {
            List<ProjectSchedule> ProSchedules = db.ProjectSchedule.Where(r => r.ProjectId == ProjectId).ToList();

            var records = ProSchedules.Select(item => new {
                Id = item.Id,
                ProjectId = item.ProjectId,
                ModeId = item.ProjectMode,
                Mode = item.ProjectMode.ToString(),
                TargetCost = item.TargetCost,
                StartDate = item.StartDate.Date.ToString("dd/MM/yyyy"),
                TargetDate = item.TargetDate,
                EndDate = item.EndDate,
                StatusId = item.Status,
                Status = item.Status.ToString(),
            }).ToList();

            return Json(records, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddRecord(ProjectSchedule ProSche)
        {
            db.ProjectSchedule.Add(ProSche);
            db.SaveChanges();

            return Json(new { Id = ProSche.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditRecord(ProjectSchedule ProSche)
        {
            db.Entry(ProSche).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Id = ProSche.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteRecord(int? Id)
        {
            ProjectSchedule ProSche = db.ProjectSchedule.Where(r => r.Id == Id).FirstOrDefault();

            if (ProSche != null) {
                db.ProjectSchedule.Remove(ProSche);
                db.SaveChanges();

                return Json(new { Id = ProSche.Id }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Id = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.SoftwareId = new SelectList(db.Software, "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(ProjectView project)
        {

            Project proj = new Project();
            proj.Title = project.Title;
            proj.StartDate = project.StartDate;
            proj.CompanyId = project.CompanyId;

            db.Project.Add(proj);
            db.SaveChanges();

            foreach(var software in project.ProSoftware)
            {
                ProjectSoftware ProSoft = new ProjectSoftware();
                ProSoft.ProjectId = proj.Id;
                ProSoft.SoftwareId = software.SoftwareId;
                ProSoft.Status = software.Status;

                db.ProjectSoftware.Add(ProSoft);
                db.SaveChanges();
            }

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ProjectView proj = new ProjectView();
            proj.Id = project.Id;
            proj.Title = project.Title;
            proj.StartDate = project.StartDate;
            //proj.CompanyId = project.CompanyId;

            List<ProjectSoftware> softlist = db.ProjectSoftware.Where(r => r.ProjectId == id).ToList();

            foreach (var software in softlist)
            {
                ProjectSoftwareView ProSoft = new ProjectSoftwareView();
                ProSoft.SoftwareId = software.SoftwareId;
                ProSoft.SoftwareName = software.Software.Name;
                ProSoft.Status = software.Status;
                proj.ProSoftware.Add(ProSoft);
            }

            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name", project.CompanyId);
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.SoftwareId = new SelectList(db.Software, "Id", "Name");

            return View(proj);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Edit(ProjectView project)
        {

            Project proj = db.Project.Where(r => r.Id == project.Id).FirstOrDefault();

            if (proj == null) return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);

            proj.Title = project.Title;
            proj.StartDate = project.StartDate;
            proj.CompanyId = project.CompanyId;

            db.Entry(proj).State = EntityState.Modified;
            db.SaveChanges();

            List<ProjectSoftware> softlist = db.ProjectSoftware.Where(r => r.ProjectId == proj.Id).ToList();
            foreach (var item in softlist)
            {
                db.ProjectSoftware.Remove(item);
                db.SaveChanges();
            }

            foreach (var software in project.ProSoftware)
            {
                ProjectSoftware ProSoft = new ProjectSoftware();
                ProSoft.ProjectId = proj.Id;
                ProSoft.SoftwareId = software.SoftwareId;
                ProSoft.Status = software.Status;

                db.ProjectSoftware.Add(ProSoft);
                db.SaveChanges();
            }

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);

        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (db.RequirementDetail.Where(r => r.ProjectId == id).FirstOrDefault() != null)
            {
                return RedirectToAction("Index", new { msg = "Delete RequirementDetails Under this Project" });
            }

            List<ProjectSoftware> ProSoft = db.ProjectSoftware.Where(r => r.ProjectId == id).ToList();
            foreach (var item in ProSoft)
            {
                db.ProjectSoftware.Remove(item);
                db.SaveChanges();
            }

            List<ProjectSchedule> ProSche = db.ProjectSchedule.Where(r => r.ProjectId == id).ToList();
            foreach (var item in ProSche)
            {
                db.ProjectSchedule.Remove(item);
                db.SaveChanges();
            }

            Project project = db.Project.Find(id);
            db.Project.Remove(project);
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
