using DongGopTuThien.Entities;
using DongGopTuThien.Models;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

public class GetChienDichsRequest 
{
    public string? Query { get; set; }
    public string? NgayBatDau { get; set; }
    public string? NgayKetThuc { get; set; }
    public GetChienDichsRequest()
    {
        // default startdate and enddate
        NgayBatDau ??= DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
        NgayKetThuc ??= DateTime.Now.ToString("yyyy-MM-dd");
    }
}

public class CreateChienDichRequest
{

    [Required]
    public string Ten { get; set; }

    [Required]
    public string NoiDung { get; set; }


    [Required]
    public string NgayBatDau { get; set; }

    [Required]
    public string NgayKetThuc { get; set; }

    
    [Required]
    [Range(0, 1000000000)]
    public decimal NganSachDuKien { get; set; }

    [Required]
    public ThongTinTaiKhoan TaiKhoan { get; set; }

    public int TrangThai { get; set; }
}

public class ThongTinTaiKhoan
{

    public int IdtaiKhoan { get; set; }

    [Required]
    public string TenNganHang { get; set; } 

    [Required]
    public string TenChuTaiKhoan { get; set; }
    [Required]
    public string SoTaiKhoan { get; set; }
    [Required]
    public string SwiftCode { get; set; }
}


public class UpdateChienDichRequest
{

    [Required]
    public string Ten { get; set; }

    [Required]
    public string NoiDung { get; set; }


    [Required]
    public string NgayBatDau { get; set; }

    [Required]
    public string NgayKetThuc { get; set; }

    
    [Required]
    [Range(0, 1000000000)]
    public decimal NganSachDuKien { get; set; }

    public int TrangThai { get; set; }

    public ThongTinTaiKhoan TaiKhoan { get; set; }
}


public class ChienDichDTO
{
    public int Id { get; set; }
    public string Ten { get; set; }
    public string NoiDung { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public decimal NganSachDuKien { get; set; }
    public TrangThaiChienDich TrangThai { get; set; }
    public ThongTinTaiKhoan[] TaiKhoans { get; set; }

}