using System;
using System.Collections.Generic;

namespace DongGopTuThien.Entities;

public partial class TkchienDich
{
    public int IdchienDich { get; set; }

    public int IdtaiKhoan { get; set; }

    public string? GhiChu { get; set; }

    public virtual ChienDich IdchienDichNavigation { get; set; } = null!;
}
