using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EMarkting.Models;
using PagedList;

namespace EMarkting.Controllers
{
    public class UserConController : Controller
    {

        private EMarkingTestEntities db = new EMarkingTestEntities();


        public ActionResult Index(int ? page)
        {
           


            return View(db.Categories.ToList().OrderByDescending(x=>x.Id).ToPagedList(page ?? 1, 9));
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            if(Session["User_Id"] !=null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        
        [HttpPost]
        public ActionResult SignUp(User_T user,HttpPostedFileBase Image)
        {
            string extention = "";
            if (ModelState.IsValid)
            {
                user.Password =
                    FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "SHA1");
                
                if (user.Image != null)
                {
                    extention = Path.GetExtension(Image.FileName);
                    if (extention.ToLower().Equals(".jpg") || extention.ToLower().Equals(".png") || extention.ToLower().Equals(".jpeg"))
                    {
                        // save PDF in folder
                        string PDFName = Path.GetFileName(Image.FileName);
                        string PDFName2 = DateTime.Now.ToString("yymmss") + PDFName;
                        string physicalPath = Server.MapPath("~/ImgUpload/" + PDFName2);
                        Image.SaveAs(physicalPath);
                        user.Image = PDFName2;
                    }
                    else
                    {
                        ModelState.AddModelError("ImgError", "Error");
                        return View(user);
                    }
                }
                else
                {
                    return View(user);
                }


                db.User_T.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // login
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["User_Id"] != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(User_T user)
        {
         
            if (ModelState.IsValid)
            { 
                var newPass =
             FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "SHA1");
            var val = db.User_T.Where(x => x.Email == user.Email && x.Password == newPass).FirstOrDefault();
                if(val !=null)
                {
                    Session["User_Id"] = val.Id;
                    return RedirectToAction("CreateAds");
                }

            }
            return View(user);
        }

        [HttpGet]
        public ActionResult CreateAds()
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.Cat_id = new SelectList(db.Categories.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateAds(Product product, HttpPostedFileBase Image)
        {
            string extention = "";
            if (ModelState.IsValid)
            {
                if (product.Image != null)
                {
                    extention = Path.GetExtension(Image.FileName);
                    if (extention.ToLower().Equals(".jpg") || extention.ToLower().Equals(".png") || extention.ToLower().Equals(".jpeg"))
                    {
                        // save PDF in folder
                        string PDFName = Path.GetFileName(Image.FileName);
                        string PDFName2 = DateTime.Now.ToString("yymmss") + PDFName;
                        string physicalPath = Server.MapPath("~/ImgUpload/" + PDFName2);
                        Image.SaveAs(physicalPath);
                        product.Image = PDFName2;
                    }
                    else
                    {
                        ModelState.AddModelError("ImgError", "Error");
                        return View(product);
                    }
                }
                else
                {
                    return View(product);
                }
                product.User_id =Convert.ToInt32(Session["User_Id"]);

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("index");
            }

            ViewBag.Cat_id = new SelectList(db.Categories.ToList(), "Id", "Name");
            return View(product);
        }

        // ads All Ads For This Category
        public ActionResult Ads(int ? id)
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View(db.Products.Where(x => x.Cat_id == id).ToList());
        }
 
        // view jsut one ads
        public ActionResult AdSpecfic(int id)
        {

            var prodct = db.Products.Where(x => x.Id == id).FirstOrDefault();
            var category = db.Categories.Where(x => x.Id == prodct.Cat_id).FirstOrDefault();
            var User = db.User_T.Where(x => x.Id == prodct.User_id).FirstOrDefault();

            ViewSpecifVM vM = new ViewSpecifVM();
            // product
            vM.Id_Product = prodct.Id;
            vM.Name_Product = prodct.Name;
            vM.Image_Product = prodct.Name;
            vM.Price_Product = prodct.Price;
            vM.Description_Product = prodct.Description;

            // category
            vM.Name_Category = category.Name;

            // user
            vM.Name_User = User.Name;
            vM.Image_User = User.Image;
            vM.Contact_User = User.Contact;

            return View(vM);
        }


        public ActionResult DeleteAd(string id)
        {
            if(Session["User_Id"] ==null)
            {
                return RedirectToAction("Index");
            }
            if(id==null)
            {
                return RedirectToAction("Index");
            }
            var val = db.Products.Where(x => x.Id == Convert.ToInt32(id)).FirstOrDefault();
            db.Products.Remove(val);
            db.SaveChanges();
            return View("Index");
        }


        public ActionResult SignOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index");
        }



    }
}