using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyNhaTro.Code;

namespace QuanLyNhaTro.Controllers
{
	public class DonDatController : Controller
	{
		QuanLyNhaTroEntities db = new QuanLyNhaTroEntities();

		[HttpPost]
		public ActionResult ModalDonDat()
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();
			TaiKhoan taiKhoan = SessionHelper.GetSession();
			KhachHang khachHang = db.KhachHangs.SingleOrDefault(s => s.userName == taiKhoan.userName);
			var lstDonDat = from s in db.DonDatPhongs where (s.id_KH == khachHang.id_KH) orderby s.ngaydat_DDP descending select s;
			return View(lstDonDat);
		}


		public ActionResult AddDonDat(int loaicoc)
		{
			TaiKhoan taiKhoan = SessionHelper.GetSession();
			KhachHang khachHang = db.KhachHangs.SingleOrDefault(s => s.userName == taiKhoan.userName);
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();
			DonDatPhong dondat = new DonDatPhong
			{
				id_KH = khachHang.id_KH,
				trangthai_DDP = 1,
				ngaydat_DDP = DateTime.Now,
				loaidatcoc_NT = loaicoc,
			};
			try
			{
				db.DonDatPhongs.Add(dondat);
				db.SaveChanges();
				int id_DD = dondat.id_DDP;
				return Json(new { status = 0, id_DD = id_DD });
			}
			catch
			{
				return Json(new { status = 1 });
			}
		}

		[HttpPost]
		public ActionResult CheckStatusPhong(int[] arrP)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();
			foreach (var item in arrP)
			{
				Phong phong = db.Phongs.SingleOrDefault(s => s.id_P == item);
				if (phong == null)
				{
					return Json(new { status = 1 });
				}
				else
				{
					if (phong.trangthai_P == 1)
					{
						return Json(new { status = 2, ten_P = phong.ten_P, ten_NT = phong.NhaTro.ten_NT });
					}
				}
			}
			return Json(new { status = 0 });
		}

		[HttpPost]
		public ActionResult AddChiTietDonDat(int id_DD, int[] arrP, int[] arrGia)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();
			TaiKhoan taiKhoan = SessionHelper.GetSession();
			KhachHang khachHang = db.KhachHangs.SingleOrDefault(s => s.userName == taiKhoan.userName);
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();
			int i = 0;
			foreach (var item in arrP)
			{
				ChiTietDonDat chiTiet = new ChiTietDonDat
				{
					id_P = item,
					id_DDP = id_DD,
					dongia_CTDD = arrGia[i]
				};
				i++;
				try
				{
					db.ChiTietDonDats.Add(chiTiet);
					db.SaveChanges();
					Phong phong = db.Phongs.SingleOrDefault(s => s.id_P == item);
					phong.trangthai_P = 1;
					db.SaveChanges();
				}
				catch
				{
					var lstChiTiet = from s in db.ChiTietDonDats where (s.id_DDP == id_DD)select s;
					foreach(var chitiet in lstChiTiet)
					{
						chitiet.Phong.trangthai_P = 0;
						db.ChiTietDonDats.Remove(chitiet);
					}
					DonDatPhong donDat = db.DonDatPhongs.SingleOrDefault(s => s.id_DDP == id_DD);
					db.DonDatPhongs.Remove(donDat);
					return Json(1);
				}
			}
			foreach(var item in arrP)
			{
				try
				{
					GioHang gioHang = db.GioHangs.SingleOrDefault(s => s.id_P == item && s.id_KH == khachHang.id_KH);
					db.GioHangs.Remove(gioHang);
					db.SaveChanges();
				}
				catch{}
			}
			return Json(0);
		}

		[HttpPost]
		public ActionResult ModalChiTietDonDat(int id_DD)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();
			var lstChiTiet = from s in db.ChiTietDonDats where (s.id_DDP == id_DD) select s;
			if(lstChiTiet == null)
			{
				return Json(1);
			}
			return View(lstChiTiet);
		}


	}
}