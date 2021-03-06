﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Components.Domains;
using Components.Repositories.Ef;

namespace choujiang_api.Controllers
{
    public class ProductsController : WxAppController
    {
        private ChouJiangDbContext db = new ChouJiangDbContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Business);
            return View(products.ToList());
        }
        public ActionResult GetProductById(int id)
        {
            bool isCan = false;
            var product = db.Products.Find(id);
            var order = db.Orders.Where(o => o.PorductId == id && o.UserId == CurrentUser.Id).SingleOrDefault();
            if (order == null)
            {
                isCan = true;
            }
            //var product = products.Select(p => new { p.Id, p.Name, BusinessName = p.Business.Name, OpenTime = p.OpenTime.ToLongTimeString() });
            return Json(new { success = true, iscan = isCan, product.Id, product.Name, BusinessName = product.Business.Name, OpenTime = product.OpenTime.ToString("MM月dd日 HH:mm") }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult List()
        {
            var products = db.Products.ToList();
            var product = products.Select(p => new { p.Id, p.Name, BusinessName = p.Business.Name, OpenTime = p.OpenTime.ToLongTimeString() });
            return Json(new { success = true, product }, JsonRequestBehavior.AllowGet);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.BusinessId = new SelectList(db.Businesses, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,OpenTime,BusinessId")] Products products, HttpPostedFileBase productHeadImg)
        {
            if (ModelState.IsValid)
            {
                products.CreateTime = DateTime.Now;
                db.Products.Add(products);
                if (productHeadImg != null)
                {
                    //存储封面图片
                    string imgName = Guid.NewGuid() + ".jpg";
                    string imgPath = System.IO.Path.Combine(Server.MapPath("~/Upload/ProductHeadImg"), imgName);
                    productHeadImg.SaveAs(imgPath);
                    products.ProductHeadImg = imgName;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BusinessId = new SelectList(db.Businesses, "Id", "Name", products.BusinessId);
            return View(products);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "Id", "Name", products.BusinessId);
            return View(products);
        }

        // POST: Products/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form, HttpPostedFileBase productHeadImg)
        {
            var product = db.Products.Find(Convert.ToInt32(form["Id"]));
            product.Name = form["Name"];
            product.OpenTime = Convert.ToDateTime(form["OpenTime"]);
            product.BusinessId = Convert.ToInt32(form["BusinessId"]);

            if (productHeadImg != null)
            {
                //存储封面图片
                string imgName = Guid.NewGuid() + ".jpg";
                string imgPath = System.IO.Path.Combine(Server.MapPath("~/Upload/ProductHeadImg"), imgName);
                productHeadImg.SaveAs(imgPath);
                product.ProductHeadImg = imgName;
                product.CreateTime = product.CreateTime;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "Id", "Name", product.BusinessId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = db.Products.Find(id);
            db.Products.Remove(products);
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
