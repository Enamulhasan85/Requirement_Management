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
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");

            IndexView requirement = new IndexView();
            DateTime To = requirement.To.AddDays(1);
            requirement.Req = db.Requirement.Where(r => r.EntryDate >= requirement.From && r.EntryDate < To ).ToList();
            return View(requirement);
        }

        // POST: Requirements
        [HttpPost]
        public ActionResult Index(IndexView requirement)
        {
            string sql = "select Requirement.Id, Requirement.Title, Requirement.Date, Requirement.EntryDate, Requirement.CompanyId, Requirement.ReqProviderId from Requirement where 1=1";

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
            if (requirement.ReqProviderId != null)
            {
                sql += "and ReqProviderId = '" + requirement.ReqProviderId + "'";
            }

            requirement.Req = db.Database.SqlQuery<Requirement>(sql).ToList();

            foreach (var row in requirement.Req)
            {
                row.Company = db.ClientCompany.Where(r => r.Id == row.CompanyId).FirstOrDefault();
                row.ReqProvider = db.RequirementProvider.Where(r => r.Id == row.ReqProviderId).FirstOrDefault();
            }

            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            //requirement.Req = db.Requirement.Where(r => r.EntryDate.Day == DateTime.Now.Day && r.EntryDate.Month == DateTime.Now.Month && r.EntryDate.Year == DateTime.Now.Year).ToList();
            return View(requirement);
        }
        // GET: Requirements/Reports/5
        [CustomAuthorize(Roles = "systemadmin, admin")]
        public ActionResult DetailsReports()
        {
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.SoftwareId = new MultiSelectList("", "Id", "Name");

            ReportView DetailReport = new ReportView();
            return View(DetailReport);
        }

        [CustomAuthorize(Roles = "systemadmin, admin")]
        [HttpPost]
        public ActionResult DetailsReports(ReportView detailReport)
        {
            string sql = "select RequirementDetail.Id, RequirementDetail.ReqTypeId, RequirementDetail.Description, RequirementDetail.Requirement, RequirementDetail.Status, RequirementDetail.StarMarked, Requirement.Date, Requirement.CompanyId, Requirement.ReqProviderId, RequirementDetail.SoftCategoryId, RequirementDetail.Workdays from RequirementDetail INNER JOIN Requirement ON Requirement.Id = RequirementDetail.ReqId where 1=1";
            if (detailReport.From != null)
            {
                sql += "and Date >= '" + detailReport.From + "'";
            }
            if (detailReport.To != null)
            {
                sql += "and Date < '" + detailReport.To.AddDays(1) + "'";
            }
            if (detailReport.StarMarked != null)
            {
                int starmark = (detailReport.StarMarked) ? 1 : 0;
                sql += "and StarMarked = " + starmark;
            }
            if (detailReport.Status >= 0)
            {
                int status = (int)detailReport.Status;
                sql += "and Status = " + status;
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
            if (detailReport.CategoryId != null)
            {
                sql += "and SoftCategoryId = " + detailReport.CategoryId;
            }

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

                if(detailReport.SoftwareId != null)
                {
                    foreach (var software in reqSoftware)
                    {
                        if (detailReport.SoftwareId.Contains(Int32.Parse(""+software.SoftwareId)))
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
                    if (row.SoftCategoryId != null) row.SoftCategoryName = db.SoftwareCategory.Find(row.SoftCategoryId).Name;
                }
                else Report.ReqDetail.Remove(row);

            }
            Report.From = detailReport.From;
            Report.To = detailReport.To;
            Report.ReqProviderId = detailReport.ReqProviderId;
            Report.CompanyId = detailReport.CompanyId;
            Report.ReqTypeId = detailReport.ReqTypeId;
            Report.StarMarked = (detailReport.StarMarked) ? true : false;
            Report.Status = detailReport.Status;
            Report.SoftwareId = detailReport.SoftwareId;

            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.SoftwareId = new MultiSelectList("", "Id", "Name");
            return View(Report);
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
            reqView.ReqProviderId = requirement.ReqProviderId;
            if (requirement.ReqProviderId != null) reqView.ReqProviderName = requirement.ReqProvider.Name;

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
                reqdetail.Workdays = info.Workdays;
                reqdetail.ReqTypeId = info.ReqTypeId;
                if (info.ReqTypeId != null) reqdetail.ReqTypeName = info.ReqType.Name;
                reqdetail.ReqId = requirement.Id;
                reqdetail.Status = info.Status;
                reqdetail.StarMarked = info.StarMarked;
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

        // GET: Requirements/Create
        public ActionResult Create()
        {
            //ViewBag.Status = new SelectList()     
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name");
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            ViewBag.SoftwareId = new MultiSelectList("", "Id", "Name");
            //ViewBag.SoftwareId = new SelectList(db.Software, "Id", "Name");
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
            requirement.ReqProviderId = reqView.ReqProviderId;

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
                row.Requirement = info.Requirement;
                row.Description = info.Description;
                row.ReqTypeId = info.ReqTypeId;
                row.ReqId = requirement.Id;
                row.Status = info.Status;
                row.StarMarked = info.StarMarked;
                row.SoftCategoryId = info.SoftCategoryId;
                row.Workdays = 0;
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
                reqdetail.Requirement = info.Requirement;
                reqdetail.Description = info.Description;
                reqdetail.ReqTypeId = info.ReqTypeId;
                if (info.ReqTypeId != null) reqdetail.ReqTypeName = info.ReqType.Name;
                reqdetail.ReqId = requirement.Id;
                reqdetail.Status = info.Status;
                reqdetail.StarMarked = info.StarMarked;
                reqdetail.SoftCategoryId = info.SoftCategoryId;
                if (info.SoftCategoryId != null)  reqdetail.SoftCategoryName = info.SoftCategory.Name;
                reqdetail.Workdays = info.Workdays;

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
            ViewBag.ReqProviderId = new SelectList(db.RequirementProvider, "Id", "Name", requirement.ReqProviderId);
            ViewBag.ReqTypeId = new SelectList(db.RequirementType, "Id", "Name");
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
            requirement.ReqProviderId = reqView.ReqProviderId;

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
            reqdetail.Requirement = reqdetailView.Requirement;
            reqdetail.Description = reqdetailView.Description;
            reqdetail.ReqTypeId = reqdetailView.ReqTypeId;
            reqdetail.ReqId = reqdetailView.ReqId;
            reqdetail.Status = reqdetailView.Status;
            reqdetail.StarMarked = reqdetailView.StarMarked;
            reqdetail.SoftCategoryId = reqdetailView.SoftCategoryId;
            reqdetail.Workdays = reqdetailView.Workdays;

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
            reqdetail.SoftCategoryId = reqdetailView.SoftCategoryId;
            reqdetail.Workdays = reqdetailView.Workdays;

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

        public JsonResult GetSoftwares(int? Id)
        {
            List<Software> Softwares = db.Software.Where(i => i.Software_CategoryId == Id).ToList();
            var softwares = Softwares.Select(c => new {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return Json(softwares, JsonRequestBehavior.AllowGet);
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
