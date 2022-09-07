using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EMarkting.Models;
using PagedList;

namespace EMarkting.Controllers
{
    public class AdminController : Controller
    {
        private EMarkingTestEntities db = new EMarkingTestEntities();


        [HttpGet]
        public ActionResult Login()
        {
           if( Session["admin_Id"] !=null)
            {
                return RedirectToAction("Index","UserCon");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin admin)
        {
         
            var val = db.Admins.Where(x => x.UserName == admin.UserName && x.Password == admin.Password).FirstOrDefault();
            if(val !=null)
            {
                Session["admin_Id"] = val.id;
                return RedirectToAction("Create");
            }
            else
            {
                ViewBag.error = "Error Username Or Password";
            }
            return View(admin);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if(Session["admin_Id"]==null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category category , HttpPostedFileBase Image)
        {
            string extention = "";
            if (ModelState.IsValid)
            {
                if(category.Image !=null)
                {
                    extention = Path.GetExtension(Image.FileName);
                    if (extention.ToLower().Equals(".jpg") || extention.ToLower().Equals(".png") || extention.ToLower().Equals(".jpeg"))
                    {
                        // save PDF in folder
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

                //Category MyCat = new Category();
                //MyCat.Name = category.Name ;
                //MyCat.Image = category.Image;
                //MyCat.Admin_Id = Convert.ToInt32(Session["admin_Id"].ToString());
                //MyCat.Status = true;

                //db.Categories.Add(MyCat);
                category.Admin_Id = Convert.ToInt32(Session["admin_Id"].ToString());
                category.Status = true;
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(category);
         }

        //
        public ActionResult ViewCategory(int ?page)
        {
            if(Session["admin_Id"] !=null)
            {
                return View(db.Categories.ToList().ToPagedList(page ?? 1, 3) );
            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }



     


     }
}