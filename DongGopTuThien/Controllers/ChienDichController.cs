using DongGopTuThien.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BCrypt.Net;

namespace DongGopTuThien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChienDichController : ControllerBase
    {
        private DaQltuThienContext _context;
        private readonly IJwtService _jwtService;

        public ChienDichController(DaQltuThienContext ctx, IJwtService jwtService)
        {
            _context = ctx;
            _jwtService = jwtService;
           
        }


        // API get/search ChienDich (public API, not authorize)
        // GET api/ChienDich
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetChienDichsRequest request)
        {
            // Apply any filtering logic here using the `request` object if needed
            var chienDiches = _context.ChienDiches.ToList();

            return Ok(chienDiches);
        }

        // API get ChienDich (public API, not authorize)
        // GET api/ChienDich/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var chienDich = await _context.ChienDiches.FindAsync(id);
            if (chienDich == null)
            {
                return NotFound();
            }

            return Ok(chienDich);

        }

        // API for ToChuc to create ChienDich
        // POST api/ChienDich
        // {
        //     "Ten": "Yagi01",
        //     "NoiDung": "Nội dung chiến dịch",
        //     "NgayBatDau": "2024-01-01",
        //     "NgayKetThuc": "2024-12-31",
        //     "NganSachDuKien": 1000000,
        //     "TaiKhoan": {
        //         "TenNganHang": "Vietcombank",
        //         "TenChuTaiKhoan": "Nguyen Van A",
        //         "SoTaiKhoan": "123456789",
        //         "SwiftCode": "VCBVVNVX"
        //     }
        // }
        [HttpPost]
        [Authorize([3])]
        public async Task<IActionResult> Post([FromBody] CreateChienDichRequest request)
        {

            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung.TrangThai != TrangThai.XacThucGiayPhep)
            {
                return Unauthorized("To chuc chua xac thuc!");
            }

            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // Create TaiKhoan
                    var tk = new TaiKhoan
                    {
                        IdtoChuc = nguoiDung.IdnguoiDung,
                        TenNganHang = request.TaiKhoan.TenNganHang,
                        TenChuTaiKhoan = request.TaiKhoan.TenChuTaiKhoan,
                        SoTaiKhoan = request.TaiKhoan.SoTaiKhoan,
                        SwiftCode = request.TaiKhoan.SwiftCode,
                    };

                    _context.TaiKhoans.Add(tk);
                    await _context.SaveChangesAsync();

                    // create chiendich
                    var cd = new ChienDich
                    {
                        Ten = request.Ten,
                        NoiDung = request.NoiDung,
                        NgayBatDau = DateTime.ParseExact(request.NgayBatDau, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToUniversalTime(),
                        NgayKetThuc = DateTime.ParseExact(request.NgayKetThuc, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToUniversalTime(),
                        NganSachDuKien = request.NganSachDuKien,
                        TrangThai = TrangThaiChienDich.Draft,
                        IdtoChuc = nguoiDung.IdnguoiDung
                    };

 
                    _context.ChienDiches.Add(cd);
                    await _context.SaveChangesAsync();

                    // create tk_cd

                    var tkcd = new TkchienDich
                    {
                        IdchienDich = cd.IdchienDich,
                        IdtaiKhoan = tk.IdtaiKhoan,
                    };

                    _context.TkchienDiches.Add(tkcd);
                    await _context.SaveChangesAsync();

                    // Complete the transaction
                    await transaction.CommitAsync();

                    return StatusCode(201, new { ChienDichId = cd.IdchienDich });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }

        }


        // API for ToChuc to edit ChienDich (thong tin, trang thai)
        //  PUT api/ChienDich/{id}
        //  {
        //     "Ten": "Yagi04",
        //     "NoiDung": "Nội dung chiến dịch",
        //     "NgayBatDau": "2024-01-01",
        //     "NgayKetThuc": "2024-12-31",
        //     "NganSachDuKien": 1000000,
        //     "TrangThai": 1
        // }
        [HttpPut("{id}")]
        [Authorize([3])]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateChienDichRequest request)
        {
            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung.TrangThai != TrangThai.XacThucGiayPhep)
            {
                return Unauthorized("To chuc chua xac thuc!");
            }

            var cd = await _context.ChienDiches.FirstOrDefaultAsync(c => c.IdchienDich == id && c.IdtoChuc == nguoiDung.IdnguoiDung);
            if (cd == null || cd.TrangThai == TrangThaiChienDich.Completed || cd.TrangThai == TrangThaiChienDich.Cancelled )
            {
                return BadRequest();
            }


            try {
                //update chiendich
                cd.Ten = request.Ten;
                cd.NoiDung = request.NoiDung;
                cd.NgayBatDau = DateTime.ParseExact(request.NgayBatDau, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToUniversalTime();
                cd.NgayKetThuc = DateTime.ParseExact(request.NgayKetThuc, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToUniversalTime();
                cd.NganSachDuKien = request.NganSachDuKien;

                if (request.TrangThai != null)
                {
                    cd.TrangThai = (TrangThaiChienDich)request.TrangThai;
                }

                _context.ChienDiches.Update(cd);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex) {
                return UnprocessableEntity(new { Error = ex.Message });
            }

            return Ok();

        }


        [HttpDelete("{id}")]
        [Authorize([3])]
        public async Task<IActionResult> Delete(int id)
        {
            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung.TrangThai != TrangThai.XacThucGiayPhep)
            {
                return Unauthorized("To chuc chua xac thuc!");
            }

            var cd = await _context.ChienDiches
                .FirstOrDefaultAsync(c => c.IdchienDich == id && c.IdtoChuc == nguoiDung.IdnguoiDung);

            if (cd == null || cd.TrangThai != TrangThaiChienDich.Draft)
            {
                return BadRequest();
            }

            _context.ChienDiches.Remove(cd);

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}