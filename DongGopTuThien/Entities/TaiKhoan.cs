﻿using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class TaiKhoan
{
    public int IdtaiKhoan { get; set; }

    public int IdchienDich { get; set; }

    public string TenNganHang { get; set; } = null!;

    public string TenChuTaiKhoan { get; set; } = null!;

    public string SoTaiKhoan { get; set; } = null!;

    public string? SwiftCode { get; set; }

    public virtual ChienDich IdchienDichNavigation { get; set; } = null!;
}
