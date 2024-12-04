using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DongGopTuThien.Entities;
using DongGopTuThien.Models;

namespace DongGopTuThien.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class DongGopsController : ControllerBase
    {
        private readonly DaQltuThienContext _context;

        public DongGopsController(DaQltuThienContext context)
        {
            _context = context;
        }

        // GET: api/DongGops
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DongGop>>> GetDongGops()
        {
            return await _context.DongGops.ToListAsync();
        }


        // GET: api/DongGopsByChienDich
        [HttpGet("ByChienDich")]
        public async Task<ActionResult<IEnumerable<DongGop>>> GetDongGopsByChienDich(int idChienDich)
        {
            return await _context.DongGops.Where(e => e.IdchienDich == idChienDich)
                                          .ToListAsync();
        }


        // GET: api/DongGopsByChienDich
        [HttpGet("ByNguoiTaiTro")]
        public async Task<ActionResult<IEnumerable<DongGop>>> GetDongGopsByNguoiTaiTro(int idNguoiTaiTro)
        {
            return await _context.DongGops.Where(e => e.IdnguoiChuyen == idNguoiTaiTro)
                                          .ToListAsync();
        }

        // GET: api/GetTop10DongGops
        [HttpGet("Top10DongGops")]
        public async Task<ActionResult<IEnumerable<DongGopModel>>> GetTop10DongGops()
        {
            var list = await _context.DongGops.OrderByDescending(e => e.NgayDongGop)
                                          .Take(10)              
                                          .ToListAsync();
            List<DongGopModel> retList = new List<DongGopModel>();
            foreach (var donggop in list)
            {
                retList.Add(new DongGopModel()
                {
                    IddongGop = donggop.IddongGop, 
                    IdnguoiChuyen = donggop.IdnguoiChuyen, 
                    IdchienDich = donggop.IdchienDich,
                    NgayDongGop = donggop.NgayDongGop,
                    SoTien = donggop.SoTien,
                    GhiChu = donggop.GhiChu,
                    TrangThai = donggop.TrangThai,
                    TenNguoiChuyen  = donggop.IdnguoiChuyenNavigation.TenDayDu,
                    TenChienDich  = donggop.IdchienDichNavigation.Ten
                });
            } 
            return retList;
        }

        // GET: api/GetTop10DongGopsByChienDich
        [HttpGet("Top10DongGopsByChienDich")]
        public async Task<ActionResult<IEnumerable<DongGop>>> GetTop10DongGopsByChienDich(int idChienDich)
        {
            return await _context.DongGops.Where(e => e.IdchienDich == idChienDich).OrderByDescending(e => e.NgayDongGop)
                                                .Take(10)
                                                .ToListAsync();
        }

        // GET: api/GetTop10DongGopsByNguoiTaiTtro
        [HttpGet("Top10DongGopsByNguoiTaiTtro")]
        public async Task<ActionResult<IEnumerable<DongGop>>> GetTop10DongGopsByNguoiTaiTtro(int idNguoiTaiTro)
        {
            return await _context.DongGops.Where(e => e.IdnguoiChuyen == idNguoiTaiTro).OrderByDescending(e => e.NgayDongGop)
                                                .Take(10)
                                                .ToListAsync();
        }

        // GET: api/DongGops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DongGop>> GetDongGop(int id)
        {
            var dongGop = await _context.DongGops.FindAsync(id);

            if (dongGop == null)
            {
                return NotFound();
            }

            return dongGop;
        }

        [Authorize([0, 1, 3])]
        [HttpPut("verifyDongGop")]
        public async Task<IActionResult> VerifyDongGop([FromBody] VerifyDongGopRequest request)
        {
            if (request == null || request.IddongGop <= 0)
            {
                return BadRequest();
            }

            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung.TrangThai != (int)TrangThai.XacThucGiayPhep)
            {
                return Unauthorized("To chuc chua xac thuc!");
            }

            var dongGop = await _context.DongGops.FindAsync(request.IddongGop);

            if (dongGop == null || dongGop.IddongGop != request.IddongGop)
            {
                return BadRequest();
            }

            var chienDich = await _context.ChienDiches.FindAsync(dongGop.IdchienDich);
            if (chienDich == null || chienDich.IdtoChuc != nguoiDung.IdnguoiDung)
            {
                return BadRequest();
            }
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    dongGop.TrangThai = (int)TrangThaiDongGop.DaDuyet;

                    _context.DongGops.Update(dongGop);

                    await _context.SaveChangesAsync();

                    chienDich.ThucThu += dongGop.SoTien;

                    _context.ChienDiches.Update(chienDich);

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

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutDongGop(int id, DongGop dongGop)
        //{
        //    if (id != dongGop.IddongGop)
        //    {
        //        return BadRequest();
        //    }
        //    //Validate params
        //    if (
        //        dongGop == null ||
        //        dongGop.IdchienDich <= 0 ||
        //        dongGop.IddongGop <= 0 ||
        //        dongGop.IdnguoiChuyen <= 0 ||
        //        dongGop.SoTien <= 0
        //        )
        //    {
        //        return BadRequest("Invalid");
        //    }

        //    _context.Entry(dongGop).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DongGopExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}
        [Authorize([0, 1, 3])]
        [HttpPost]
        public async Task<ActionResult<DongGop>> PostDongGop([FromBody] CreateDongGopRequest dongGop)
        {
            //Validate params
            if (
                dongGop == null ||
                dongGop.IdchienDich<=0 ||
                dongGop.SoTien <= 0 
                )
            {
                return BadRequest("Invalid");
            }
            var chienDich = await _context.ChienDiches.FindAsync(dongGop.IdchienDich);
            if(chienDich == null)
            {
                return BadRequest("Chien Dich not found");
            }
            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung == null)
            {
                return BadRequest();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // create chiendich
                var cd = new DongGop
                {
                    IdchienDich = dongGop.IdchienDich,
                    IdnguoiChuyen = nguoiDung.IdnguoiDung,
                    NgayDongGop = dongGop.NgayDongGop,
                    SoTien = dongGop.SoTien,
                    TrangThai = (int)TrangThaiDongGop.ChoDuyet,
                    HinhAnh = dongGop.HinhAnh,
                    GhiChu = dongGop.GhiChu
                };

                await _context.DongGops.AddAsync(cd);
                await _context.SaveChangesAsync();
                // Complete the transaction
                await transaction.CommitAsync();

                return StatusCode(201, new { IdDongGop = cd.IddongGop });
            }
        }
        [Authorize([0, 1, 3])]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDongGop(int id)
        {
            var dongGop = await _context.DongGops.FindAsync(id);
            if (dongGop == null)
            {
                return NotFound();
            }

            _context.DongGops.Remove(dongGop);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DongGopExists(int id)
        {
            return _context.DongGops.Any(e => e.IddongGop == id);
        }
    }
}
