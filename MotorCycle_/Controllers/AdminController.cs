using MotorCycle_.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Web;
using System.Web.Mvc;

using System.Web.UI.WebControls;

namespace MotorCycle_.Controllers
{
    public class AdminController : Controller
    {
        QLBanXeGanMayEntities db = new QLBanXeGanMayEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Xe(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 4;

            // Lấy danh sách xe từ cơ sở dữ liệu
            List<XEGANMAY> listXe = db.XEGANMAYs.ToList();
            var pagedList = listXe.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            Console.WriteLine($"Username: {tendn}, Password: {matkhau}"); // Debug line

            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                ADMIN admin = db.ADMINs.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (admin != null)
                {
                    Session["Taikhoanadmin"] = admin;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        public ActionResult ThemmoiXe()
        {
            ViewBag.MaLX = new SelectList(db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiXe(XEGANMAY xe, HttpPostedFileBase fileUpload)
        {
            try
            {
                ViewBag.MaLX = new SelectList(db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
                ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                return View();
            }

            //ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n=>n.TenChuDe), "MaCD", "TenChude");
            //ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    xe.Anhbia = fileName;
                    // Luu vao CSDL
                    db.XEGANMAYs.Add(xe);
                    db.SaveChanges();
                }
                return RedirectToAction("Xe");
            }
        }

        public ActionResult Chitietxe(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            ViewBag.Maxe = xe.MaXe;
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(xe);
        }

        [HttpGet]
        public ActionResult Xoaxe(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            ViewBag.Masach = xe.MaXe;
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(xe);
        }

        [HttpPost, ActionName("Xoaxe")]

        public ActionResult Xacnhanxoa(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.XEGANMAYs.Remove(xe);
            db.SaveChanges();
            return RedirectToAction("Xe");
        }

        [HttpGet]
        public ActionResult Suaxe(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Gán ViewBag với danh sách các mục
            ViewBag.MaLX = new SelectList(db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe", xe.MaLX);
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP", xe.MaNPP);

            return View(xe);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suaxe(XEGANMAY xe, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaLX = new SelectList(db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");

            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);

                    var existingCar = db.XEGANMAYs.FirstOrDefault(c => c.MaXe == xe.MaXe);
                   

                    existingCar.TenXe = xe.TenXe;
                    existingCar.Giaban = xe.Giaban;
                    existingCar.Mota = xe.Mota;
                    existingCar.Ngaycapnhat = xe.Ngaycapnhat;
                    existingCar.Soluongton = xe.Soluongton;
                    existingCar.MaLX = xe.MaLX;
                    existingCar.MaNPP = xe.MaNPP;

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    existingCar.Anhbia = fileName;
                    // Luu vao CSDL
                    db.SaveChanges();
                }
                else
                {
                    return Json(new { mesage = "fail" }, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Xe");
            }
        }
    }
}