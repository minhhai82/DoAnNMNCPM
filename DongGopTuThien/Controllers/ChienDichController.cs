using DongGopTuThien.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DongGopTuThien.Models;

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
            var chienDiches = await _context.ChienDiches
                                      .Include(p => p.TaiKhoans)
                                      .ToListAsync();

            return Ok(chienDiches);
        }

        // GET api/ChienDich
        [HttpGet("GetChienDichByToChuc")]
        public async Task<ActionResult> GetChienDichByToChuc(int idToChuc)
        {
            var chienDiches = await _context.ChienDiches.Where(e => e.IdtoChuc == idToChuc)
                                             .Include(p => p.TaiKhoans)
                                             .ToListAsync();
            return Ok(chienDiches);

        }

        // API get ChienDich (public API, not authorize)
        // GET api/ChienDich/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var chienDich = await _context.ChienDiches.Include(p => p.TaiKhoans).FirstOrDefaultAsync(p => p.IdchienDich == id);

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

            if (nguoiDung.TrangThai != (int)TrangThai.XacThucGiayPhep)
            {
                return Unauthorized("To chuc chua xac thuc!");
            }

            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // create chiendich
                    var cd = new ChienDich
                    {
                        Ten = request.Ten,
                        NoiDung = request.NoiDung,
                        NgayBatDau = DateTime.ParseExact(request.NgayBatDau, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToUniversalTime(),
                        NgayKetThuc = DateTime.ParseExact(request.NgayKetThuc, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToUniversalTime(),
                        NganSachDuKien = request.NganSachDuKien,
                        TrangThai = (int)TrangThaiChienDich.Draft,
                        IdtoChuc = nguoiDung.IdnguoiDung
                    };


                    _context.ChienDiches.Add(cd);
                    await _context.SaveChangesAsync();

                    // Create TaiKhoan
                    var tk = new TaiKhoan
                    {
                        //IdtoChuc = nguoiDung.IdnguoiDung,
                        IdchienDich = cd.IdchienDich,
                        TenNganHang = request.TaiKhoan.TenNganHang,
                        TenChuTaiKhoan = request.TaiKhoan.TenChuTaiKhoan,
                        SoTaiKhoan = request.TaiKhoan.SoTaiKhoan,
                        SwiftCode = request.TaiKhoan.SwiftCode,
                    };

                    _context.TaiKhoans.Add(tk);
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

            if (nguoiDung.TrangThai != (int)TrangThai.XacThucGiayPhep)
            {
                return Unauthorized("To chuc chua xac thuc!");
            }

            var cd = await _context.ChienDiches.Include(p => p.TaiKhoans).FirstOrDefaultAsync(c => c.IdchienDich == id && c.IdtoChuc == nguoiDung.IdnguoiDung);
            if (cd == null || cd.TrangThai == (int)TrangThaiChienDich.Completed || cd.TrangThai == (int)TrangThaiChienDich.Cancelled )
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

                if (request.TrangThai >0)
                {
                    cd.TrangThai = (int)(TrangThaiChienDich)request.TrangThai;
                }

                var tk = cd.TaiKhoans.FirstOrDefault(p => p.IdtaiKhoan == request.TaiKhoan.IdtaiKhoan);

                if (tk != null)
                {
                    tk.TenNganHang = request.TaiKhoan.TenNganHang;
                    tk.TenChuTaiKhoan = request.TaiKhoan.TenChuTaiKhoan;
                    tk.SoTaiKhoan = request.TaiKhoan.SoTaiKhoan;
                    tk.SwiftCode = request.TaiKhoan.SwiftCode;
                }
                else
                {
                    tk = new TaiKhoan
                    {
                        IdchienDich = cd.IdchienDich,
                        TenNganHang = request.TaiKhoan.TenNganHang,
                        TenChuTaiKhoan = request.TaiKhoan.TenChuTaiKhoan,
                        SoTaiKhoan = request.TaiKhoan.SoTaiKhoan,
                        SwiftCode = request.TaiKhoan.SwiftCode,
                    };
                    cd.TaiKhoans.Add(tk);
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

            if (nguoiDung.TrangThai != (int)TrangThai.XacThucGiayPhep)
            {
                return Unauthorized("To chuc chua xac thuc!");
            }

            var cd = await _context.ChienDiches
                .FirstOrDefaultAsync(c => c.IdchienDich == id && c.IdtoChuc == nguoiDung.IdnguoiDung);

            if (cd == null || cd.TrangThai != (int)TrangThaiChienDich.Draft)
            {
                return BadRequest();
            }

            _context.ChienDiches.Remove(cd);

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}