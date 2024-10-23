using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class BanDoChiTiet
{
    public int IdchiTiet { get; set; }

    public int IdbanDo { get; set; }

    public double KinhDo { get; set; }

    public double ViDo { get; set; }

    public decimal GiaTri1 { get; set; }

    public decimal? GiaTri2 { get; set; }

    public decimal? GiaTri3 { get; set; }

    public decimal? GiaTri4 { get; set; }

    public decimal? GiaTri5 { get; set; }
}
