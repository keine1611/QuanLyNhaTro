using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaTro.Code;
namespace QuanLyNhaTro.Controllers
{
    public class AdminDonDatController : Controller
    {
        QuanLyNhaTroEntities db = new QuanLyNhaTroEntities();

        public ActionResult DanhSachDonDat()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.GetQuyenSD().Equals("Admin"))
                return HttpNotFound();
            var danhsach = from s in db.DonDatPhongs select s;
            return View(danhsach);
        }
        [HttpGet]
        public JsonResult GetDataPluginTable()
        {
            var lstNhaTro = from s in db.DonDatPhongs select (new { s.id_DDP, s.id_KH, V = s.ngaydat_DDP.ToString(), s.trangthai_DDP });
            return Json(new { data = lstNhaTro.ToList() }, JsonRequestBehavior.AllowGet);
        }
    }
}