using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
     
        public ActionResult Error()
        {
            return View();
        }
        // GET: Products
        public ActionResult Index(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            try
            {
                Log.Info("Product Index Visible");
                /*var products = db.Products.Include(p => p.Category);
                return View(products.ToList());*/

                var products = db.Products.Include(p => p.Category);
                if (Search_Data != null)
                {
                    Page_No = 1;
                }
                else
                {
                    Search_Data = Filter_Value;
                }

                ViewBag.FilterValue = Search_Data;
                if (!String.IsNullOrEmpty(Search_Data))

                {
                    products = products.Where(p => p.Name.ToUpper().Contains(Search_Data.ToUpper())
                        || p.Category.CategoryName.ToUpper().Contains(Search_Data.ToUpper()));
                }
                ViewBag.CurrentSortOrder = Sorting_Order;
                ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name" : "";
                ViewBag.SortingDate = Sorting_Order == "Date_Enroll" ? "Category_dec" : "Category";

                switch (Sorting_Order)
                {
                    case "Name":
                        products = products.OrderByDescending(p => p.Name);
                        break;
                    case "Category":
                        products = products.OrderBy(p => p.Category.CategoryName);
                        break;
                    case "Category_dec":
                        products = products.OrderByDescending(p => p.Category.CategoryName);
                        break;
                    default:
                        products = products.OrderBy(p => p.Name);
                        break;
                }
                int Size_Of_Page = 4;
                int No_Of_Page = (Page_No ?? 1);
                return View(products.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return RedirectToAction("Error","Products");

            }
           

            

           /* return View(products);*/


        }
        [Authorize]
        public ActionResult MultipleDelete(int[] ids)
        {
            try
            {
                foreach (int id in ids)
                {
                    var product = this.db.Products.Find(id);
                    this.db.Products.Remove(product);
                    this.db.SaveChanges();
                }
                TempData["Type"] = 0;
                TempData["Message"] = "Products Deleted Successfully";
                throw new Exception();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return RedirectToAction("Error", "Products");

            }

        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (product.SImage.ContentLength > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("SImage", "File size cannot be more then 2Mb");
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(product.SImage.FileName);
                        string extension = Path.GetExtension(product.SImage.FileName).ToLower();
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        product.Smallimage = "~/Image/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                        product.SImage.SaveAs(fileName);
                    }
                    if (product.LImage.ContentLength > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("SImage", "File size cannot be more then 5Mb");
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(product.LImage.FileName);
                        string extension = Path.GetExtension(product.LImage.FileName).ToLower();
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        product.Largeimage = "~/Image/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                        product.LImage.SaveAs(fileName);
                    }


                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["Type"] = 0;
                    TempData["Message"] = "Product Added Successfully";
                    return RedirectToAction("Index");
                }

                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return RedirectToAction("Error", "Products");

            }
            
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
                TempData["Type"] = 0;
                TempData["Message"] = "Product edited Successfully";
                return View(product);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return RedirectToAction("Error", "Products");

            }
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (product.SImage != null)
                    {
                        if (product.SImage.ContentLength > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("SImage", "File size cannot be more then 2Mb");
                        }
                        else
                        {
                            if (System.IO.File.Exists(Server.MapPath(product.Smallimage)))
                            {
                                System.IO.File.Delete(Server.MapPath(product.Smallimage));

                            }
                            string fileName = Path.GetFileNameWithoutExtension(product.SImage.FileName);
                            string extension = Path.GetExtension(product.SImage.FileName).ToLower();
                            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                            product.Smallimage = "~/Image/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                            product.SImage.SaveAs(fileName);
                        }
                    }
                    if (product.LImage != null)
                    {
                        if (product.LImage.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("SImage", "File size cannot be more then 5Mb");
                        }
                        else
                        {
                            if (System.IO.File.Exists(Server.MapPath(product.Largeimage)))
                            {
                                System.IO.File.Delete(Server.MapPath(product.Largeimage));

                            }
                            string fileName = Path.GetFileNameWithoutExtension(product.LImage.FileName);
                            string extension = Path.GetExtension(product.LImage.FileName).ToLower();
                            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                            product.Largeimage = "~/Image/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                            product.LImage.SaveAs(fileName);
                        }
                    }


                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Type"] = 0;
                    TempData["Message"] = "Product edited Successfully";
                    return RedirectToAction("Index");
                }
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return RedirectToAction("Error", "Products");

            }
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
