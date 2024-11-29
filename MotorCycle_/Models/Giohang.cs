using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MotorCycle_.Models
{
    public class Giohang
    {
        QLBanXeGanMayEntities data = new QLBanXeGanMayEntities();
        public int iMaXe { get; set; }
        public string sTenxe { get; set; }
        public string sAnhbia { get; set; }
        public Double dDongia { get; set; }
        public int iSoluong { get; set; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }

        // Khoi tao gio hang theo Masach duoc truyen vao voi so luong mac dinh la 1
        public Giohang(int Maxe)
        {
            iMaXe = Maxe;
            XEGANMAY xe = data.XEGANMAYs.Single(n => n.MaXe == Maxe);
            sTenxe = xe.TenXe;
            sAnhbia = xe.Anhbia;
            dDongia = double.Parse(xe.Giaban.ToString());
            iSoluong = 1;
        }
    }
}