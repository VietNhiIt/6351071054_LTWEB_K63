using MotorCycle_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList.Mvc;

namespace MotorCycle_.Controllers
{
    public class GiohangController : Controller
    {
        QLBanXeGanMayEntities data = new QLBanXeGanMayEntities();
        // GET: Giohang
  
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;

            if (lstGiohang == null)
            {
                // neu gio hang chua ton tai thi khoi tao listGiohang
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }

        public ActionResult ThemGiohang(int iMaXe, string sUrl)
        {
            // Lay ra session gio hang
            List<Giohang> lstGiohang = Laygiohang();

            // Kiem tra sach nay ton tai trong Session["Giohang"] chua ?
            Giohang sanpham = lstGiohang.Find(n => n.iMaXe == iMaXe);

            if (sanpham == null)
            {
                sanpham = new Giohang(iMaXe);
                lstGiohang.Add(sanpham);
                return Redirect(sUrl);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(sUrl);
            }
        }

        public int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;

            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        public double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;

            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        // GET: Giohang
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "MotorCycle");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }

        public ActionResult Giohangpartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGiohang(int id)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaXe == id);

            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMaXe == id);
                return RedirectToAction("Giohang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "MotorCycle");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapnhatGiohang(int id, FormCollection f)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaXe == id);

            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "MotorCycle");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            // Kiểm tra nếu người dùng chưa đăng nhập
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            // Kiểm tra nếu giỏ hàng trống
            var giohang = Session["Giohang"] as List<Giohang>;
            if (giohang == null || !giohang.Any())
            {
                return RedirectToAction("GioHang", "Giohang");
            }

            // Tính tổng số lượng và tổng tiền
            ViewBag.Tongsoluong = giohang.Sum(g => g.iSoluong);
            ViewBag.Tongtien = giohang.Sum(g => g.dThanhtien);

            // Truyền danh sách giỏ hàng vào View
            return View(giohang);
        }


        public ActionResult DatHang(FormCollection collection)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            if (kh == null) return RedirectToAction("Dangnhap", "Nguoidung");

            List<Giohang> gh = Laygiohang();
            if (gh == null || !gh.Any()) return RedirectToAction("Index", "MotorCycle");

            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;

            var ngaygiao = collection["Ngaygiao"];
            if (!DateTime.TryParseExact(ngaygiao, "MM/dd/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedNgayGiao))
            {
                ModelState.AddModelError("", "Ngày giao không hợp lệ. Vui lòng nhập đúng định dạng MM/dd/yyyy.");
                return View();
            }
            ddh.Ngaygiao = parsedNgayGiao;
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;

            data.DONDATHANGs.Add(ddh);
            data.SaveChanges();

            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG
                {
                    MaDonHang = ddh.MaDonHang,
                    MaXe = item.iMaXe,
                    Soluong = item.iSoluong,
                    Dongia = (decimal)item.dDongia
                };
                data.CHITIETDONTHANGs.Add(ctdh);
            }
            data.SaveChanges();

            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }


        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}