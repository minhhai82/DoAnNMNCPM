using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class BinhLuan
{
    public int IdbinhLuan { get; set; }

    public int IdbanTin { get; set; }

    public int IdnguoiBinhLuan { get; set; }

    public string NoiDung { get; set; } = null!;

    public DateTime NgayBinhLuan { get; set; }

    public virtual BanTin IdbanTinNavigation { get; set; } = null!;

    public virtual NguoiDung IdnguoiBinhLuanNavigation { get; set; } = null!;
}
