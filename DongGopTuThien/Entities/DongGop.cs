using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class DongGop
{
    public int IddongGop { get; set; }

    public int IdnguoiChuyen { get; set; }

    public int IdchienDich { get; set; }

    public DateTime NgayDongGop { get; set; }

    public decimal SoTien { get; set; }

    public byte[] HinhAnh { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public int TrangThai { get; set; }

    public virtual ChienDich IdchienDichNavigation { get; set; } = null!;

    public virtual NguoiDung IdnguoiChuyenNavigation { get; set; } = null!;
}
