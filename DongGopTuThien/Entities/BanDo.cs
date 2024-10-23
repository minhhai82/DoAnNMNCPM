using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class BanDo
{
    public int IdbanDo { get; set; }

    public DateTime NgayCapNhat { get; set; }

    public int TrangThai { get; set; }
}
