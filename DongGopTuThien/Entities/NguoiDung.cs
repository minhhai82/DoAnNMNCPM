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

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public byte[]? GiayPhep { get; set; }

    public int TrangThai { get; set; }

    public int Loai { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<ChienDich> ChienDiches { get; set; } = new List<ChienDich>();

    public virtual ICollection<DongGop> DongGops { get; set; } = new List<DongGop>();

    public virtual ICollection<PhanHoiDanhGium> PhanHoiDanhGia { get; set; } = new List<PhanHoiDanhGium>();

    public virtual ICollection<TaiTro> TaiTros { get; set; } = new List<TaiTro>();

    public virtual ICollection<XinTaiTro> XinTaiTros { get; set; } = new List<XinTaiTro>();
}
