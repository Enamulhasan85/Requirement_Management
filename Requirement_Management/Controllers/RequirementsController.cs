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
using System.IO;
using Microsoft.AspNet.Identity;
using Requirement_Management.CustomAuthentication;
using System.Globalization;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace Requirement_Management.Controllers
{
    [CustomAuthorize(Roles = "systemadmin, admin, normal user")]
    public class RequirementsController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: Requirements
        public ActionResult Index()
        {
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");

            IndexView requirement = new IndexView();
            DateTime To = requirement.To.AddDays(1);
            requirement.Req = db.Requirement.Where(r => r.EntryDate >= requirement.From && r.EntryDate < To ).ToList();
            return View(requirement);
        }

        // POST: Requirements
        [HttpPost]
        public ActionResult Index(IndexView requirement)
        {
            string sql = "select Requirement.Id, Requirement.Title, Requirement.Date, Requirement.EntryDate, Requirement.CompanyId from Requirement where 1=1";

            if (requirement.Id != null)
            {
                sql += "and Id = '" + requirement.Id + "'";
            }
            else if (requirement.Query == Query.EntryDate)
            {
                if (requirement.From != null)
                {
                    sql += "and EntryDate >= '" + requirement.From + "'";
                }
                if (requirement.To != null)
                {
                    sql += "and EntryDate < '" + requirement.To.AddDays(1) + "'";
                }
            }
            else
            {
                if (requirement.From != null)
                {
                    sql += "and Date >= '" + requirement.From + "'";
                }
                if (requirement.To != null)
                {
                    sql += "and Date < '" + requirement.To.AddDays(1) + "'";
                }
            }
            if (requirement.CompanyId != null)
            {
                sql += "and CompanyId = '" + requirement.CompanyId + "'";
            }
            //if (requirement.ReqProviderId != null)
            //{
            //    sql += "and ReqProviderId = '" + requirement.ReqProviderId + "'";
            //}

            requirement.Req = db.Database.SqlQuery<Requirement>(sql).ToList();

            foreach (var row in requirement.Req)
            {
                row.Company = db.ClientCompany.Where(r => r.Id == row.CompanyId).FirstOrDefault();
                //row.ReqProvider = db.RequirementProvider.Where(r => r.Id == row.ReqProviderId).FirstOrDefault();
            }

            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            //requirement.Req = db.Requirement.Where(r => r.EntryDate.Day == DateTime.Now.Day && r.EntryDate.Month == DateTime.Now.Month && r.EntryDate.Year == DateTime.Now.Year).ToList();
            return View(requirement);
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
            RequirementView reqView = new RequirementView();
            reqView.Id = requirement.Id;
            reqView.Title = requirement.Title;
            reqView.Date = requirement.Date;
            reqView.CompanyId = requirement.CompanyId;
            if (requirement.CompanyId != null) reqView.CompanyName = requirement.Company.Name;

            List<RequirementFile> reqFile = db.RequirementFile.Where(r => r.ReqId == requirement.Id).ToList();
            
            foreach (var file in reqFile)
            {
                reqView.FilePath.Add(file.Filename);
            }

            List<RequirementDetail> reqDetail = db.RequirementDetail.Where(r => r.ReqId == requirement.Id).ToList();

            foreach (var info in reqDetail)
            {
                RequirementDetailView reqdetail = new RequirementDetailView();
                reqdetail.Id = info.Id;
                reqdetail.Requirement = info.Requirement;
                reqdetail.Description = info.Description;
                reqdetail.ReqTypeId = info.ReqTypeId;
                if (info.ReqTypeId != null) reqdetail.ReqTypeName = info.ReqType.Name;
                reqdetail.ReqProviderId = info.ReqProviderId;
                if (info.ReqProviderId != null) reqdetail.ReqProviderName = info.ReqProvider.Name;
                reqdetail.ReqId = requirement.Id;
                reqdetail.ReqMode = info.ReqMode;
                reqdetail.Status = info.Status;
                reqdetail.Priority = info.Priority;
                reqdetail.ProjectId = info.ProjectId;
                if (info.ProjectId != null) reqdetail.ProjectName = info.Project.Title;
                reqdetail.ProjectScheduleId = info.ProjectScheduleId;
                if (info.ProjectScheduleId != null) reqdetail.ScheduleName = info.ProjectSchedule.ProjectMode + " - " + info.ProjectSchedule.StartDate.ToString("dd/MM/yyyy");
                reqdetail.SoftCategoryId = info.SoftCategoryId;
                if (info.SoftCategoryId != null) reqdetail.SoftCategoryName = info.SoftCategory.Name;

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
            
            return View(reqView);
        }

        // GET: Requirements/Create
        public ActionResult Create()
        {
            //ViewBag.Status = new SelectList()     
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.Project = new SelectList(db.Project, "Id", "Title");

            var schedules = db.ProjectSchedule.ToList();
            List<CustomProjectScheduleDropDownList> ProSche = new List<CustomProjectScheduleDropDownList>();
            foreach(var item in schedules)
            {
                CustomProjectScheduleDropDownList schedule = new CustomProjectScheduleDropDownList(item.Id, item.StartDate.Date.ToString("dd/MM/yyyy"), item.ProjectMode.ToString());
                ProSche.Add(schedule);
            }
            
            ViewBag.ProjectSchedule = new SelectList(ProSche, "Id", "Name");
            ViewBag.SoftwareId = new MultiSelectList("", "Id", "Name");
            return View();
        }

        // POST: Requirements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    int n = files.Count;
                    string[] filepath = new string[n];
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = User.Identity.GetUserId() + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = User.Identity.GetUserId() + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        filepath[i] = fname;
                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json(new { error = false, message = "File Uploaded Successfully.", filepaths = filepath }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { error = true, message = "File Upload Failed.", filepaths = new string[] { } }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { error = true, message = "No files selected.", filepaths = new string[] { } }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Create(RequirementView reqView)
        {
            Requirement requirement = new Requirement();
            requirement.Title = reqView.Title;
            requirement.Date = reqView.Date;
            requirement.EntryDate = DateTime.Now;
            requirement.CompanyId = reqView.CompanyId;

            db.Requirement.Add(requirement);
            db.SaveChanges();

            foreach (var info in reqView.FilePath)
            {
                RequirementFile row = new RequirementFile();
                row.Filename = info;
                row.ReqId = requirement.Id;
                db.RequirementFile.Add(row);
                db.SaveChanges();
            }

            foreach (var info in reqView.ReqDetail)
            {
                RequirementDetail row = new RequirementDetail();
                row.ReqProviderId = info.ReqProviderId;
                row.Requirement = info.Requirement;
                row.Description = info.Description;
                row.ReqTypeId = info.ReqTypeId;
                row.ReqId = requirement.Id;
                row.Status = info.Status;
                row.ReqMode = info.ReqMode;
                row.Priority = info.Priority;
                row.SoftCategoryId = info.SoftCategoryId;
                row.ProjectId = info.ProjectId;
                row.ProjectScheduleId = info.ProjectScheduleId;
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
                reqdetail.ReqProviderId = info.ReqProviderId;
                if (info.ReqProviderId != null) reqdetail.ReqProviderName = info.ReqProvider.Name;
                reqdetail.Requirement = info.Requirement;
                reqdetail.Description = info.Description;
                reqdetail.ReqTypeId = info.ReqTypeId;
                if (info.ReqTypeId != null) reqdetail.ReqTypeName = info.ReqType.Name;
                reqdetail.ReqId = requirement.Id;
                reqdetail.ReqMode = info.ReqMode;
                reqdetail.Status = info.Status;
                reqdetail.Priority = info.Priority;
                reqdetail.SoftCategoryId = info.SoftCategoryId;
                if (info.SoftCategoryId != null)  reqdetail.SoftCategoryName = info.SoftCategory.Name;
                reqdetail.ProjectId = info.ProjectId;
                if (info.ProjectId != null) reqdetail.ProjectName = info.Project.Title;
                reqdetail.ProjectScheduleId = info.ProjectScheduleId;
                if (info.ProjectScheduleId != null) reqdetail.ScheduleName = info.ProjectSchedule.ProjectMode + " - " + info.ProjectSchedule.StartDate.Date.ToString("dd/MM/yyyy");

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

            List<RequirementFile> reqFile = db.RequirementFile.Where(r => r.ReqId == requirement.Id).ToList();
            foreach(var file in reqFile)
            {
                reqView.FilePath.Add(file.Filename);
            }

            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name", requirement.CompanyId);
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            ViewBag.Project = new SelectList(db.Project, "Id", "Title");

            var schedules = db.ProjectSchedule.ToList();
            List<CustomProjectScheduleDropDownList> ProSche = new List<CustomProjectScheduleDropDownList>();
            foreach (var item in schedules)
            {
                CustomProjectScheduleDropDownList schedule = new CustomProjectScheduleDropDownList(item.Id, item.StartDate.Date.ToString("dd/MM/yyyy"), item.ProjectMode.ToString());
                ProSche.Add(schedule);
            }

            ViewBag.ProjectSchedule = new SelectList(ProSche, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.SoftwareId = new MultiSelectList("", "Id", "Name");
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

            foreach (var info in reqView.FilePath)
            {
                RequirementFile row = new RequirementFile();
                row.Filename = info;
                row.ReqId = requirement.Id;
                db.RequirementFile.Add(row);
                db.SaveChanges();
            }

            db.Entry(requirement).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Id = requirement.Id }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddReqDetail(RequirementDetailView reqdetailView)
        {
            RequirementDetail reqdetail = new RequirementDetail();
            reqdetail.ReqProviderId = reqdetailView.ReqProviderId;
            reqdetail.Requirement = reqdetailView.Requirement;
            reqdetail.Description = reqdetailView.Description;
            reqdetail.ReqTypeId = reqdetailView.ReqTypeId;
            reqdetail.ReqMode = reqdetailView.ReqMode;
            reqdetail.ReqId = reqdetailView.ReqId;
            reqdetail.Status = reqdetailView.Status;
            reqdetail.Priority = reqdetailView.Priority;
            reqdetail.ProjectId = reqdetailView.ProjectId;
            reqdetail.ProjectScheduleId = reqdetailView.ProjectScheduleId;
            reqdetail.SoftCategoryId = reqdetailView.SoftCategoryId;

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
            reqdetail.ReqProviderId = reqdetailView.ReqProviderId;
            reqdetail.Requirement = reqdetailView.Requirement;
            reqdetail.Description = reqdetailView.Description;
            reqdetail.ReqTypeId = reqdetailView.ReqTypeId;
            reqdetail.ReqMode = reqdetailView.ReqMode;
            reqdetail.ReqId = reqdetailView.ReqId;
            reqdetail.Status = reqdetailView.Status;
            reqdetail.Priority = reqdetailView.Priority;
            reqdetail.ProjectId = reqdetailView.ProjectId;
            reqdetail.ProjectScheduleId = reqdetailView.ProjectScheduleId;
            reqdetail.SoftCategoryId = reqdetailView.SoftCategoryId;

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


        public JsonResult GetProjects(int? Id)
        {
            List<Project> ProList = new List<Project>();
            if (Id == null) ProList = db.Project.ToList();
            else ProList = db.Project.Where(r => r.CompanyId == Id).ToList();

            var projects = ProList.Select(c => new {
                Id = c.Id,
                Name = c.Title
            }).ToList();

            return Json(projects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReqProvider(int? Id)
        {
            List<RequirementProvider> reqprolist = db.RequirementProvider.ToList();
            if (Id != null)
            { 
                var comlist = db.CompanyProvider.Where(i => i.CompanyId == Id).ToList();
                List<RequirementProvider> resreqprolist = new List<RequirementProvider>();
                foreach (var provider in reqprolist)
                {
                    if (comlist.Where(i => i.ReqProviderId == provider.Id).FirstOrDefault() != null)
                    {
                        resreqprolist.Add(provider);
                    }
                }
                reqprolist = resreqprolist;
            }
            var reqproviders = reqprolist.Select(c => new {
                Id = c.Id,
                Name = c.Name,
            }).ToList();

            return Json(reqproviders, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchedules(int? Id)
        {
            List<ProjectSchedule> ProScheList = new List<ProjectSchedule>();
            if (Id == null)   ProScheList =  db.ProjectSchedule.Where(r => r.Status == 0).ToList();
            else ProScheList = db.ProjectSchedule.Where(r => r.ProjectId == Id && r.Status == 0).ToList();

            var projects = ProScheList.Select(c => new {
                Id = c.Id,
                Mode = c.ProjectMode.ToString(),
                StartDate = c.StartDate.Date.ToString("dd/MM/yyyy")
            }).ToList();

            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetCategories(int? Id)
        {
            List<SoftwareCategory> SoftwareCategory = new List<SoftwareCategory>();
            List<ProjectSoftware> ProSoft = new List<ProjectSoftware>();
            if (Id == null)
            {
                ProSoft = db.ProjectSoftware.Where(r => r.Status == 0).ToList();
            }
            else
            {
                ProSoft = db.ProjectSoftware.Where(r => r.ProjectId == Id && r.Status == 0).ToList();
            }

            foreach (var Software in ProSoft)
            {
                SoftwareCategory Category = db.SoftwareCategory.Where(i => i.Id == Software.Software.Software_CategoryId).FirstOrDefault();
                if (SoftwareCategory.Find(r => r.Id == Category.Id) == null)
                {
                    SoftwareCategory.Add(Category);
                }
            }

            var categories = SoftwareCategory.Select(c => new {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectSoftwares(int? ProjectId, int? CategoryId)
        {
            List<ProjectSoftware> Softwares = new List<ProjectSoftware>();
            List<Software> SoftList = new List<Software>();
            if (ProjectId == null && CategoryId == null)
            {
                Softwares = db.ProjectSoftware.Where(i => i.Status == 0).ToList();
                foreach(var software in Softwares)
                {
                    if(SoftList.Where(i => i.Id == software.SoftwareId).FirstOrDefault() == null)
                    {
                        SoftList.Add(db.Software.Where(i => i.Id == software.SoftwareId).FirstOrDefault());
                    }
                }
            }
            else if(ProjectId == null)
            {
                Softwares = db.ProjectSoftware.Where(i => i.Software.Software_CategoryId == CategoryId && i.Status == 0).ToList();
                foreach (var software in Softwares)
                {
                    if (SoftList.Where(i => i.Id == software.SoftwareId).FirstOrDefault() == null)
                    {
                        SoftList.Add(db.Software.Where(i => i.Id == software.SoftwareId).FirstOrDefault());
                    }
                }
            }
            else if(CategoryId == null)
            {
                Softwares = db.ProjectSoftware.Where(i => i.ProjectId == ProjectId && i.Status == 0).ToList();
                foreach (var software in Softwares)
                {
                    if (SoftList.Where(i => i.Id == software.SoftwareId).FirstOrDefault() == null)
                    {
                        SoftList.Add(db.Software.Where(i => i.Id == software.SoftwareId).FirstOrDefault());
                    }
                }
            }
            else
            {
                Softwares = db.ProjectSoftware.Where(i => i.ProjectId == ProjectId && i.Software.Software_CategoryId==CategoryId && i.Status == 0).ToList();
                foreach (var software in Softwares)
                {
                    if (SoftList.Where(i => i.Id == software.SoftwareId).FirstOrDefault() == null)
                    {
                        SoftList.Add(db.Software.Where(i => i.Id == software.SoftwareId).FirstOrDefault());
                    }
                }
            }

            SoftList = SoftList.OrderBy(i => i.Id).ToList();
            var softwares = SoftList.Select(c => new {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return Json(softwares, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMode(int? Id)
        {
            int id = (int) db.ProjectSchedule.Where(i => i.Id == Id).FirstOrDefault().ProjectMode;
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSoftwares(int? Id)
        {
            List<Software> Softwares;
            if (Id == null)
            {
                Softwares = db.Software.ToList();
            }
            else
            {
                 Softwares = db.Software.Where(i => i.Software_CategoryId == Id).ToList();
            }
            
            var softwares = Softwares.Select(c => new {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return Json(softwares, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetHourlyCost(int? Id)
        {
            decimal cost=0;
            if (Id == null)
            {
                cost = 0;
            }
            else
            {
                cost = db.JobHolders.Where(x => x.Id == Id).Select(x => x.HourlyCost).FirstOrDefault();
            }
      
            return Json(cost, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        public JsonResult DeleteFile(string Filename)
        {
            RequirementFile reqFile = db.RequirementFile.Where(i => i.Filename == Filename).FirstOrDefault();
            string path = Path.Combine(Server.MapPath("~/Uploads/"), reqFile.Filename);
            FileInfo fl = new FileInfo(path);
            if (fl.Exists)//check file exsit or not  
            {
                fl.Delete();
            }
            db.RequirementFile.Remove(reqFile);
            db.SaveChanges();

            return Json(new { Id = reqFile.Id }, JsonRequestBehavior.AllowGet);
        }

        // POST: Requirements/Delete/5
        [HttpGet, ActionName("Delete")]
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

            List<RequirementFile> reqFile = db.RequirementFile.Where(i => i.ReqId == id).ToList();
            foreach (var file in reqFile)
            {
                string path = Path.Combine(Server.MapPath("~/Uploads/"), file.Filename);
                FileInfo fl = new FileInfo(path);
                if (fl.Exists)//check file exsit or not  
                {
                    fl.Delete();
                }
                db.RequirementFile.Remove(file);
                db.SaveChanges();
            }

            Requirement requirement = db.Requirement.Find(id);
            db.Requirement.Remove(requirement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Requirements/Reports/5
        [CustomAuthorize(Roles = "systemadmin, admin")]
        public ActionResult DetailsReports()
        {
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");

            var schedules = db.ProjectSchedule.ToList();
            List<CustomProjectScheduleDropDownList> ProSche = new List<CustomProjectScheduleDropDownList>();
            foreach (var item in schedules)
            {
                CustomProjectScheduleDropDownList schedule = new CustomProjectScheduleDropDownList(item.Id, item.StartDate.Date.ToString("dd/MM/yyyy"), item.ProjectMode.ToString());
                ProSche.Add(schedule);
            }

            ViewBag.ProjectScheduleId = new SelectList(ProSche, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.SoftwareId = new MultiSelectList("", "Id", "Name");
            ViewBag.JobHolders = new SelectList(db.JobHolders.Where(g => g.isActive == true), "Id", "Name");
            ViewBag.JobHolderId = new SelectList(db.JobHolders.Where(g => g.isActive == true), "Id", "Name");
            var ReqMode = from RequirementMode e in Enum.GetValues(typeof(RequirementMode))
                           select new
                           {
                               Id = (int)e,
                               Name = e.ToString()
                           };
            ViewBag.ReqMode = new SelectList(ReqMode, "Id", "Name");

            ReportView DetailReport = new ReportView();
            return View(DetailReport);
        }

        [CustomAuthorize(Roles = "systemadmin, admin")]
        [HttpPost]
        public ActionResult DetailsReports(ReportView detailReport)
        {
            string sql = @" SELECT RequirementDetail.Id, 
		                    RequirementDetail.ReqProviderId,
		                    RequirementDetail.ReqTypeId,
		                    RequirementDetail.Description,
                            RequirementDetail.Requirement, 
		                    Requirement.Id as RequirementId, 
		                    RequirementDetail.Status, 
		                    RequirementDetail.ReqMode, 
		                    RequirementDetail.Priority, 
                            Requirement.Date, 
		                    Requirement.CompanyId, 
		                    RequirementDetail.SoftCategoryId,
		                    RequirementDetail.ProjectId,
		                    RequirementDetail.ProjectScheduleId
                            From RequirementDetail
                            INNER JOIN Requirement ON Requirement.Id = RequirementDetail.ReqId Where 1 = 1";


            if (detailReport.From != null)
            {
                sql += "and Date >= '" + detailReport.From + "'";
            }
            if (detailReport.To != null)
            {
                sql += "and Date < '" + detailReport.To.AddDays(1) + "'";
            }

            if (detailReport.NullStatus >= 0)
            {
                int status = (int)detailReport.NullStatus;
                sql += "and Status = " + status;
            }
            if (detailReport.ReqMode >= 0)
            {
                int reqmode = (int)detailReport.ReqMode;
                sql += "and ReqMode = " + reqmode;
            }
            if (detailReport.ReqTypeId != null)
            {
                sql += "and ReqTypeId = " + detailReport.ReqTypeId;
            }
            if (detailReport.CompanyId != null)
            {
                sql += "and CompanyId = " + detailReport.CompanyId;
            }
            if (detailReport.ReqProviderId != null)
            {
                sql += "and ReqProviderId = " + detailReport.ReqProviderId;
            }
            if (detailReport.ProjectId != null)
            {
                sql += "and ProjectId = " + detailReport.ProjectId;
            }
            if (detailReport.ProjectScheduleId != null)
            {
                sql += "and ProjectScheduleId = " + detailReport.ProjectScheduleId;
            }
            if (detailReport.CategoryId != null)
            {
                sql += "and SoftCategoryId = " + detailReport.CategoryId;
            }
            if (detailReport.NullPriority >= 0)
            {
                int priority = (int)detailReport.NullPriority;
                sql += "and Priority = " + priority;
            }
            sql += "order by Date DESC";

            ReportView Report = new ReportView();
            Report.ReqDetail = db.Database.SqlQuery<DetailReportView>(sql).ToList();

            if (Report.ReqDetail == null)
            {
                return HttpNotFound();
            }

            foreach (var row in Report.ReqDetail.ToList())
            {
                List<RequirementSoftware> reqSoftware = db.RequirementSoftware.Where(r => r.RequirementDetailId == row.Id).ToList();
                bool found = false;

                if (detailReport.SoftwareId != null)
                {
                    foreach (var software in reqSoftware)
                    {
                        if (detailReport.SoftwareId.Contains(Int32.Parse("" + software.SoftwareId)))
                        {
                            found = true;
                            break;
                        }
                    }
                }
                else
                {
                    found = true;
                }

                if (detailReport.JobHolderId != null)
                {
                    List<ManageRequirement> reqSchedeule = db.ManageRequirement.Where(r => r.RequirementDetailId == row.Id && r.JobHolderId == detailReport.JobHolderId && r.CompType == null).ToList();
                    if (reqSchedeule.Count() == 0) found = false;
                }

                if (found)
                {
                    string sa = "";
                    string softwareName = "";
                    foreach (var software in reqSoftware)
                    {
                        sa += software.SoftwareId + ", ";
                        softwareName += software.Software.Name + ", ";
                    }
                    sa = sa.Remove(sa.Length - 1, 1);
                    sa = sa.Remove(sa.Length - 1, 1);
                    softwareName = softwareName.Remove(softwareName.Length - 1, 1);
                    softwareName = softwareName.Remove(softwareName.Length - 1, 1);
                    row.STRSoftwareId += sa;
                    row.STRSoftwareName += softwareName;
                    if (row.CompanyId != null) row.CompanyName = db.ClientCompany.Find(row.CompanyId).Name;
                    if (row.ReqProviderId != null) row.ReqProviderName = db.RequirementProvider.Find(row.ReqProviderId).Name;
                    if (row.ReqTypeId != null) row.ReqTypeName = db.RequirementType.Find(row.ReqTypeId).Name;
                    if (row.ProjectId != null) row.ProjectName = db.Project.Find(row.ProjectId).Title;
                    if (row.ProjectScheduleId != null) row.ScheduleName = db.ProjectSchedule.Find(row.ProjectScheduleId).ProjectMode + " - " + db.ProjectSchedule.Find(row.ProjectScheduleId).StartDate.ToString("dd/MM/yyyy");
                    if (row.SoftCategoryId != null) row.SoftCategoryName = db.SoftwareCategory.Find(row.SoftCategoryId).Name;
                    if (detailReport.JobHolderId != null) row.JobHolderName = db.JobHolders.Find(detailReport.JobHolderId).Name;

                    List<ManageRequirement> ScheduleList = db.ManageRequirement.Where(r => r.RequirementDetailId == row.Id).ToList();
                    decimal totalconwh = 0, totaltarwh = 0;
                    foreach(var item in ScheduleList)
                    {
                        totalconwh +=  item.WorkhoursConsumed;
                        if (item.TargetWorkhours == 0) totaltarwh += item.WorkhoursConsumed;
                        else totaltarwh +=  item.TargetWorkhours;
                    }
                    row.TotalConWorkhoursinReq = totalconwh;
                    row.TotalTarWorkhoursinReq = totaltarwh;
                }
                else Report.ReqDetail.Remove(row);

            }

            Report.From = detailReport.From;
            Report.To = detailReport.To;
            Report.ReqProviderId = detailReport.ReqProviderId;
            Report.CompanyId = detailReport.CompanyId;
            Report.ReqTypeId = detailReport.ReqTypeId;
            Report.NullPriority = detailReport.NullPriority;
            Report.ReqMode = detailReport.ReqMode;
            Report.NullStatus = detailReport.NullStatus;
            Report.SoftwareId = detailReport.SoftwareId;
            Report.ProjectId = detailReport.ProjectId;
            Report.ProjectScheduleId = detailReport.ProjectScheduleId;

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            var schedules = db.ProjectSchedule.ToList();
            List<CustomProjectScheduleDropDownList> ProSche = new List<CustomProjectScheduleDropDownList>();
            foreach (var item in schedules)
            {
                CustomProjectScheduleDropDownList schedule = new CustomProjectScheduleDropDownList(item.Id, item.StartDate.Date.ToString("dd/MM/yyyy"), item.ProjectMode.ToString());
                ProSche.Add(schedule);
            }
            ViewBag.ProjectScheduleId = new SelectList(ProSche, "Id", "Name");
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.SoftwareId = new MultiSelectList("", "Id", "Name");
            ViewBag.JobHolders = new SelectList(db.JobHolders.Where(g => g.isActive == true), "Id", "Name");
            ViewBag.JobHolderId = new SelectList(db.JobHolders.Where(g => g.isActive == true), "Id", "Name");
            var ReqMode = from RequirementMode e in Enum.GetValues(typeof(RequirementMode))
                          select new
                          {
                              Id = (int)e,
                              Name = e.ToString()
                          };
            ViewBag.ReqMode = new SelectList(ReqMode, "Id", "Name");

            return View(Report);
        }


        [HttpPost]
        public JsonResult DetailsReportEditReq(DetailsRepForEdit reqView)
        {
            RequirementDetail RequirementTobeEdited = db.RequirementDetail.Find(reqView.RequirementDetailId);
            RequirementTobeEdited.ReqProviderId = reqView.ReqProviderId;
            RequirementTobeEdited.Requirement = reqView.Requirement;
            RequirementTobeEdited.Description = reqView.Description;
            RequirementTobeEdited.ReqTypeId = reqView.ReqTypeId;
            RequirementTobeEdited.ReqMode = reqView.ReqMode;
            RequirementTobeEdited.Status = reqView.Status;
            RequirementTobeEdited.Priority = reqView.Priority;
            RequirementTobeEdited.SoftCategoryId = reqView.CategoryId;
            RequirementTobeEdited.ProjectId = reqView.ProjectId;
            RequirementTobeEdited.ProjectScheduleId = reqView.ProjectScheduleId;

            db.Entry(RequirementTobeEdited).State = EntityState.Modified;
            db.SaveChanges();

            List<RequirementSoftware> SoftwareList = db.RequirementSoftware.Where(r => r.RequirementDetailId == reqView.RequirementDetailId).ToList();
           
            foreach(var item in SoftwareList)
            {
                db.RequirementSoftware.Remove(item);
                db.SaveChanges();
            }

            foreach (var item in reqView.SoftwareId)
            {
                RequirementSoftware RequirementSoftware = new RequirementSoftware();
                RequirementSoftware.SoftwareId = item;
                RequirementSoftware.RequirementDetailId = reqView.RequirementDetailId;
                db.RequirementSoftware.Add(RequirementSoftware);
                db.SaveChanges();
            }

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetManageRecords(int reqDetailId)
        {
            List<ManageRequirement> ManageList = new List<ManageRequirement>();
            ManageList = db.ManageRequirement.Where(r => r.RequirementDetailId == reqDetailId).ToList();

            var records = ManageList.Select(c => new {
                Id = c.Id,
                JobHolderId = c.JobHolderId,
                JobHolderName = (db.JobHolders.Where(r => r.Id == c.JobHolderId).FirstOrDefault() == null)?  "" : db.JobHolders.Where(r => r.Id == c.JobHolderId).FirstOrDefault().Name ,
                RequirementDetailId = c.RequirementDetailId,
                CurrStatusId = c.CurrStatus,
                CurrStatus = c.CurrStatus.ToString(),
                TargetStatusId = c.TargetStatus,
                TargetStatus = c.TargetStatus.ToString(),
                AssignDate = c.AssignDate.Date.ToString("dd/MM/yyyy"),
                DeadLine = c.DeadLine.Date.ToString("dd/MM/yyyy"),
                HourlyCost = c.HourlyCost,
                TargetWorkhours = c.TargetWorkhours,
                WorkhoursConsumed = c.WorkhoursConsumed,
                CompTypeId = c.CompType,
                CompType = c.CompType.ToString(),
                CompDate = c.CompDate,
                Note = c.Note,
                DevNote = c.DevNote
            }).ToList();

            return Json(records, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        public JsonResult addRecord(DetailsRepForEditRecord record)
        {
            ManageRequirement jobHolderRecord = new ManageRequirement();
            jobHolderRecord.JobHolderId = record.JobHolderId;
            jobHolderRecord.RequirementDetailId = record.RequirementDetailId;
            if (record.CurrStatus == null) jobHolderRecord.CurrStatus = db.RequirementDetail.Where(i => i.Id == record.RequirementDetailId).FirstOrDefault().Status;
            else jobHolderRecord.CurrStatus = (Status) record.CurrStatus;
            jobHolderRecord.TargetStatus = record.TargetStatus;
            jobHolderRecord.AssignDate = record.AssignDate;
            jobHolderRecord.DeadLine = record.DeadLine;
            if (record.TargetWorkhours == 0) record.TargetWorkhours = record.WorkhoursConsumed;
            jobHolderRecord.TargetWorkhours = record.TargetWorkhours;
            jobHolderRecord.WorkhoursConsumed = record.WorkhoursConsumed;
            jobHolderRecord.CompType = record.CompType;
            jobHolderRecord.CompDate = record.CompDate;
            jobHolderRecord.Note = record.Note;
            jobHolderRecord.DevNote = record.DevNote;
            jobHolderRecord.HourlyCost = record.HourlyCost;

            db.ManageRequirement.Add(jobHolderRecord);
            db.SaveChanges();

            return Json(new { Id = jobHolderRecord.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult editRecord(DetailsRepForEditRecord record)
        {
            ManageRequirement jobHolderRecord = db.ManageRequirement.Find(record.Id);
            jobHolderRecord.JobHolderId = record.JobHolderId;
            jobHolderRecord.RequirementDetailId = record.RequirementDetailId;
            jobHolderRecord.CurrStatus = (Status) record.CurrStatus;
            jobHolderRecord.TargetStatus = record.TargetStatus;
            jobHolderRecord.AssignDate = record.AssignDate;
            jobHolderRecord.DeadLine = record.DeadLine;
            jobHolderRecord.TargetWorkhours = record.TargetWorkhours;
            jobHolderRecord.WorkhoursConsumed = record.WorkhoursConsumed;
            jobHolderRecord.CompType = record.CompType;
            jobHolderRecord.CompDate = record.CompDate;
            jobHolderRecord.Note = record.Note;
            jobHolderRecord.DevNote = record.DevNote;
            jobHolderRecord.HourlyCost = record.HourlyCost;

            db.Entry(jobHolderRecord).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Id = jobHolderRecord.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteRecord(int? Id)
        {
            ManageRequirement Record = db.ManageRequirement.Find(Id);
            db.ManageRequirement.Remove(Record);
            db.SaveChanges();

            return Json(new { Id = Record.Id }, JsonRequestBehavior.AllowGet);
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
