using System.ComponentModel.DataAnnotations;

namespace DongGopTuThien.Models
{
    public class XinTaiTroModel
    {
        public int IdxinTaiTro { get; set; }

        public int IdnguoiNhan { get; set; }

        public int IdchienDich { get; set; }

        public string? NoiDung { get; set; }

        public string? PhanHoi { get; set; }

        public int TrangThai { get; set; }

        public string TenNganHang { get; set; } = null!;

        public string TenChuTaiKhoan { get; set; } = null!;

        public string SoTaiKhoan { get; set; } = null!;

        public string? SwiftCode { get; set; }
    }

    public class GetXinTaiTrosRequest
    {
        public string? Query { get; set; }
        public string? NgayBatDau { get; set; }
        public string? NgayKetThuc { get; set; }
        //public GetChienDichsRequest()
        //{
        //    // default startdate and enddate
        //    NgayBatDau ??= DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
        //    NgayKetThuc ??= DateTime.Now.ToString("yyyy-MM-dd");
        //}
    }

    public class CreateXinTaiTro
    {
        [Required]
        public int IdnguoiNhan { get; set; }

        [Required]
        public int IdchienDich { get; set; }

        [Required]
        public string? NoiDung { get; set; }

        public string? PhanHoi { get; set; }

        [Required]
        public string TenNganHang { get; set; } = null!;

        [Required]
        public string TenChuTaiKhoan { get; set; } = null!;

        [Required]
        public string SoTaiKhoan { get; set; } = null!;

        [Required]
        public string? SwiftCode { get; set; }
    }

    public class VerifyXinTaiTroRequest
    {
        [Required]
        public int IdxinTaiTro { get; set; }

        [Required]
        public string? PhanHoi { get; set; }

        [Required]
        public bool ChapThuan {  get; set; }

        public decimal SoTien { get; set; }

        public byte[] HinhAnh { get; set; } = null!;

    }
}