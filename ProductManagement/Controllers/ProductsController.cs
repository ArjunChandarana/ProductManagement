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

        // GET: Products
        public ActionResult Index(string sortOrder, string CurrentSort, int? page)
        {
            /*var products = db.Products.Include(p => p.Category);
            return View(products.ToList());*/

          
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.CurrentSort = sortOrder;
            sortOrder = String.IsNullOrEmpty(sortOrder) ? "Name" : sortOrder;
            IPagedList<Product> products = null;
            switch (sortOrder)
            {
                case "Name":
                    if (sortOrder.Equals(CurrentSort))
                        products = db.Products.OrderByDescending
                                (m => m.Name).ToPagedList(pageIndex, pageSize);
                    else
                        products = db.Products.OrderBy
                                (m => m.Name).ToPagedList(pageIndex, pageSize);
                    break;
                case "Category":
                    if (sortOrder.Equals(CurrentSort))
                        products = db.Products.OrderByDescending
                                (m => m.Category.CategoryName).ToPagedList(pageIndex, pageSize);
                    else
                        products = db.Products.OrderBy
                                (m => m.Category.CategoryName).ToPagedList(pageIndex, pageSize);
                    break;
               
                case "Default":
                    products = db.Products.OrderBy
                        (m => m.Name).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(products);


        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
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
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
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
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
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
