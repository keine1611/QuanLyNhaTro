using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaTro.Code;

namespace QuanLyNhaTro.Controllers
{
    public class ShoppingCartController : Controller
    {
        QuanLyNhaTroEntities db = new QuanLyNhaTroEntities();

        [HttpPost]
        public ActionResult ModalShoppingCart()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
                return HttpNotFound();
            TaiKhoan taikhoan = SessionHelper.GetSession();
            KhachHang khachHang = db.KhachHangs.SingleOrDefault(s => s.userName == taikhoan.userName);
            var lstItem = from s in db.GioHangs where s.id_KH == khachHang.id_KH select s;
            return View(lstItem);
        }


        [HttpPost]
        public ActionResult AddToCart(int id_P) {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
                return HttpNotFound();
            TaiKhoan taikhoan = SessionHelper.GetSession();
            KhachHang khachHang = db.KhachHangs.SingleOrDefault(s => s.userName == taikhoan.userName);
            GioHang itemCart = db.GioHangs.SingleOrDefault(s => s.id_P == id_P && s.id_KH == khachHang.id_KH);
			if (itemCart == null)
			{
                GioHang item = new GioHang();
                item.id_P = id_P;
               
                item.id_KH = khachHang.id_KH;
                try
                {
                    db.GioHangs.Add(item);
                    db.SaveChanges();
                    return Json(new { result = "0" });
                }
                catch
                {
                    return Json(new { result = "1" });
                }
              
            }
			else
			{
                return Json(new { result = "2" });
			}
        }

        [HttpPost]
        public ActionResult DeleteItemInCart(int id_P)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
                return HttpNotFound();
            TaiKhoan taikhoan = SessionHelper.GetSession();
            KhachHang khachHang = db.KhachHangs.SingleOrDefault(s => s.userName == taikhoan.userName);
            GioHang itemCart = db.GioHangs.SingleOrDefault(s => s.id_P == id_P && s.id_KH == khachHang.id_KH);
            if (itemCart != null)
            {
                try
                {
                    db.GioHangs.Remove(itemCart);
                    db.SaveChanges();
                    return Json(new { result = "0" });
                }
                catch
                {
                    return Json(new { result = "1" });
                }

            }
            else
            {
                return Json(new { result = "2" });
            }

        }



    }
}