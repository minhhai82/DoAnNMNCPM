using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class XinTaiTro
{
    public int IdxinTaiTro { get; set; }

    public int IdnguoiNhan { get; set; }

    public int IdchienDich { get; set; }

    public string? NoiDung { get; set; }

    public string? PhanHoi { get; set; }

    public int TrangThai { get; set; }

    public virtual ChienDich IdchienDichNavigation { get; set; } = null!;

    public virtual NguoiDung IdnguoiNhanNavigation { get; set; } = null!;
}
