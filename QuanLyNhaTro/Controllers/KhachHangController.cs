using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaTro.Code;
namespace QuanLyNhaTro.Controllers
{
    public class KhachHangController : Controller
    {
        QuanLyNhaTroEntities db = new QuanLyNhaTroEntities();
        // GET: KhachHang
        public ActionResult dsKhachHang(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name" : "";
            ViewBag.GioitinhSortParm = String.IsNullOrEmpty(sortOrder) ? "Gioitinh" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var KH = from s in db.KhachHangs select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                KH = KH.Where(s => s.hoten_KH.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Name":
                    KH = KH.OrderBy(s => s.userName);
                    break;
                case "Gioitinh":
                    KH = KH.OrderBy(s => s.gioitinh_KH);
                    break;
                default:
                    KH = KH.OrderBy(s => s.userName);
                    break;
            }

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(KH.ToPagedList(pageNumber, pageSize));
        }

        //sua Khách hàng

        [HttpGet]
        [ValidateInput(true)]
        public ActionResult suaKhachHang(string userName)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            if (userName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                KhachHang khachhang = new KhachHang();
                khachhang = db.KhachHangs.SingleOrDefault(s => s.userName.Equals(userName));
                return View(khachhang);
            }
        }
        [HttpPost, ActionName("suaKhachHang")]
        [ValidateAntiForgeryToken]

        public ActionResult suaKhachHang(KhachHang khachhang)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(khachhang).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("dsKhachHang");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "lỗi tìm dữ liệu");
            }
            return View();
        }
    }
}