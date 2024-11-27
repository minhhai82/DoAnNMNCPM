using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class ChienDich
{
    public int IdchienDich { get; set; }

    public string Ten { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public decimal NganSachDuKien { get; set; }

    public decimal ThucThu { get; set; }

    public decimal ThucChi { get; set; }

    public int TrangThai { get; set; }

    public int IdtoChuc { get; set; }

    public virtual ICollection<DongGop> DongGops { get; set; } = new List<DongGop>();

    public virtual NguoiDung IdtoChucNavigation { get; set; } = null!;

    public virtual ICollection<PhanHoiDanhGia> PhanHoiDanhGia { get; set; } = new List<PhanHoiDanhGia>();

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

    public virtual ICollection<TaiTro> TaiTros { get; set; } = new List<TaiTro>();

    public virtual ICollection<XinTaiTro> XinTaiTros { get; set; } = new List<XinTaiTro>();
}
