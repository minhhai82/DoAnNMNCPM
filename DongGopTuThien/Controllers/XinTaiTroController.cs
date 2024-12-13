using DongGopTuThien.Entities;
using DongGopTuThien.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DongGopTuThien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XinTaiTroController : ControllerBase
    {
        private DaQltuThienContext _context;
        public XinTaiTroController(DaQltuThienContext ctx)
        {
            _context = ctx;
        }

        // GET: api/XinTaiTro/5
        [Authorize([0, 2, 3])]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetXinTaiTro(int id)
        {
            var xinTaiTro = await _context.XinTaiTros
                .Where( x => x.IdxinTaiTro == id)
                .Select( x => new
                {
                    x.IdxinTaiTro,
                    x.PhanHoi,
                    x.TrangThai,
                    hinhAnh = x.TaiTros
                        .Where(t => t.IdxinTaiTro == x.IdxinTaiTro)
                        .Select(t => t.HinhAnh)
                        .FirstOrDefault(),
                    ngayTaiTro = x.TaiTros
                        .Where(t => t.IdxinTaiTro == x.IdxinTaiTro)
                        .Select(t => t.NgayTaiTro)
                        .FirstOrDefault()
                }
                )
                .FirstOrDefaultAsync();

            if (xinTaiTro == null)
            {
                return NotFound();
            }

            return Ok(xinTaiTro);

            //return Utilities.ConvertToDto<XinTaiTro, XinTaiTroModel>(xinTaiTro);
        }

        // GET: api/XinTaiTro/GetXinTaiTrosByChienDich
        [Authorize([0, 1, 2, 3])]
        [HttpGet("ByChienDich")]
        public async Task<ActionResult<IEnumerable<XinTaiTro>>> GetXinTaiTrosByChienDich(int idChienDich)
        {
            var dg = await _context.XinTaiTros
                .Include(p => p.IdchienDichNavigation)
                .Include(p => p.IdnguoiNhanNavigation)
                .Include(p => p.TaiTros)
                .Where(e => e.IdchienDich == idChienDich)
                .Select(d => new
                {
                    IdXinTaiTro = d.IdxinTaiTro,
                    IdNguoiNhan = d.IdnguoiNhan,
                    IdchienDich = d.IdchienDich,
                    NoiDung = d.NoiDung,
                    PhanHoi = d.PhanHoi,
                    TenChuTaiKhoan = d.TenChuTaiKhoan,
                    TrangThai = d.TrangThai,
                    TenNganHang = d.TenNganHang,
                    SwiftCode = d.SwiftCode,
                    SoTaiKhoan = d.SoTaiKhoan,
                    NgayTaiTro = d.TaiTros
                        .Where(t => t.IdxinTaiTro == d.IdxinTaiTro)
                        .Select(t => t.NgayTaiTro.ToString("yyyy-MM-dd HH:mm:ss"))
                        .FirstOrDefault(),
                    SoTien = d.TaiTros
                        .Where(t => t.IdxinTaiTro == d.IdxinTaiTro)
                        .Select(t => t.SoTien)
                        .FirstOrDefault(),
                    TenNguoiNhan = d.IdnguoiNhanNavigation.TenDayDu,
                    TenChienDich = d.IdchienDichNavigation.Ten
                })
                .ToListAsync();
            return Ok(dg);
        }

        // GET: api/XinTaiTro/GetXinTaiTrosByByNguoiXin
        [Authorize([0, 2, 3])]
        [HttpGet("ByNguoiXin")]
        public async Task<ActionResult<IEnumerable<XinTaiTro>>> GetXinTaiTrosByByNguoiXin(int idNguoiXin)
        {
            var dg = await _context.XinTaiTros
                .Include(p => p.IdchienDichNavigation)
                .Include(p => p.IdnguoiNhanNavigation)
                .Where(e => e.IdnguoiNhan == idNguoiXin)
                .Select(d => new
                {
                    IdXinTaiTro = d.IdxinTaiTro,
                    IdNguoiNhan = d.IdnguoiNhan,
                    IdchienDich = d.IdchienDich,
                    NoiDung = d.NoiDung,
                    PhanHoi = d.PhanHoi,
                    TenChuTaiKhoan = d.TenChuTaiKhoan,
                    TrangThai = d.TrangThai,
                    TenNganHang = d.TenNganHang,
                    SwiftCode = d.SwiftCode,
                    SoTaiKhoan = d.SoTaiKhoan,
                    TenNguoiNhan = d.IdnguoiNhanNavigation.TenDayDu,
                    TenChienDich = d.IdchienDichNavigation.Ten
                })
                .ToListAsync();
            return Ok(dg);
        }


        [Authorize([2])]
        [HttpPost]
        public async Task<ActionResult<int>> PostXinTaiTro([FromBody] CreateXinTaiTro xinTaiTro)
        {
            //Validate params
            if (
                xinTaiTro == null ||
                xinTaiTro.IdchienDich <= 0 ||
                xinTaiTro.IdnguoiNhan <= 0
                )
            {
                return BadRequest("Invalid");
            }
            var chienDich = await _context.ChienDiches.FindAsync(xinTaiTro.IdchienDich);
            if (chienDich == null)
            {
                return BadRequest("Chien Dich not found");
            }
            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung == null || nguoiDung.IdnguoiDung != xinTaiTro.IdnguoiNhan)
            {
                return BadRequest();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // create chiendich
                var cd = new XinTaiTro
                {
                    IdchienDich = xinTaiTro.IdchienDich,
                    IdnguoiNhan = nguoiDung.IdnguoiDung,
                    NoiDung = xinTaiTro.NoiDung,                   
                    TrangThai = (int)TrangThaiXinTaiTro.ChoDuyet,
                    TenNganHang = xinTaiTro.TenNganHang,
                    TenChuTaiKhoan = xinTaiTro.TenChuTaiKhoan,
                    SoTaiKhoan = xinTaiTro.SoTaiKhoan,
                    SwiftCode = xinTaiTro.SwiftCode
                };

                await _context.XinTaiTros.AddAsync(cd);
                await _context.SaveChangesAsync();
                // Complete the transaction
                await transaction.CommitAsync();

                return StatusCode(201, new { IdDongGop = cd.IdxinTaiTro });
            }
        }

        [Authorize([3])]
        [HttpPut("verifyXinTaiTro")]
        public async Task<IActionResult> VerifyXinTaiTro([FromBody] VerifyXinTaiTroRequest request)
        {
            if (request == null || request.IdxinTaiTro <= 0)
            {
                return BadRequest();
            }

            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung.TrangThai != (int)TrangThai.XacThucGiayPhep)
            {
                return Unauthorized("To chuc chua xac thuc!");
            }

            var xinTaiTro = await _context.XinTaiTros.FindAsync(request.IdxinTaiTro);

            if (xinTaiTro == null)
            {
                return BadRequest();
            }

            var chienDich = await _context.ChienDiches.FindAsync(xinTaiTro.IdchienDich);
            if (chienDich == null || chienDich.IdtoChuc != nguoiDung.IdnguoiDung)
            {
                return BadRequest();
            }
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // Cập nhật phản hồi và trạng thái của Xin Tài Trợ
                    xinTaiTro.TrangThai = request.ChapThuan ? (int)TrangThaiXinTaiTro.DuocDuyet : (int)TrangThaiXinTaiTro.TuChoi;
                    xinTaiTro.PhanHoi = request.PhanHoi;
                    _context.XinTaiTros.Update(xinTaiTro);
                    
                    if (request.ChapThuan)
                    {
                        var taiTro = new TaiTro()
                        {
                            IdnguoiNhan = xinTaiTro.IdnguoiNhan,
                            IdchienDich = xinTaiTro.IdchienDich,
                            IdxinTaiTro = xinTaiTro.IdxinTaiTro,
                            NgayTaiTro = DateTime.Now,
                            SoTien = request.SoTien,
                            HinhAnh = request.HinhAnh,
                            GhiChu = $"{xinTaiTro.PhanHoi} - {xinTaiTro.IdchienDich} - {xinTaiTro.IdnguoiNhan}"
                        };
                        _context.TaiTros.Add(taiTro);

                        chienDich.ThucChi += request.SoTien;

                        _context.ChienDiches.Update(chienDich);
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return UnprocessableEntity();
            }
        }
    }
}
