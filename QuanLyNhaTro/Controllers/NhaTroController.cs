using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaTro.Code;
namespace QuanLyNhaTro.Controllers
{
    public class NhaTroController : Controller
    {
        QuanLyNhaTroEntities db = new QuanLyNhaTroEntities();
        public ActionResult DanhSachNhaTro()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            var danhsach = from s in db.NhaTroes select s;
            return View(danhsach);
        }

        public ActionResult ThemNhaTro()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            return View();
        }

        [HttpPost, ActionName("ThemNhaTro")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNhaTro(NhaTro NhaTro, HinhAnh HinhAnh, HttpPostedFileBase fileUpload)
        {

            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            {

                try
                {
                    if (ModelState.IsValid)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                        }
                        db.NhaTroes.Add(NhaTro); db.SaveChanges();
                        HinhAnh.url_HA = fileName;
                        HinhAnh.id_NT = NhaTro.id_NT;
                        db.HinhAnhs.Add(HinhAnh);
                        db.SaveChanges();
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
                var DanhSachNhaTro = from s in db.NhaTroes select s;
                return View("DanhSachNhaTro", DanhSachNhaTro);
            }
        }

        [HttpGet]
        public ActionResult CapNhatNhaTro(int? id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            if (id == null)
            {
                return HttpNotFound();
            }
            NhaTro NhaTro = db.NhaTroes.SingleOrDefault(s => s.id_NT == id);
            return View(NhaTro);
        }

        [HttpPost, ActionName("CapNhatNhaTro")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatNhaTro(NhaTro nt, HttpPostedFileBase file)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            try
            {

                if (file == null)
                {
                    ViewBag.ThongBao = "Lỗi tải ảnh";
                }
                else
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    file.SaveAs(path);
                    HinhAnh ha = new HinhAnh();
                    ha.url_HA = fileName;
                    ha.id_NT = nt.id_NT;
                    db.HinhAnhs.Add(ha);
                    db.SaveChanges();
                }
                if (ModelState.IsValid)
                {
                    db.Entry(nt).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DanhSachNhaTro");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Thong tin ko hop le");
            }
            return View();
        }

        [HttpGet]
        public ActionResult XoaNhaTro(int id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            NhaTro nt = db.NhaTroes.SingleOrDefault(n => n.id_NT == id);
            if (nt == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nt);
        }


        [HttpPost, ActionName("XoaNhaTro")]
        public ActionResult XacNhanXoaNhaTro(int id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            NhaTro nhatro = db.NhaTroes.SingleOrDefault(n => n.id_NT == id);
            HinhAnh hinhanh = db.HinhAnhs.SingleOrDefault(n => n.id_NT == id);
            var path = Path.Combine(Server.MapPath("~/Content/Images"), hinhanh.url_HA);

            if (nhatro == null || !KiemTraKhoaNgoai(id, "NhaTro"))
            {
                ModelState.AddModelError("", "Ban phai xoa het cac phong trong nha tro nay!");
                return View("XoaNhaTro", nhatro);
            }

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            db.HinhAnhs.Remove(hinhanh);
            db.NhaTroes.Remove(nhatro);
            db.SaveChanges();
            return RedirectToAction("DanhSachNhaTro");
        }

        [HttpPost]
        public ActionResult XoaHinhAnhNhaTro(int? id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                HinhAnh ha = db.HinhAnhs.SingleOrDefault(s => s.id_HA == id);
                db.HinhAnhs.Remove(ha);
                db.SaveChanges();
                return Json(new { data = 0 });
            }
        }



        [HttpPost]
        public ActionResult LoadModalHinhAnh(int? id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.id_NT = id;
                var lstHA = from s in db.HinhAnhs where (s.id_NT == id) select s;
                return View(lstHA);
            }
        }

        //Quan ly phong

        public ActionResult DanhSachPhongTro(int id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            var danhsach = from s in db.Phongs where (s.id_NT == id) select s;
            return View(danhsach);
        }

        public ActionResult ThemPhongTro(int id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            Phong p = new Phong();
            p.id_NT = id;
            return View(p);
        }

        [HttpPost, ActionName("ThemPhongTro")]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult ThemPhongTro(Phong p)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Phongs.Add(p);
                    db.SaveChanges();
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }
            var DanhSachPhong = from s in db.Phongs where (s.id_NT == p.id_NT) select s;
            return View("DanhSachPhongTro", DanhSachPhong);
        }
        // Cap Nhat Phong
        [HttpGet]
        public ActionResult CapNhatPhongTro(int? id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phong p = db.Phongs.FirstOrDefault(s => s.id_P == id);
            return View(p);
        }

        [HttpPost, ActionName("CapNhatPhongTro")]
        [ValidateInput(true)]
        public ActionResult CapNhatPhongTro(Phong p)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(p).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Thong tin ko hop le");
            }
            var ds = from s in db.Phongs where (s.id_NT == p.id_NT) select s;
            return View("DanhSachPhongTro", ds);
        }

        //public ActionResult XemPhongTheoNhaTro(int id)
        //{
        //    ViewBag.id = id;
        //    var ds = from s in db.Phongs where (s.id_NT == id) select s;
        //    return View(ds);
        //}
        //[HttpPost]
        //public ActionResult GetDataPluginTable(int id)
        //{
        //    var lstNhaTro = from s in db.Phongs where (s.id_NT == id) select (new { s.id_NT, s.id_P, s.ten_P,s.noithat_P ,s.songuoi_P  });
        //    return Json(new { data = lstNhaTro.ToList() }, JsonRequestBehavior.AllowGet);
        //}


        [HttpGet]
        public ActionResult XoaPhongTro(int? id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            Phong p = db.Phongs.SingleOrDefault(s => s.id_P == id);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(p);
        }

        [HttpPost, ActionName("XoaPhongTro")]
        public ActionResult XacNhanXoaPhong(int id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            Phong p = db.Phongs.SingleOrDefault(n => n.id_P == id);
            if (p == null || !KiemTraKhoaNgoai(id, "Phong"))
            {
                ModelState.AddModelError("", "Ban phai xoa het cac don hang trong phong tro nay!");
                return View("XoaPhongTro", p);
            }
            db.Phongs.Remove(p);
            db.SaveChanges();
            var ds = from s in db.Phongs where (s.id_NT == p.id_NT) select s;
            return View("DanhSachPhongTro", ds);
        }


        public bool KiemTraKhoaNgoai(int id, String a)
        {
            bool ketqua = true;
            if (a.Equals("Phong"))
            {
                var gh = db.GioHangs.SingleOrDefault(s => s.id_P == id);
                return (gh == null);//Dung khi k co gio hang
            }
            if (a.Equals("NhaTro"))
            {
                var p = db.Phongs.FirstOrDefault(s => s.id_NT == id);
                return (p == null);
            }
            return ketqua;
        }

        //public ActionResult GioHang()
        //{
        //    var gioHangs = db.GioHangs.Include(g => g.KhachHang).Include(g => g.Phong);
        //    return View(gioHangs.ToList());
        //}

        //public ActionResult DonDatPhong()
        //{
        //    var ddh = db.DonDatPhongs.Include(s => s.KhachHang);
        //    return View(ddh.ToList());
        //}

    }
}