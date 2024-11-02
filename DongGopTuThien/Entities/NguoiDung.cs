using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class NguoiDung
{
    public int IdnguoiDung { get; set; }

    public string TenDayDu { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string DienThoai { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string TenDangNhap { get; set; }

    public string MatKhau { get; set; } = null!;

    public byte[]? GiayPhep { get; set; }

    public TrangThai TrangThai { get; set; }
    public Loai Loai { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<ChienDich> ChienDiches { get; set; } = new List<ChienDich>();

    public virtual ICollection<DongGop> DongGops { get; set; } = new List<DongGop>();

    public virtual ICollection<PhanHoiDanhGium> PhanHoiDanhGia { get; set; } = new List<PhanHoiDanhGium>();

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

    public virtual ICollection<TaiTro> TaiTros { get; set; } = new List<TaiTro>();

    public virtual ICollection<XinTaiTro> XinTaiTros { get; set; } = new List<XinTaiTro>();
}

public enum TrangThai {
    ChuaXacThuc = 0,
    XacThucDienThoai = 1,
    XacThucGiayPhep = 2,

}

public enum Loai {
    Admin = 0,
    NguoiDongGop = 1,

    NguoiXinTaiTro = 2,

    ToChucTuThien = 3,
}