using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class PhanHoiDanhGia
{
    public int IdphanHoi { get; set; }

    public int IdnguoiPhanHoi { get; set; }

    public int IdchienDich { get; set; }

    public string NoiDung { get; set; } = null!;

    public int DanhGia { get; set; }

    public DateTime NgayPhanHoi { get; set; }

    public virtual ChienDich IdchienDichNavigation { get; set; } = null!;

    public virtual NguoiDung IdnguoiPhanHoiNavigation { get; set; } = null!;
}
