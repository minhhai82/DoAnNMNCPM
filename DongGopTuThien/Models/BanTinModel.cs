using System.ComponentModel.DataAnnotations;
namespace DongGopTuThien.Models;

public class BanTinModel
{
    public int IdbanTin { get; set; }

    public string TieuDe { get; set; } = null!;

    public string Nguon { get; set; } = null!;

    public string NoiDungNgan { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime NgayCapNhat { get; set; }
}

public class BanTinRequest
{
    [Required]
    public string TieuDe { get; set; } = null!;
    [Required]
    public string Nguon { get; set; } = null!;
    [Required]
    public string NoiDungNgan { get; set; } = null!;
    [Required]
    public string NoiDung { get; set; } = null!;
    [Required]
    public DateTime NgayCapNhat { get; set; }
}