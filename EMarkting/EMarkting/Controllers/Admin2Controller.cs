using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EMarkting.Models;
using PagedList;

namespace EMarkting.Controllers
{
    public class Admin2Controller : Controller
    {
        private EMarkingTestEntities db;

        public Admin2Controller()
        {
            db = new EMarkingTestEntities();
        }

        public ActionResult Login()
        {
            if(Session["Admin_Id"] !=null  && Session["Admin_Name"] !=null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult  Login(Admin admin)
        {
            if(ModelState.IsValid)
            {

                var result = db.Admins.Where(x => x.UserName == admin.UserName && x.Password == admin.Password).FirstOrDefault();
                if(result !=null)
                {
                    Session["Admin_Id"] = result.id;
                    Session["Admin_Name"] = result.UserName;
                    return RedirectToAction("Index");

                }
            }

            return View(admin);
        }


        [HttpGet]
        public ActionResult CreateCategory()
        {
            if(Session["Admin_Id"] ==null &&  Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category category , HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var extention = "";
                if (category.Image != null)
                {
                    extention = Path.GetExtension(Image.FileName);
                    if (extention.ToLower().Equals(".jpg") || extention.ToLower().Equals(".png") || extention.ToLower().Equals(".jpeg"))
                    {
                        // save Image in folder
                        string PDFName = Path.GetFileName(Image.FileName);
                        string PDFName2 = DateTime.Now.ToString("yymmss") + PDFName;
                        string physicalPath = Server.MapPath("~/ImgUpload/" + PDFName2);
                        Image.SaveAs(physicalPath);
                        category.Image = PDFName2;
                    }
                    else
                    {
                        ModelState.AddModelError("ImgError", "Error");
                        return View(category);
                    }
                }
                else
                {
                    return View(category);
                }
                category.Admin_Id =  Convert.ToInt32( Session["Admin_Id"].ToString() );
                category.Status = true;
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }



        public ActionResult ViewAllCategory(int? page )
        {

            if (Session["Admin_Id"] == null && Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login");
            }

            var result = db.Categories.ToList().ToPagedList(page ?? 1, 3) ;

            return View(result);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ViewCategory");
            }

            var Result = db.Categories.Find(id);

            db.Categories.Remove(Result);
            db.SaveChanges();
            Thread.Sleep(2000);
            return RedirectToAction("ViewAllCategory");
        }




        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            Thread.Sleep(2000);
            return RedirectToAction("Index");
        }

    }
}