using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class TaiKhoan
{
    public int IdtaiKhoan { get; set; }

    public int IdtoChuc { get; set; }

    public string TenNganHang { get; set; } = null!;

    public string TenChuTaiKhoan { get; set; } = null!;

    public string SoTaiKhoan { get; set; } = null!;

    public string? SwiftCode { get; set; }

    public virtual NguoiDung IdtoChucNavigation { get; set; } = null!;
}
