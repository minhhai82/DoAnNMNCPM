using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class BanTin
{
    public int IdbanTin { get; set; }

    public string TieuDe { get; set; } = null!;

    public string Nguon { get; set; } = null!;

    public string NoiDungNgan { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime NgayCapNhat { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
}
