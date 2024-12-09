using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DongGopTuThien.Entities;
using DongGopTuThien.Models;

namespace DongGopTuThien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhanHoiController : ControllerBase
    {
        private readonly DaQltuThienContext _context;

        public PhanHoiController(DaQltuThienContext context)
        {
            _context = context;
        }

        // GET: api/PhanHois
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhanHoiModel>>> GetPhanHois()
        {
            var list = await _context.PhanHoiDanhGia.ToListAsync();
            return Utilities.ConvertToDtoList<PhanHoiDanhGia, PhanHoiModel>(list);
        }


        // GET: api/PhanHoiByChienDich
        [HttpGet("ByChienDich")]
        public async Task<ActionResult<IEnumerable<PhanHoiModel>>> GetPhanHoisByChienDich(int idChienDich)
        {
            var query = from ph in _context.PhanHoiDanhGia
                        join nd in _context.NguoiDungs
                        on ph.IdnguoiPhanHoi equals nd.IdnguoiDung into phanHoiGroup
                        from nd in phanHoiGroup.DefaultIfEmpty()
                        where ph.IdchienDich == idChienDich
                        select new
                        {
                            idBinhLuan = ph.IdphanHoi,
                            idChienDich = ph.IdchienDich,
                            idNguoiDung = ph.IdnguoiPhanHoi,
                            tenNguoiDung = nd.TenDayDu,
                            noiDung = ph.NoiDung,
                            ngayPhanHoi = ph.NgayPhanHoi
                        };
            var list = await query.ToListAsync();
            return Ok(list);
        }

        // GET: api/BinhLuan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhanHoiModel>> GetPhanHoi(int id)
        {
            var phanHoi = await _context.PhanHoiDanhGia.FindAsync(id);

            if (phanHoi == null)
            {
                return NotFound();
            }

            return Utilities.ConvertToDto<PhanHoiDanhGia, PhanHoiModel>(phanHoi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBinhLuan(int id, PhanHoiModel request)
        {
            if (id != request.IdphanHoi)
            {
                return BadRequest();
            }
            //Validate params
            if (request == null ||request.IdchienDich <= 0 ||request.IdnguoiPhanHoi <= 0)
            {
                return BadRequest("Invalid");
            }
            
            var cd = await _context.PhanHoiDanhGia.FirstOrDefaultAsync(c => c.IdphanHoi == id);
            if (cd == null )
            {
                return BadRequest();
            }

            try {
                //update bantin
                cd.NoiDung = request.NoiDung;
                cd.NgayPhanHoi = request.NgayPhanHoi;

                _context.PhanHoiDanhGia.Update(cd);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex) {
                return UnprocessableEntity(new { Error = ex.Message });
            }

            return Ok();
        }

        [HttpPost]
        [Authorize([0,1,2,3])]
        public async Task<ActionResult<PhanHoiModel>> PostBinhLuan([FromBody] PhanHoiRequest request)
        {
            //Validate params
            if (request == null ||
                request.IdnguoiPhanHoi<=0 ||
                request.IdchienDich <= 0 
                )
            {
                return BadRequest("Invalid");
            }
            var chienDich = await _context.ChienDiches.FindAsync(request.IdchienDich);
            if(chienDich == null)
            {
                return BadRequest("Chien dich not found");
            }
            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            if (nguoiDung == null || nguoiDung.IdnguoiDung != request.IdnguoiPhanHoi)
            {
                return BadRequest();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // create binhluan
                var cd = Utilities.ConvertToDto<PhanHoiRequest, PhanHoiDanhGia>(request);

                await _context.PhanHoiDanhGia.AddAsync(cd);
                await _context.SaveChangesAsync();
                // Complete the transaction
                await transaction.CommitAsync();

                return StatusCode(201, new { IdpPhanHoi = cd.IdphanHoi });
            }
        }

        [HttpDelete("{id}")]
        [Authorize([0,1,2,3])]
        public async Task<IActionResult> Delete(int id)
        {
            var phanHoi = await _context.PhanHoiDanhGia.FindAsync(id);
            if (phanHoi == null){
                return NotFound();
            }

            _context.PhanHoiDanhGia.Remove(phanHoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
