using Requirement_Management.CustomAuthentication;
using Requirement_Management.Models;
using Requirement_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Requirement_Management.Controllers
{
    [CustomAuthorize(Roles = "systemadmin, admin")]
    public class ReportsController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListReport()
        {
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            ListReportView ListReport = new ListReportView();
            return View(ListReport);
        }


        [HttpPost]
        public ActionResult ListReport(ListReportView ListReport)
        {
            string sql = @"SELECT   [Title],
		                            [Project].[StartDate],
		                            [CompanyId],
		                            [ClientCompany].[Name] as CompanyName
		                            FROM (([RequirementManagement].[dbo].[Project] 
                                    INNER JOIN ProjectSchedule ON ProjectSchedule.ProjectId = Project.Id)
                                    INNER JOIN ClientCompany ON ClientCompany.Id = Project.CompanyId)
                                    WHERE 1 = 1 ";

            //if (StatusReport.From != null)
            //{
            //    manageReqsql += " AND AssignDate >= '" + StatusReport.From + "'";
            //}
            //if (StatusReport.To != null)
            //{
            //    manageReqsql += " AND AssignDate < '" + StatusReport.To.Value.AddDays(1) + "'";
            //}

            if (ListReport.CompanyId != null)
            {
                sql += " AND CompanyId = " + ListReport.CompanyId;
            }
            if (ListReport.ProjectId != null)
            {
                sql += " AND Project.Id = " + ListReport.ProjectId;
            }
            if (ListReport.Mode == 0)
            {
                sql += " AND ProjectMode = " + 0;
            }
            else if (ListReport.Mode > 0) sql += " AND ProjectMode != " + 0;

            sql += " ORDER BY [StartDate] DESC";
            ListReport.ListReport = db.Database.SqlQuery<ProjectListReportView>(sql).ToList();

            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            return View(ListReport);
        }

        public ActionResult StatusReport()
        {
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");
            StatusReportView StatusReport = new StatusReportView();
            return View(StatusReport);
        }


        [HttpPost]
        public ActionResult StatusReport(StatusReportView StatusReport)
        {
            string sql = @"SELECT   Project.Id,
                                    Title,
                                    StartDate,
                                    CompanyId
                                    FROM ((([RequirementManagement].[dbo].[Project]
                                    INNER JOIN ProjectSoftware ON ProjectSoftware.ProjectId = Project.Id)
                                    INNER JOIN Software ON Software.Id = ProjectSoftware.SoftwareId)
                                    INNER JOIN SoftwareCategory ON SoftwareCategory.Id = Software.Software_CategoryId)
                                    WHERE 1 = 1 ";

            if (StatusReport.CategoryId != null) sql += " AND SoftwareCategory.Id = " + StatusReport.CategoryId;

            sql += " Group By Project.Id, Title, StartDate, CompanyId ";

            var ProList = db.Database.SqlQuery<Project>(sql).ToList();

            string manageReqsql = @"SELECT  [RequirementDetailId],
		                                    [AssignDate],
		                                    [TargetWorkhours],
		                                    [WorkhoursConsumed],
		                                    [CompDate],
		                                    CASE WHEN ((CompDate IS NULL) AND [TargetWorkhours] - [WorkhoursConsumed] > 0) THEN [TargetWorkhours] - [WorkhoursConsumed] 
		                                         ELSE 0 
                                            END AS PendingWorkhours
		                                    FROM [RequirementManagement].[dbo].[ManageRequirement] WHERE 1 = 1 ";

            if (StatusReport.From != null)
            {
                manageReqsql += " AND AssignDate >= '" + StatusReport.From + "'";
            }
            if (StatusReport.To != null)
            {
                manageReqsql += " AND AssignDate < '" + StatusReport.To.Value.AddDays(1) + "'";
            }

            sql = @"SELECT  Project.Id,
                            [CompanyId],
	                        [Title] as ProjectName,
	                        SUM([TargetWorkhours]) as TargetWorkhours,
	                        SUM([WorkhoursConsumed]) as WorkhoursConsumed,
	                        SUM([PendingWorkhours]) as PendingWorkhours
                            FROM (([RequirementManagement].[dbo].[Project]
                            INNER JOIN RequirementDetail ON Project.Id = RequirementDetail.ProjectId)
                            INNER JOIN (" + manageReqsql + @") AS ManageRequirement ON RequirementDetail.Id = ManageRequirement.RequirementDetailId)
                            WHERE 1=1 ";
            
            if (StatusReport.CompanyId != null)
            {
                sql += " AND CompanyId = " + StatusReport.CompanyId;
            }
            if (StatusReport.ProjectId != null)
            {
                sql += " AND Project.Id = " + StatusReport.ProjectId;
            }

            if (ProList.Any())
            {
                sql += " AND (";

                foreach (var item in ProList)
                {
                    sql += "Project.Id = " + item.Id + " OR ";
                }
                sql = sql.Remove(sql.Length - 1, 1);
                sql = sql.Remove(sql.Length - 1, 1);
                sql = sql.Remove(sql.Length - 1, 1);
                sql += ") ";
            }

            sql += " GROUP BY [Title], Project.Id, Project.CompanyId";
            StatusReport.ProjectDetail = db.Database.SqlQuery<ProjectReportView>(sql).ToList();

            if (StatusReport.From.HasValue)
            {
                StatusReport.Fromdate += StatusReport.From.Value.ToString("yyyy-MM-dd");
            }
            if (StatusReport.To.HasValue)
            {
                StatusReport.Todate += StatusReport.To.Value.ToString("yyyy-MM-dd");
            }

            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");

            return View(StatusReport);
        }

        public ActionResult DeveloperwiseReport()
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
            ViewBag.JobHolderId = new SelectList(db.JobHolders.Where(g => g.isActive == true), "Id", "Name");
            var ReqMode = from RequirementMode e in Enum.GetValues(typeof(RequirementMode))
                          select new
                          {
                              Id = (int)e,
                              Name = e.ToString()
                          };
            ViewBag.ReqMode = new SelectList(ReqMode, "Id", "Name");

            DeveloperwiseReportView DevRepView = new DeveloperwiseReportView();
            return View(DevRepView);
        }


        [HttpPost]
        public ActionResult DeveloperwiseReport(DeveloperwiseReportView DevRepView)
        {
            string reqdetailsql = @"SELECT  [RequirementDetail].[Id],
                                            [Title]
        		                            FROM (([RequirementManagement].[dbo].[RequirementDetail]
		                                    INNER JOIN Project ON Project.Id = RequirementDetail.ProjectId)
		                                    INNER JOIN RequirementSoftware ON RequirementSoftware.RequirementDetailId = RequirementDetail.Id)
		                                    WHERE 1=1 ";
            
            if (DevRepView.Status >= 0)
            {
                int status = (int)DevRepView.Status;
                reqdetailsql += "and Status = " + status;
            }
            if (DevRepView.ReqMode >= 0)
            {
                int reqmode = (int)DevRepView.ReqMode;
                reqdetailsql += "and ReqMode = " + reqmode;
            }
            if (DevRepView.ReqTypeId != null)
            {
                reqdetailsql += "and ReqTypeId = " + DevRepView.ReqTypeId;
            }
            if (DevRepView.CompanyId != null)
            {
                reqdetailsql += "and CompanyId = " + DevRepView.CompanyId;
            }
            if (DevRepView.ReqProviderId != null)
            {
                reqdetailsql += "and ReqProviderId = " + DevRepView.ReqProviderId;
            }
            if (DevRepView.ProjectId != null)
            {
                reqdetailsql += "and ProjectId = " + DevRepView.ProjectId;
            }
            if (DevRepView.ProjectScheduleId != null)
            {
                reqdetailsql += "and ProjectScheduleId = " + DevRepView.ProjectScheduleId;
            }
            if (DevRepView.CategoryId != null)
            {
                reqdetailsql += "and SoftCategoryId = " + DevRepView.CategoryId;
            }
            if (DevRepView.Priority >= 0)
            {
                int priority = (int)DevRepView.Priority;
                reqdetailsql += "and Priority = " + priority;
            }
            if (DevRepView.SoftwareId.Any())
            {
                reqdetailsql += " AND (";

                foreach (var software in DevRepView.SoftwareId)
                {
                    reqdetailsql += "SoftwareId = " + software + " OR ";
                }
                reqdetailsql = reqdetailsql.Remove(reqdetailsql.Length - 1, 1);
                reqdetailsql = reqdetailsql.Remove(reqdetailsql.Length - 1, 1);
                reqdetailsql = reqdetailsql.Remove(reqdetailsql.Length - 1, 1);
                reqdetailsql += ") ";
            }
            
            reqdetailsql += " GROUP BY [RequirementDetail].[Id], [Title]";

            string managereqsql = @"SELECT  [RequirementDetailId],
                                            [Title],
                                            [Name],
		                                    [JobHolderId],
		                                    [AssignDate],
		                                    [TargetWorkhours],
		                                    [WorkhoursConsumed],
		                                    [CompDate],
		                                    CASE WHEN ((CompDate IS NULL) AND [TargetWorkhours] - [WorkhoursConsumed] > 0) THEN [TargetWorkhours] - [WorkhoursConsumed] 
			                                    ELSE 0 
		                                    END AS PendingWorkhours
		                                    FROM (([RequirementManagement].[dbo].[ManageRequirement]
		                                    INNER JOIN JobHolder ON JobHolder.Id = ManageRequirement.JobHolderId)
		                                    INNER JOIN (" + reqdetailsql + @") AS RequirementDetail ON [RequirementDetail].[Id] = [ManageRequirement].[RequirementDetailId])
		                                    WHERE 1=1 ";

            if (DevRepView.From != null)
            {
                managereqsql += "and AssignDate >= '" + DevRepView.From + "'";
            }
            if (DevRepView.To != null)
            {
                managereqsql += "and AssignDate < '" + DevRepView.To.AddDays(1) + "'";
            }
            if (DevRepView.JobHolderId != null)
            {
                managereqsql += "and JobHolderId = " + DevRepView.JobHolderId;
            }

            string sql = @"SELECT	[Name] as [JobHolderName],
		                            [JobHolderId],
                                    [Title] as [ProjectName],
		                            SUM([TargetWorkhours]) as [TargetWorkhours],
		                            SUM([WorkhoursConsumed]) as [WorkhoursConsumed],
                                    SUM([PendingWorkhours]) as [PendingWorkhours]
		                            FROM (" + managereqsql + @") AS ManageRequirement GROUP BY [Name], [JobHolderId], [Title]
                                    ORDER BY [JobHolderId]";

            DevRepView.DevReportTable = db.Database.SqlQuery<DeveloperwiseReportViewTable>(sql).ToList();
            
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
            ViewBag.JobHolderId = new SelectList(db.JobHolders.Where(g => g.isActive == true), "Id", "Name");
            var ReqMode = from RequirementMode e in Enum.GetValues(typeof(RequirementMode))
                          select new
                          {
                              Id = (int)e,
                              Name = e.ToString()
                          };
            ViewBag.ReqMode = new SelectList(ReqMode, "Id", "Name");
            return View(DevRepView);
        }

        public ActionResult RequirementReport()
        {
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");

            RequirementReportView StatusReport = new RequirementReportView();
            StatusReport.Date = "";
            StatusReport.CompanyName = "";
            return View(StatusReport);
        }

        [HttpPost]
        public ActionResult RequirementReport(RequirementReportView ReqReport)
        {
            string manageReqsql = @"SELECT  [Requirement].[Id]
                                            ,[Project].[Title] as ProjectName
                                            ,[Date]
                                            ,[ClientCompany].[Id]
					                        ,[ClientCompany].[Name] as CompanyName
                                            ,[RequirementDetail].[ProjectId]
	                                        ,[ProjectMode]
	                                        ,[Requirement]
					                        ,[RequirementProvider].[Name] as ProviderName
	                                        ,[RequirementDetail].[Status]
	                                        ,[RequirementType].[Name] as ReqTypeName
                                            ,[Priority]
                                        FROM (((((([RequirementManagement].[dbo].[Requirement]
                                        INNER JOIN RequirementDetail ON RequirementDetail.ReqId = Requirement.Id)
				                        INNER JOIN RequirementProvider ON RequirementProvider.Id = RequirementDetail.ReqProviderId)
				                        INNER JOIN RequirementType ON RequirementType.Id = RequirementDetail.ReqTypeId)
				                        INNER JOIN Project ON Project.Id = RequirementDetail.ProjectId)
                                        INNER JOIN ProjectSchedule ON ProjectSchedule.ProjectId = RequirementDetail.ProjectId)
				                        INNER JOIN ClientCompany ON ClientCompany.Id = [Project].[CompanyId])
				                        WHERE 1=1 ";

            if (ReqReport.From != null)
            {
                manageReqsql += " AND Date >= '" + ReqReport.From + "'";
            }
            if (ReqReport.To != null)
            {
                manageReqsql += " AND Date < '" + ReqReport.To.AddDays(1) + "'";
            }

            if (ReqReport.CompanyId != null)
            {
                manageReqsql += " AND ClientCompany.Id = " + ReqReport.CompanyId;
            }
            if (ReqReport.ProjectId != null)
            {
                manageReqsql += " AND Project.Id = " + ReqReport.ProjectId;
            }
            if (ReqReport.CategoryId != null)
            {
                manageReqsql += "and SoftCategoryId = " + ReqReport.CategoryId;
            }
            if (ReqReport.Mode == 0)
            {
                manageReqsql += " AND ProjectMode = " + 0;
            }
            else if(ReqReport.Mode > 0)  manageReqsql += " AND ProjectMode != " + 0;


            manageReqsql += " ORDER BY [Requirement].[Date] DESC";
            ReqReport.ReqReport = db.Database.SqlQuery<RequirementStatusReportView>(manageReqsql).ToList();

            if (ReqReport.ReqReport.Any()) {
                ReqReport.Date = ReqReport.ReqReport[0].Date.ToString("dd/MM/yyyy");
                ReqReport.CompanyName = ReqReport.ReqReport[0].CompanyName;

                foreach(var item in ReqReport.ReqReport)
                {
                    if (item.Date.ToString("dd/MM/yyyy") != ReqReport.Date) ReqReport.Date = "";
                    if (item.CompanyName != ReqReport.CompanyName) ReqReport.CompanyName = "";
                }
            }
            else
            {
                ReqReport.Date = "";
                ReqReport.CompanyName = "";
            }
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");

            return View(ReqReport);
            
        }

        public ActionResult StatuswiseRequirementReport()
        {
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");

            StatuswiseRequirementReportView StatusReport = new StatuswiseRequirementReportView();
            StatusReport.Date = "";
            StatusReport.CompanyName = "";
            return View(StatusReport);
        }

        [HttpPost]
        public ActionResult StatuswiseRequirementReport(StatuswiseRequirementReportView ReqReport)
        {
            string manageReqsql = @"SELECT  [Requirement].[Id]
                                            ,[Project].[Title] as ProjectName
                                            ,[Date]
                                            ,[ClientCompany].[Id]
					                        ,[ClientCompany].[Name] as CompanyName
                                            ,[RequirementDetail].[ProjectId]
	                                        ,[ProjectMode]
	                                        ,[Requirement]
					                        ,[RequirementProvider].[Name] as ProviderName
	                                        ,[RequirementDetail].[Status]
	                                        ,[RequirementType].[Name] as ReqTypeName
                                            ,[Priority]
                                        FROM (((((([RequirementManagement].[dbo].[Requirement]
                                        INNER JOIN RequirementDetail ON RequirementDetail.ReqId = Requirement.Id)
				                        INNER JOIN RequirementProvider ON RequirementProvider.Id = RequirementDetail.ReqProviderId)
				                        INNER JOIN RequirementType ON RequirementType.Id = RequirementDetail.ReqTypeId)
				                        INNER JOIN Project ON Project.Id = RequirementDetail.ProjectId)
                                        INNER JOIN ProjectSchedule ON ProjectSchedule.ProjectId = RequirementDetail.ProjectId)
				                        INNER JOIN ClientCompany ON ClientCompany.Id = [Project].[CompanyId])
				                        WHERE 1=1 ";

            if (ReqReport.From != null)
            {
                manageReqsql += " AND Date >= '" + ReqReport.From + "'";
            }
            if (ReqReport.To != null)
            {
                manageReqsql += " AND Date < '" + ReqReport.To.AddDays(1) + "'";
            }

            if (ReqReport.CompanyId != null)
            {
                manageReqsql += " AND ClientCompany.Id = " + ReqReport.CompanyId;
            }
            if (ReqReport.ProjectId != null)
            {
                manageReqsql += " AND Project.Id = " + ReqReport.ProjectId;
            }
            if (ReqReport.Mode == 0)
            {
                manageReqsql += " AND ProjectMode = " + 0;
            }
            else if (ReqReport.Mode > 0) manageReqsql += " AND ProjectMode != " + 0;
            if (ReqReport.CategoryId != null)
            {
                manageReqsql += "and SoftCategoryId = " + ReqReport.CategoryId;
            }
            if (ReqReport.ReqStatus != null)
            {
                if (ReqReport.ReqStatus == 0)
                {
                    manageReqsql += "and RequirementDetail.Status = 0";
                }
                else if ((int)ReqReport.ReqStatus == 1)
                {
                    manageReqsql += "and RequirementDetail.Status > 0 and RequirementDetail.Status <= 4 ";
                }
                else if ((int)ReqReport.ReqStatus == 2)
                {
                    manageReqsql += "and RequirementDetail.Status > 4";
                }
            }

            manageReqsql += " ORDER BY [Requirement].[Date] DESC";
            ReqReport.ReqReport = db.Database.SqlQuery<ListOfStatuswiseRequirementReportView>(manageReqsql).ToList();

            if (ReqReport.ReqReport.Any())
            {
                ReqReport.Date = ReqReport.ReqReport[0].Date.ToString("dd/MM/yyyy");
                ReqReport.CompanyName = ReqReport.ReqReport[0].CompanyName;

                foreach (var item in ReqReport.ReqReport)
                {
                    if (item.Date.ToString("dd/MM/yyyy") != ReqReport.Date) ReqReport.Date = "";
                    if (item.CompanyName != ReqReport.CompanyName) ReqReport.CompanyName = "";
                }
            }
            else
            {
                ReqReport.Date = "";
                ReqReport.CompanyName = "";
            }
            ViewBag.CompanyId = new SelectList(db.ClientCompany, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.SoftwareCategory, "Id", "Name");

            return View(ReqReport);

        }

    }
}