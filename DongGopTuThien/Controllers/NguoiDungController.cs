using DongGopTuThien.Entities;
using Microsoft.AspNetCore.Mvc;


namespace DongGopTuThien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private DaQltuThienContext _context;
        private readonly IJwtService _jwtService;

        private readonly IOTPService _otpService;

        public NguoiDungController(DaQltuThienContext ctx, IJwtService jwtService, IOTPService optService)
        {
            _context = ctx;
            _jwtService = jwtService;
            _otpService = optService;
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
                string.IsNullOrEmpty(request.MatKhau)
                )
            {
                return BadRequest("Invalid");
            }

            try
            {
               using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // Create NguoiDung
                    var nguoiDung = new NguoiDung
                    {
                        Email = request.Email,
                        MatKhau = request.MatKhau,
                        DienThoai = request.DienThoai,
                        TenDayDu = request.TenDayDu,
                        DiaChi = request.DiaChi,
                        TrangThai = TrangThai.ChuaXacThuc,
                        Loai = request.Loai,
                        TenDangNhap = request.Email,
                    };

                    _context.NguoiDungs.Add(nguoiDung);
                    await _context.SaveChangesAsync();

                    // send SMS
                    _otpService.SendOtp(request.DienThoai);

                    var token = _jwtService.GenerateToken(nguoiDung.IdnguoiDung, nguoiDung.Email);

                    // Complete the transaction
                    await transaction.CommitAsync();

                    return Ok(new { nguoiDungId = nguoiDung.IdnguoiDung, Token = token });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }


    }
}
