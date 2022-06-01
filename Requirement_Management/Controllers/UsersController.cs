using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Requirement_Management.DataAccess;
using Requirement_Management.Models;
using System.Web.Security;
using Requirement_Management.CustomAuthentication;

namespace Requirement_Management.Controllers
{
    [CustomAuthorize(Roles = "systemadmin, admin")]
    public class UsersController : Controller
    {
        private RequirementManagementContext db = new RequirementManagementContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.User.ToList());
        }

        //// GET: Users/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User user = db.User.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        // GET: Users/Registration
        [HttpGet]
        public ActionResult Registration()
        {
            ViewBag.RoleId = db.Role.Select(r => new SelectListItem()
            {
                Text = r.RoleName,
                Value = r.RoleId.ToString()
            });
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegistrationView registrationView)
        {
            bool statusRegistration = false;
            string messageRegistration = string.Empty;

            if (ModelState.IsValid)
            {
                // Email Verification
                string userName = Membership.GetUserNameByEmail(registrationView.Email);
                if (!string.IsNullOrEmpty(userName))
                {
                    ViewBag.Message = "Sorry: Email already Exists";
                    ViewBag.RoleId = db.Role.Select(r => new SelectListItem()
                    {
                        Text = r.RoleName,
                        Value = r.RoleId.ToString()
                    });
                    return View(registrationView);
                }

                var user1 = db.User.Where(r => r.Username == registrationView.Username).FirstOrDefault();
                if (user1 != null)
                {
                    ViewBag.Message = "Sorry: Username already Exists";
                    ViewBag.RoleId = db.Role.Select(r => new SelectListItem()
                    {
                        Text = r.RoleName,
                        Value = r.RoleId.ToString()
                    });
                    return View(registrationView);
                }

                //Save User Data
                using (RequirementManagementContext dbContext = new RequirementManagementContext())
                {
                    var user = new User()
                    {
                        Username = registrationView.Username,
                        FirstName = registrationView.FirstName,
                        LastName = registrationView.LastName,
                        Email = registrationView.Email,
                        Password = registrationView.Password,
                        ActivationCode = Guid.NewGuid(),
                    };
                    user.IsActive = true;
                    user.Roles = dbContext.Role.Where(r => r.RoleId == registrationView.RoleId).ToList();
                    dbContext.User.Add(user);
                    dbContext.SaveChanges();
                }
                //Verification Email
                //VerificationEmail(registrationView.Email, registrationView.ActivationCode.ToString());
                //messageRegistration = "Your account has been created successfully. ^_^";
                //statusRegistration = true;

                return RedirectToAction("Index");
            }
            else
            {
                messageRegistration = "Something Wrong!";
            }
            ViewBag.Message = messageRegistration;
            ViewBag.Status = statusRegistration;
            ViewBag.RoleId = db.Role.Select(r => new SelectListItem()
            {
                Text = r.RoleName,
                Value = r.RoleId.ToString()
            });
            return View(registrationView);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            RegistrationView regView = new RegistrationView();
            regView.UserId = user.UserId;
            regView.Username = user.Username;
            regView.FirstName = user.FirstName;
            regView.LastName = user.LastName;
            regView.Email = user.Email;
            regView.ActivationCode = Guid.NewGuid();
            regView.Password = user.Password;
            regView.ConfirmPassword = user.Password;
            regView.IsActive = user.IsActive;
            regView.RoleId = user.Roles.FirstOrDefault().RoleId;

            ViewBag.RoleId = db.Role.Select(r => new SelectListItem()
            {
                Text = r.RoleName,
                Value = r.RoleId.ToString()
            });

            return View(regView);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegistrationView regView)
        {
            
            if (ModelState.IsValid)
            {
                //string userName = Membership.GetUserNameByEmail(regView.Email);
                var user1 = db.User.Where(r => r.Email == regView.Email).FirstOrDefault();
                if (user1 != null && user1.UserId != regView.UserId)
                {
                    ViewBag.Message = "Sorry: Email already Exists";
                    ViewBag.RoleId = db.Role.Select(r => new SelectListItem()
                    {
                        Text = r.RoleName,
                        Value = r.RoleId.ToString()
                    });
                    return View(regView);
                }
                
                user1 = db.User.Where(r => r.Username == regView.Username).FirstOrDefault();
                if (user1 != null && user1.UserId != regView.UserId)
                {
                    ViewBag.Message = "Sorry: Username already Exists";
                    ViewBag.RoleId = db.Role.Select(r => new SelectListItem()
                    {
                        Text = r.RoleName,
                        Value = r.RoleId.ToString()
                    });
                    return View(regView);
                }

                db.Database.ExecuteSqlCommand("delete from [UserRoles] where UserId = " + regView.UserId);
                
                User user = db.User.Find(regView.UserId);
                user.UserId = regView.UserId;
                user.Username = regView.Username;
                user.FirstName = regView.FirstName;
                user.LastName = regView.LastName;
                user.Email = regView.Email;
                user.ActivationCode = regView.ActivationCode;
                user.Password = regView.Password;
                user.IsActive = true;
                user.Roles = db.Role.Where(r => r.RoleId == regView.RoleId).ToList();

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = db.Role.Select(r => new SelectListItem()
            {
                Text = r.RoleName,
                Value = r.RoleId.ToString()
            });
            return View(regView);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
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
