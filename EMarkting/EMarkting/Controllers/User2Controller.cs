using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using EMarkting.Models;

namespace EMarkting.Controllers
{
    public class User2Controller : Controller
    {

        private EMarkingTestEntities db;


        public User2Controller()
        {
            db = new EMarkingTestEntities();
        }

        // GET: User2
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult LoginUser()
        {
            if(Session["User_Id"] != null && Session["User_Name"] !=null )
            {
                return RedirectToAction("Index");
            }

            return View();
        }


        [HttpPost]
        public ActionResult LoginUser( User_T user)
        {
            if (ModelState.IsValid)
            {
                var result = db.User_T.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();

                Session["User_Id"] = result.Id;
                Session["User_Name"] = result.Name;

                return RedirectToAction("Index");
            }

            return View(user);
        }

      





        [HttpGet]
        public ActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(User_T user, HttpPostedFileBase Image)
        {
            string extention = "";

            if (ModelState.IsValid)
            {
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
                return RedirectToAction("LoginUser");

            }

            return View(user);
        }



        [HttpGet]
        public ActionResult CreateProduct()
        {
            if (Session["User_Id"] == null && Session["User_Name"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Cat_id = new SelectList( db.Categories.ToList() , "Id", "Name");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product product, HttpPostedFileBase Image)
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
                        ViewBag.Cat_id = new SelectList(db.Categories.ToList(), "Id", "Name",product.Cat_id);
                        return View(product);
                    }
                }
                else
                {
                    return View(product);
                }
                product.User_id = Convert.ToInt32(Session["User_Id"].ToString());
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Cat_id = new SelectList(db.Categories.ToList(), "Id", "Name", product.Cat_id);
            return View(product);
        }







        public ActionResult CategoriesList()
        {
            if ( Session["User_Id"] == null && Session["User_Name"] == null )
            {
                return RedirectToAction("Index");
            }
            var result = db.Categories.ToList();
            return View(result);
        }



        public ActionResult ProductList()
        {
            if (Session["User_Id"] == null && Session["User_Name"] == null)
            {
                return RedirectToAction("Index");
            }
            var result = db.Products.ToList();
            return View(result);
        }


        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ProductList");
            }

            var Result = db.Products.Find(id);

            db.Products.Remove(Result);
            db.SaveChanges();
            Thread.Sleep(2000);
            return RedirectToAction("ProductList");
        }




    }
}