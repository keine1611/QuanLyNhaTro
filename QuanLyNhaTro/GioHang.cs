//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyNhaTro
{
    using System;
    using System.Collections.Generic;
    
    public partial class GioHang
    {
        public int id_KH { get; set; }
        public int id_P { get; set; }
        public string ghichu { get; set; }
    
        public virtual KhachHang KhachHang { get; set; }
        public virtual Phong Phong { get; set; }
    }
}