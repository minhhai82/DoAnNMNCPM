using System.ComponentModel.DataAnnotations;

public class DongGopModel
{
    public int IddongGop { get; set; }

    public int IdnguoiChuyen { get; set; }

    public int IdchienDich { get; set; }

    public DateTime NgayDongGop { get; set; }

    public decimal SoTien { get; set; }

    public byte[] HinhAnh { get; set; } = null!;

    public string GhiChu { get; set; } = null!;

    public int TrangThai { get; set; }
}

public class CreateDongGopRequest
{
    [Required]
    public int IdnguoiChuyen { get; set; }

    [Required]
    public int IdchienDich { get; set; }


    [Required]
    public DateTime NgayDongGop { get; set; }
        
    [Required]
    [Range(0, 1000000000)]
    public decimal SoTien { get; set; }

    [Required]
    public byte[] HinhAnh { get; set; }

    public string GhiChu { get; set; } = null!;
}

public class VerifyDongGopRequest
{
    public int IddongGop { get; set; }
}
