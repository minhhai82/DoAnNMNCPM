using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DongGopTuThien.Entities;
using DongGopTuThien.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace DongGopTuThien.Controllers
{
    //[Authorize([0, 1])]
    [Route("api/[controller]")]
    [ApiController]
    public class BinhLuanController : ControllerBase
    {
        private readonly DaQltuThienContext _context;

        public BinhLuanController(DaQltuThienContext context)
        {
            _context = context;
        }

        // GET: api/BinhLuans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BinhLuanModel>>> GetBinhLuans()
        {
            var list = await _context.BinhLuans.ToListAsync();
            return Utilities.ConvertToDtoList<BinhLuan, BinhLuanModel>(list);
        }


        // GET: api/BinhLuansByBanTin
        [HttpGet("ByBanTin")]
        public async Task<ActionResult<IEnumerable<BinhLuanModel>>> GetBinhLuansByBanTin(int idBanTin)
        {
            var query = from bl in _context.BinhLuans
                        join nd in _context.NguoiDungs
                        on bl.IdnguoiBinhLuan equals nd.IdnguoiDung into binhLuanGroup
                        from nd in binhLuanGroup.DefaultIfEmpty()
                        where bl.IdbanTin == idBanTin
                        select new
                        {
                            idBinhLuan = bl.IdbinhLuan,
                            idBanTin = bl.IdbanTin,
                            tenNguoiDung = nd.TenDayDu,
                            noiDung = bl.NoiDung,
                            ngayBinhLuan = bl.NgayBinhLuan
                        };
            var list = await query.ToListAsync();
            return Ok(list);
            //var list = await _context.BinhLuans
            //    .Where(e => e.IdbanTin == idBanTin)
            //    .ToListAsync();
            //return Utilities.ConvertToDtoList<BinhLuan, BinhLuanModel>(list);
        }


        // GET: api/BinhLuansByChienDich
        [HttpGet("ByNguoiDung")]
        public async Task<ActionResult<IEnumerable<BinhLuanModel>>> GetBinhLuansByNguoiDung(int idNguoiDung)
        {
            var list =  await _context.BinhLuans.Where(e => e.IdnguoiBinhLuan == idNguoiDung)
                                          .ToListAsync();
            return Utilities.ConvertToDtoList<BinhLuan, BinhLuanModel>(list);
        }

        // GET: api/GetTop10BinhLuans
        [HttpGet("Top10BinhLuans")]
        public async Task<ActionResult<IEnumerable<BinhLuanModel>>> GetTop10BinhLuans()
        {
            var list = await _context.BinhLuans.OrderByDescending(e => e.NgayBinhLuan)
                                          .Take(10)              
                                          .ToListAsync();
            return Utilities.ConvertToDtoList<BinhLuan, BinhLuanModel>(list);
        }

        // GET: api/GetTop10BinhLuansByBanTin
        [HttpGet("Top10BinhLuansByBanTin")]
        public async Task<ActionResult<IEnumerable<BinhLuanModel>>> GetTop10BinhLuansByBanTin(int idBanTin)
        {
            var list = await _context.BinhLuans.Where(e => e.IdbanTin == idBanTin).OrderByDescending(e => e.NgayBinhLuan)
                                                .Take(10)
                                                .ToListAsync();
            return Utilities.ConvertToDtoList<BinhLuan, BinhLuanModel>(list);
        }

        // GET: api/GetTop10BinhLuansByNguoiDung
        [HttpGet("Top10BinhLuansByNguoiDung")]
        public async Task<ActionResult<IEnumerable<BinhLuanModel>>> GetTop10BinhLuansByNguoiDung(int idNguoiDung)
        {
            var list =  await _context.BinhLuans.Where(e => e.IdnguoiBinhLuan == idNguoiDung).OrderByDescending(e => e.NgayBinhLuan)
                                                .Take(10)
                                                .ToListAsync();
            return Utilities.ConvertToDtoList<BinhLuan, BinhLuanModel>(list);
        }

        // GET: api/BinhLuan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BinhLuanModel>> GetBinhLuan(int id)
        {
            var binhLuan = await _context.BinhLuans.FindAsync(id);

            if (binhLuan == null)
            {
                return NotFound();
            }

            return Utilities.ConvertToDto<BinhLuan, BinhLuanModel>(binhLuan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBinhLuan(int id, BinhLuan request)
        {
            if (id != request.IdbinhLuan)
            {
                return BadRequest();
            }
            //Validate params
            if (request == null ||request.IdbanTin <= 0 ||request.IdnguoiBinhLuan <= 0)
            {
                return BadRequest("Invalid");
            }
            
            var cd = await _context.BinhLuans.FirstOrDefaultAsync(c => c.IdbinhLuan == id);
            if (cd == null )
            {
                return BadRequest();
            }

            try {
                //update bantin
                cd.NoiDung = request.NoiDung;
                cd.NgayBinhLuan = request.NgayBinhLuan;

                _context.BinhLuans.Update(cd);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex) {
                return UnprocessableEntity(new { Error = ex.Message });
            }

            return Ok();
        }

        [HttpPost]
        [Authorize([0,1,2,3])]
        public async Task<ActionResult<DongGop>> PostBinhLuan([FromBody] BinhLuanRequest request)
        {
            //Validate params
            if (request == null ||
                request.IdnguoiBinhLuan<=0 ||
                request.IdbanTin <= 0 
                )
            {
                return BadRequest("Invalid");
            }
            var banTin = await _context.BanTins.FindAsync(request.IdbanTin);
            if(banTin == null)
            {
                return BadRequest("Ban tin not found");
            }
            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung == null || nguoiDung.IdnguoiDung != request.IdnguoiBinhLuan)
            {
                return BadRequest();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // create binhluan
                var cd = Utilities.ConvertToDto<BinhLuanRequest, BinhLuan>(request);

                await _context.BinhLuans.AddAsync(cd);
                await _context.SaveChangesAsync();
                // Complete the transaction
                await transaction.CommitAsync();

                return StatusCode(201, new { IdBinhLuan = cd.IdbinhLuan });
            }
        }

        [HttpDelete("{id}")]
        [Authorize([0,1,2,3])]
        public async Task<IActionResult> Delete(int id)
        {
            var binhLuan = await _context.BinhLuans.FindAsync(id);
            if (binhLuan == null){
                return NotFound();
            }

            _context.BinhLuans.Remove(binhLuan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
