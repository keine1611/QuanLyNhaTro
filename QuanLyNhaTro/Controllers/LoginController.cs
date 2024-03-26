using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using QuanLyNhaTro.Code;

namespace QuanLyNhaTro.Controllers
{
	public class LoginController : Controller
	{

		QuanLyNhaTroEntities db = new QuanLyNhaTroEntities();
		string strQuyenSD;
		// GET: Login
		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(TaiKhoan taiKhoan)
		{
			var myUser = db.TaiKhoans.FirstOrDefault(u => u.userName == taiKhoan.userName && u.password == taiKhoan.password);
			if (myUser != null)
			{
				strQuyenSD = myUser.quyenSD.ToString().Trim();
				if (strQuyenSD.Equals("Admin"))
				{
					SessionHelper.SetSession(myUser);
					return RedirectToAction("DanhSachNhaTro", "NhaTro");
				}
				else
				{
					if (strQuyenSD.Equals("KhachThue"))
					{
						SessionHelper.SetSession(myUser);
						return RedirectToAction("Index", "Home");
					}
					else
					{
						SessionHelper.SetSession(myUser);
						return RedirectToAction("ChuTroIndex", "Home");
					}
				}
			}
			else
			{
				ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng, vui lòng thử lại!");

			}
			return View(taiKhoan);
		}

		public ActionResult Register()
		{
			return View();
		}
		[HttpPost, ActionName("Register")]
		[ValidateAntiForgeryToken]
		public ActionResult Register(TaiKhoan taikhoan)
		{
			var user = db.TaiKhoans.FirstOrDefault(u => u.userName == taikhoan.userName);
			if (user != null)
			{
				ModelState.AddModelError("", "Tài khoản đã tồn tại");
			}
			else
			{
				string confirmpw = Request.Form.Get("confirmpw").ToString().Trim();
				if (confirmpw != taikhoan.password)
				{
					ModelState.AddModelError("", "Mật khẩu không khớp");
				}
				else
				{
					string quyenSD = Request.Form.Get("accountType").ToString().Trim();
					taikhoan.quyenSD = quyenSD;
					taikhoan.avatar = "user_default.png";
					db.TaiKhoans.Add(taikhoan);
					db.SaveChanges();
					if (taikhoan.quyenSD.Equals("KhachThue"))
					{
						KhachHang khachHang = new KhachHang();
						khachHang.userName = taikhoan.userName;
						db.KhachHangs.Add(khachHang);
						db.SaveChanges();
						SessionHelper.SetSession(taikhoan);
						return RedirectToAction("Index", "Home");
					}
					SessionHelper.SetSession(taikhoan);
					return RedirectToAction("Index", "Home");
				}
			}
			return View();
		}

		public ActionResult ForgetAccount()
		{
			return View();
		}
		public ActionResult LogOut()
		{
			Session.RemoveAll();
			return RedirectToAction("Login", "Login");
		}
	}
}