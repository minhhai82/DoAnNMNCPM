using System.ComponentModel.DataAnnotations;
namespace DongGopTuThien.Models;

public class BinhLuanModel
{
    public int IdbinhLuan { get; set; }

    public int IdbanTin { get; set; }

    public int IdnguoiBinhLuan { get; set; }

    public string NoiDung { get; set; } = null!;

    public DateTime NgayBinhLuan { get; set; }
}

public class BinhLuanRequest
{
    [Required]
    public int IdbanTin { get; set; }

    [Required]
    public int IdnguoiBinhLuan { get; set; }

    [Required]
    public string NoiDung { get; set; } = null!;

    [Required]
    public DateTime NgayBinhLuan { get; set; }
}