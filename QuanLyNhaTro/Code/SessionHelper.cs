using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyNhaTro.Code
{
	public class SessionHelper
	{
        public static void SetSession(TaiKhoan taiKhoan)
        {
          HttpContext.Current.Session["taikhoan"] = taiKhoan;
        }

        public static TaiKhoan GetSession()
        {
            var session = HttpContext.Current.Session["taikhoan"];
            if (session == null)
                return null;
            else
            {
                return session as TaiKhoan;
            }
        }
        public static bool CheckSession()
		{
            var session = HttpContext.Current.Session["taikhoan"];
            if (session == null)
                return false;
            else
                return true;
        }
        public static String GetQuyenSD()
        {
            var session = HttpContext.Current.Session["taikhoan"] as TaiKhoan;
            return session.quyenSD;
        }
    }
}