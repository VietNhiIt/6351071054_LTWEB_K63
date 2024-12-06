using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MotorCycle_.Models;
using PagedList;

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

        public ActionResult Index(int ? page)
        {
            int pageSize = 5;
            //tạo biến số trang
            int pageNum = (page ?? 1);

            var xemoi = Layxemoi(5);
            return View(xemoi.ToPagedList(pageNum, pageSize));
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

        public ActionResult SPTheoloai(int id, int? page)
        {
            ViewBag.MaLX = id;
            int pageSize = 3;
            int pageNum = (page ?? 1);

            var xe = (from x in data.XEGANMAYs
                        where x.MaLX == id
                        select x).ToList();

            return View(xe.ToPagedList(pageNum, pageSize));
        }

        public ActionResult SPTheoNPP(int id, int? page)
        {
            ViewBag.MaLX = id;
            int pageSize = 3;
            int pageNum = (page ?? 1);

            var xe = from x in data.XEGANMAYs
                       where x.MaNPP == id
                       orderby x.MaXe // or another property to order by
                       select x;

            return View(xe.ToPagedList(pageNum, pageSize));
        }


        public ActionResult About()
        {
            return View();
        }
    }
}