using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DongGopTuThien.Entities;
using DongGopTuThien.Models;

namespace DongGopTuThien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanTinController : ControllerBase
    {
        private readonly DaQltuThienContext _context;

        public BanTinController(DaQltuThienContext context)
        {
            _context = context;
        }

        // GET: api/BanTins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BanTinModel>>> GetBanTins()
        {
            var list = await _context.BanTins.ToListAsync();
            return Utilities.ConvertToDtoList<BanTin, BanTinModel>(list);
        }

        // GET: api/GetTop10BanTins
        [HttpGet("Top10BanTins")]
        public async Task<ActionResult<IEnumerable<BanTinModel>>> GetTop10BanTins()
        {
            var list = await _context.BanTins.OrderByDescending(e => e.NgayCapNhat)
                                          .Take(10)              
                                          .ToListAsync();
            return Utilities.ConvertToDtoList<BanTin, BanTinModel>(list);
        }

        // GET: api/BanTin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BanTinModel>> GetBanTin(int id)
        {
            var banTin = await _context.BanTins.FindAsync(id);

            if (banTin == null)
            {
                return NotFound();
            }

            return Utilities.ConvertToDto<BanTin, BanTinModel>(banTin);
        }

        [HttpPut("{id}")]
        [Authorize([0])]
        public async Task<IActionResult> PutBanTin(int id, BanTinModel request)
        {
            if (id != request.IdbanTin)
            {
                return BadRequest();
            }

            var cd = await _context.BanTins.FirstOrDefaultAsync(c => c.IdbanTin == id);
            if (cd == null )
            {
                return BadRequest();
            }
            try {
                //update bantin
                cd.Nguon = request.Nguon;
                cd.NoiDung = request.NoiDung;
                cd.TieuDe = request.TieuDe;
                cd.NoiDungNgan = request.NoiDungNgan;
                cd.NgayCapNhat = request.NgayCapNhat;

                _context.BanTins.Update(cd);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex) {
                return UnprocessableEntity(new { Error = ex.Message });
            }

            return Ok();
        }

        [HttpPost]
        [Authorize([0])]
        public async Task<IActionResult> Post([FromBody] BanTinRequest request)
        {
            //Validate params
            if (request == null)
            {
                return BadRequest("Invalid");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // create bantin
                var cd = Utilities.ConvertToDto<BanTinRequest, BanTin>(request);

                await _context.BanTins.AddAsync(cd);
                await _context.SaveChangesAsync();
                // Complete the transaction
                await transaction.CommitAsync();

                return StatusCode(201, new { IdBanTin = cd.IdbanTin });
            }
        }

        [Authorize([0])]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanTin(int id)
        {
            var banTin = await _context.BanTins.FindAsync(id);
            if (banTin == null)
            {
                return NotFound();
            }

            _context.BanTins.Remove(banTin);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
