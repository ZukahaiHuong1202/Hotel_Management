using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKS
{
    internal static class Ma
    {
        public static String KhachHang()
        {
            String makh = "";
            makh = "KH" + DateTime.Now.ToString("HHmmssddMMyyyy");
            return makh;
        }

        public static String BookingPhong()
        {
            String mabkp = "";
            mabkp = "BKP" + DateTime.Now.ToString("HHmmssddMMyyyy");
            return mabkp;
        }

        public static String HoaDon()
        {
            String mahd = "";
            mahd = "HD" + DateTime.Now.ToString("HHmmssddMMyyyy");
            return mahd;
        }

        public static String Phong(String sophong)
        {
            String map = "P" + sophong;
            return map;
        }
    }
}
