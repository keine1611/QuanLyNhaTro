using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Data.Entity.Infrastructure;
using System.Web.UI.WebControls;
using PagedList;
using QuanLyNhaTro.Code;
namespace QuanLyNhaTro.Controllers
{
    public class AccountsController : Controller
    { QuanLyNhaTroEntities db = new QuanLyNhaTroEntities();

        // GET: Accounts


        public ActionResult TaiKhoan(string sortOrder,string currentFilter, string searchString, int? page)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.QuyenSDSortParm = string.IsNullOrEmpty(sortOrder) ? "Quyen" : ""; 


            if(searchString != null)
            {
                page= 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var taikhoan = from s in db.TaiKhoans select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                taikhoan = taikhoan.Where(s => s.quyenSD.Contains(searchString) || s.userName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Quyen":
                    taikhoan = taikhoan.OrderBy(s => s.quyenSD);
                    break;
                default:
                    taikhoan=taikhoan.OrderBy(s=>s.userName);
                    break;
            }

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(taikhoan.ToPagedList(pageNumber, pageSize));
            
        }

        public ActionResult showTaiKhoan(string quyenSD)
        {
            if (quyenSD.Equals("all"))
            {
                var dsTaiKhoan = from s in db.TaiKhoans select s;
                return View("TaiKhoan", dsTaiKhoan);
            }
            else
            {
                var dsTaiKhoan = from s in db.TaiKhoans where s.quyenSD.Equals(quyenSD) select s;
                return View("TaiKhoan", dsTaiKhoan);
            }
        }


        //tạo Tài khoản
        public ActionResult themTaiKhoan()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            return View();
        }
        [HttpPost, ActionName("themTaiKhoan")]
        [ValidateInput(true)]
        public ActionResult themTaiKhoan(TaiKhoan taiKhoan, HttpPostedFileBase fileUpload, KhachHang khachhang)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            var tk = db.TaiKhoans.SingleOrDefault(s => s.userName == taiKhoan.userName);
            if(tk != null)
			{
                ModelState.AddModelError("", "Tài khoản đã được sử dụng!");
                return View();
			}
            try
            {
                if(fileUpload != null || ModelState.IsValid || fileUpload.ContentLength >0)
                {
                    string fileName = Path.GetFileName(fileUpload.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    if(System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        fileUpload.SaveAs(path);
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    taiKhoan.avatar = fileUpload.FileName;

                    db.TaiKhoans.Add(taiKhoan);
                    db.KhachHangs.Add(khachhang);
                    db.SaveChanges();
                    
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "lỗi thêm tài khoản");
            }
            return RedirectToAction("dsKhachHang", "KhachHang");

        }


        //sửa tài khoản
        [HttpGet]
        [ValidateInput(true)]
        public ActionResult suaTaiKhoan (string userName)
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
                TaiKhoan taikhoan = new TaiKhoan();
                taikhoan = db.TaiKhoans.SingleOrDefault(s => s.userName.Equals(userName));
                return View(taikhoan);
            }
        }

        [HttpPost, ActionName("suaTaiKhoan")]
        [ValidateAntiForgeryToken]
        public ActionResult suaTaiKhoan (TaiKhoan taikhoan , HttpPostedFileBase fileUpload)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            try
            {
                if (fileUpload!=null ||fileUpload.ContentLength>0 || ModelState.IsValid)
                {
                    string fileName = Path.GetFileName(fileUpload.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/avatar/"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        fileUpload.SaveAs(path);
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    taikhoan.avatar = fileUpload.FileName;
                    db.Entry(taikhoan).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("TaiKhoan");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "lỗi tìm dữ liệu");
            }
            return View();
        }

        //xoa tai khoan
        [HttpGet]
        [ValidateInput(true)]
        public ActionResult xoaTaiKhoan(string userName)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            TaiKhoan taikhoan = db.TaiKhoans.SingleOrDefault(s => s.userName ==userName);
            if(taikhoan == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost, ActionName("xoaTaiKhoan")]
        [ValidateAntiForgeryToken]
        public ActionResult xacNhanXoaTaiKhoan(string userName)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();

            TaiKhoan taikhoan = db.TaiKhoans.SingleOrDefault(s => s.userName == userName);
            KhachHang khachhang = db.KhachHangs.SingleOrDefault(s => s.userName == userName);

            if (taikhoan == null )
            {
                return HttpNotFound();
            }
            else if (khachhang == null)
            {
                db.TaiKhoans.Remove(taikhoan);
                db.SaveChanges();
                return RedirectToAction("TaiKhoan");
            }
            else
            {
                db.TaiKhoans.Remove(taikhoan);
                db.KhachHangs.Remove(khachhang);
                db.SaveChanges();
                return RedirectToAction("TaiKhoan");
            }

        }
    }
}