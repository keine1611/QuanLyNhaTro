using QuanLyNhaTro.Code;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QuanLyNhaTro.Controllers
{

	public class HomeController : Controller
	{
		QuanLyNhaTroEntities db = new QuanLyNhaTroEntities();
		//Danh cho Khach Thuê
		public ActionResult Index()
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();

			var listNhaTro = from s in db.NhaTroes orderby s.gia_NT descending select s;
			return View(listNhaTro.Take(6));
		}

		[HttpPost]
		public ActionResult GetDataSelected(string field, int flag)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();

			System.Threading.Thread.Sleep(1000);
			var listNhaTro = from s in db.NhaTroes select s;
			switch (field)
			{
				case "gia_NT":
					if (flag == 1)
						listNhaTro = listNhaTro.OrderBy(s => s.gia_NT);
					else
						listNhaTro.OrderByDescending(s => s.gia_NT);
					break;
				case "dienTich_NT":
					if (flag == 1)
						listNhaTro = listNhaTro.OrderBy(s => s.dienTich_NT);
					else
						listNhaTro = listNhaTro.OrderByDescending(s => s.dienTich_NT);
					break;
			}
			return View("Index", listNhaTro.Take(6));

		}

		[HttpPost]
		public ActionResult GetData(int numrow, string field, int flag)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();

			System.Threading.Thread.Sleep(1000);
			var listNhaTro = from s in db.NhaTroes select s;
			int count = listNhaTro.Count() - numrow;
			switch (field)
			{
				case "gia_NT":
					if (flag == 1)
						listNhaTro = listNhaTro.OrderBy(s => s.gia_NT).Skip(numrow);
					else
						listNhaTro = listNhaTro.OrderByDescending(s => s.gia_NT).Skip(numrow);
					break;
				case "dienTich_NT":
					if (flag == 1)
						listNhaTro = listNhaTro.OrderBy(s => s.dienTich_NT).Skip(numrow);
					else
						listNhaTro = listNhaTro.OrderByDescending(s => s.dienTich_NT).Skip(numrow);
					break;
			}
			if (count == 0)
				return Json(new { hehe = "hehe" });
			else
				if (count < 6)
			{
				return View("Index", listNhaTro);
			}
			else
			{
				return View("Index", listNhaTro.Take(6));
			}
		}

		[HttpGet]
		public ActionResult ViewProduct(int? id)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();

			if (id == null)
			{
				return HttpNotFound();
			}

			var product = db.NhaTroes.Include(s => s.HinhAnhs).Include(s => s.Phongs).SingleOrDefault(s => s.id_NT == id);
			if (product == null)
			{
				return HttpNotFound();
			}
			return View(product);
		}

		public ActionResult ViewProfileRoute()
		{

			TaiKhoan tk = Code.SessionHelper.GetSession();
			if (tk.quyenSD == "KhachThue")
			{
				return View("viewKhachHangProfile");
			}
			else
			{

			}
			return View();
		}

		public ActionResult ViewKhachHangProfile()
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();

			TaiKhoan tk = Code.SessionHelper.GetSession();
			KhachHang khachHang = db.KhachHangs.SingleOrDefault(s => s.userName == tk.userName);
			return View(khachHang);
		}

		[HttpPost]
		public ActionResult LuuProfileKhachHang(string hoten_KH, int gioitinh_KH, string sodt_KH, string cccd_KH, string email_KH, string diachi_KH)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("KhachThue"))
				return HttpNotFound();

			TaiKhoan tk = SessionHelper.GetSession();
			KhachHang khachHang = db.KhachHangs.SingleOrDefault(s => s.userName == tk.userName);
			try
			{
				khachHang.hoten_KH = hoten_KH;
				khachHang.gioitinh_KH = gioitinh_KH;
				khachHang.sodt_KH = sodt_KH;
				khachHang.cccd_KH = cccd_KH;
				khachHang.email_KH = email_KH;
				khachHang.diachi_KH = diachi_KH;
				db.Entry(khachHang).State = EntityState.Modified;
				db.SaveChanges();
				return View("ViewKhachHangProfile");
			}
			catch
			{
				return Json(new { hehe = "hehe" });
			}

		}





		//ChuTro
		public ActionResult ChuTroIndex()
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("ChuTro"))
				return HttpNotFound();

			TaiKhoan tk = Code.SessionHelper.GetSession();
			var listNhaTro = from s in db.NhaTroes where (s.userName.Equals(tk.userName)) select s;
			return View(listNhaTro);
		}


		[HttpPost]
		public ActionResult GetModalNhaTro(int? id_NT)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("ChuTro"))
				return HttpNotFound();

			if (id_NT == null)
				return View();
			else
			{
				var nt = db.NhaTroes.SingleOrDefault(s => s.id_NT == id_NT);
				if (nt == null)
				{
					return HttpNotFound();
				}
				else
				{
					return View(nt);
				}

			}

		}

		[HttpPost]
		public ActionResult ChangeStatusPhong(int? id)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("ChuTro"))
				return HttpNotFound();

			if (id == null)
			{
				return Json(new { data = 0 });
			}
			else
			{
				var pt = db.Phongs.SingleOrDefault(s => s.id_P == id);
				if (pt.trangthai_P == 1)
				{
					pt.trangthai_P = 0;
					db.SaveChanges();
				}
				else
				{
					pt.trangthai_P = 1;
					db.SaveChanges();
				}
				db.SaveChanges();
				return Json(new { data = 1 });
			}
		}

		[HttpPost]
		public ActionResult UpdateNhaTro(int id_NT, string ten_NT, string diachi_NT, string sdt_NT, string CCCD_NT, string email_NT, double dienTich_NT, int gac_NT, string ghichu_NT,int gia_NT)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("ChuTro"))
				return HttpNotFound();

			NhaTro nhaTro = db.NhaTroes.SingleOrDefault(s => s.id_NT == id_NT);
			nhaTro.id_NT = id_NT;
			nhaTro.ten_NT = ten_NT;
			nhaTro.diachi_NT = diachi_NT;
			nhaTro.sodt_NT = sdt_NT;
			nhaTro.CCCD_NT = CCCD_NT;
			nhaTro.email_NT = email_NT;
			nhaTro.dienTich_NT = dienTich_NT;
			nhaTro.gac_NT = gac_NT;
			nhaTro.ghichu_NT = ghichu_NT;
			nhaTro.gia_NT = gia_NT;
			try
			{
				db.Entry(nhaTro).State = System.Data.Entity.EntityState.Modified;
				db.SaveChanges();
				return Json(0);
			}
			catch
			{
				return Json(1);
			}
		}

		[HttpPost]
		public ActionResult AddNhaTro(string ten_NT, string diachi_NT, string sdt_NT, string CCCD_NT, string email_NT, double dienTich_NT, int gac_NT, string ghichu_NT, int gia_NT)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("ChuTro"))
				return HttpNotFound();

			var tk = SessionHelper.GetSession();
			try
			{
				NhaTro nt = new NhaTro();
				nt.ten_NT = ten_NT;
				nt.diachi_NT = diachi_NT;
				nt.sodt_NT = sdt_NT;
				nt.CCCD_NT = CCCD_NT;
				nt.email_NT = email_NT;
				nt.dienTich_NT = dienTich_NT;
				nt.gac_NT = gac_NT;
				nt.ghichu_NT = ghichu_NT;
				nt.sophong_NT = 0;
				nt.userName = tk.userName;
				nt.gia_NT = gia_NT;
				db.NhaTroes.Add(nt);
				db.SaveChanges();
				
				HinhAnh ha = new HinhAnh();
				ha.url_HA = "motel_default.png";
				ha.id_NT = nt.id_NT;
				db.HinhAnhs.Add(ha);
				db.SaveChanges();
				return Json(0);
			}
			catch
			{
				return Json(1);
			}
		}

		[HttpPost]
		public ActionResult DeleteNhaTro(int id_NT)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("ChuTro"))
				return HttpNotFound();

			NhaTro nhaTro = db.NhaTroes.SingleOrDefault(s => s.id_NT == id_NT);
			if (nhaTro == null)
			{
				return Json(1);
			}
			else
			{
				try
				{
					db.NhaTroes.Remove(nhaTro);
					db.SaveChanges();
					return Json(0);
				}
				catch
				{
					return Json(1);
				}
			}
		}

		[HttpPost]
		public ActionResult DeleteHinhAnh(int id_HA)
		{
			if (!SessionHelper.CheckSession())
				return RedirectToAction("Login", "Login");
			if (!SessionHelper.GetQuyenSD().Equals("ChuTro"))
				return HttpNotFound();

			HinhAnh hinhAnh = db.HinhAnhs.SingleOrDefault(s => s.id_HA == id_HA);
			if (hinhAnh == null)
			{
				return Json(1);
			}
			else
			{
				try
				{
					db.HinhAnhs.Remove(hinhAnh);
					db.SaveChanges();
					return Json(0);
				}
				catch
				{
					return Json(1);
				}
			}
		}



		[HttpPost]
		public ActionResult AddHinhAnh()
		{
			
			var id_NT = 1;
			HttpFileCollectionBase files = HttpContext.Request.Files;
			if (files.Count > 0)
			{
				for (int i = 0; i < files.Count; i++)
				{
					HttpPostedFileBase file = files[i];
					try
					{
						var fileName = file.FileName;
						HinhAnh ha = new HinhAnh
						{
							id_NT = id_NT,
							url_HA = fileName,
						};
						//Lưu đường dẫn file ảnh 
						var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
						//Kiểm tra file đã tồn tại
						if (System.IO.File.Exists(path))
						{
							return Json(1);
						}
						else
						{
							file.SaveAs(path);
						}

						db.HinhAnhs.Add(ha);
						db.SaveChanges();

					}
					catch
					{
						return Json(1);
					}
				}
				return Json(0);

			}
			return Json(2);




		}
	}
}