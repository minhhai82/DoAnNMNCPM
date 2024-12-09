using System.ComponentModel.DataAnnotations;
namespace DongGopTuThien.Models;

public class PhanHoiModel
{
    public int IdphanHoi { get; set; }

    public int IdnguoiPhanHoi { get; set; }

    public int IdchienDich { get; set; }

    public string NoiDung { get; set; } = null!;

    public int DanhGia { get; set; }

    public DateTime NgayPhanHoi { get; set; }
}

public class PhanHoiRequest
{
    [Required]
    public int IdchienDich { get; set; }

    [Required]
    public int IdnguoiPhanHoi { get; set; }

    [Required]
    public string NoiDung { get; set; } = null!;
    
    [Required]
    public int DanhGia { get; set; }

    [Required]
    public DateTime NgayPhanHoi { get; set; }
}