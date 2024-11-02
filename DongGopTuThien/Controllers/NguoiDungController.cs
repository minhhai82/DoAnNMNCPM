using DongGopTuThien.Entities;
using Microsoft.AspNetCore.Mvc;


namespace DongGopTuThien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private DaQltuThienContext _context;

        public NguoiDungController(DaQltuThienContext ctx)
        {
            _context = ctx;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.NguoiDungs.ToList());
        }


        // Endpoint to register a user
        // POST /api/NguoiDung/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] NguoiDung request)
        {

            //Validate params
            if (
                request == null ||
                string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.DienThoai) ||
                string.IsNullOrEmpty(request.MatKhau) ||
                )
            {
                return BadRequest("Invalid");
            }

            try
            {
                // Create NguoiDung
                var nguoiDung = new NguoiDung
                {
                    Email = request.Email,
                    MatKhau = request.MatKhau,
                    DienThoai = request.DienThoai,
                    TenDayDu = request.TenDayDu,
                    DiaChi = request.DiaChi,
                    TrangThai = TrangThai,
                    Loai = request.Loai,
                };

                _context.NguoiDungs.Add(nguoiDung);
                await _context.SaveChangesAsync();
                var nguoiDungId = nguoiDung.IdnguoiDung;

                return Ok(new { NguoiDungId = nguoiDungId, Token = customToken });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                return BadRequest(new { Error = ex.Message });
            }
        }


    }
}
