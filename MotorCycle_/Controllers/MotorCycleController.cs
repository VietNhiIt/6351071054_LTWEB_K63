using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MotorCycle_.Models;

namespace MotorCycle_.Controllers
{
    public class MotorCycleController : Controller
    {
        // GET: MotorCycle
        // Tạo một đối tượng chưas toàn bộ csdl từ QLBanXeGanMay 
        QLBanXeGanMayEntities data = new QLBanXeGanMayEntities();

        private List<XEGANMAY> Layxemoi(int count)
        {
            return data.XEGANMAYs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var xemoi = Layxemoi(5);
            return View(xemoi);
        }



        public ActionResult LoaiXe()
        {
            var loaixe = from lx in data.LOAIXEs select lx;
            return PartialView(loaixe);
        }
        public ActionResult Nhaphanphoi()
        {
            var loaixe = from lx in data.NHAPHANPHOIs select lx;
            return PartialView(loaixe);
        }

        public ActionResult Details(int id)
        {
            var xe = from x in data.XEGANMAYs
                       where x.MaXe == id
                       select x;
            return View(xe.Single());
        }

        public ActionResult SPTheoloai(int id)
        {
            var xe = from x in data.XEGANMAYs where x.MaLX == id select x;
            return View(xe);
        }

        public ActionResult SPTheoNPP(int id)
        {
            var xe = from x in data.XEGANMAYs where x.MaNPP == id select x;
            return View(xe);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}