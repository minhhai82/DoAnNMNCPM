using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class TaiTro
{
    public int IdtaiTro { get; set; }

    public int? IdnguoiNhan { get; set; }

    public int IdchienDich { get; set; }

    public DateTime NgayTaiTro { get; set; }

    public decimal SoTien { get; set; }

    public byte[] HinhAnh { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public virtual ChienDich IdchienDichNavigation { get; set; } = null!;

    public virtual NguoiDung? IdnguoiNhanNavigation { get; set; }
}
