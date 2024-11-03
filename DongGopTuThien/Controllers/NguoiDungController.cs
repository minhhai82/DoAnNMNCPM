using DongGopTuThien.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

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
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.MatKhau);
                    var nguoiDung = new NguoiDung
                    {
                        Email = request.Email,
                        MatKhau = hashedPassword,
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

                    return StatusCode(201, new { nguoiDungId = nguoiDung.IdnguoiDung, Token = token });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }

        // api/NguoiDung/Login
        //{ "Email": "test01@email.com", "Matkhau": "123456" }
        // 200, {Token: 'token'}
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            //validate
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.MatKhau))
            {
                return BadRequest();
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (nguoiDung == null)
            {
                return Unauthorized(new { Error = "Invalid email or password." });
            }

            // Validate password
            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.MatKhau, nguoiDung.MatKhau);
            if (!isValidPassword)
            {
                return Unauthorized(new { Error = "Invalid email or password." });
            }

            // Generate token
            var token = _jwtService.GenerateToken(nguoiDung.IdnguoiDung, nguoiDung.Email);

            return Ok(new { Token = token });
        }

        // api/NguoiDung/verifyOtp
        [HttpPut("verifyOtp")]
        //{DienThoai: "+841232", Code: "123456"}
        //200
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (request == null ||
                   string.IsNullOrEmpty(request.DienThoai) ||
                   string.IsNullOrEmpty(request.Code))
            {
                return BadRequest();
            }


            // Access user claims from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.Sub);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.IdnguoiDung == int.Parse(userIdClaim.Value));

            if (nguoiDung == null || nguoiDung.DienThoai != request.DienThoai)
            {
                return BadRequest();
            }

            try
            {

                var otpCheck = await _otpService.VerifyOtp(request.DienThoai, request.Code);
                if (otpCheck)
                {
                    nguoiDung.TrangThai = TrangThai.XacThucDienThoai;
                    _context.NguoiDungs.Update(nguoiDung);
                    await _context.SaveChangesAsync();

                    return Ok();
                }

                return UnprocessableEntity();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return UnprocessableEntity();
            }
        }

        // api/NguoiDung/{id}/submitPaper
        [HttpPut("{id}/submitPaper")]
        public async Task<IActionResult> SubmitPaper(int id, IFormFile file)
        {
            if (file == null || file.Length == 0 || file.ContentType != "image/jpeg")
                return BadRequest();

            // Access user claims from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.Sub);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.IdnguoiDung == int.Parse(userIdClaim.Value));

            if (nguoiDung == null || nguoiDung.TrangThai != TrangThai.XacThucDienThoai || nguoiDung.Loai != Loai.ToChucTuThien)
            {
                return Unauthorized();
            }

            // Convert the file to byte array
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                nguoiDung.GiayPhep = memoryStream.ToArray();
            }

            // TAM THOI AUTO VERIFIED 
            nguoiDung.TrangThai = TrangThai.XacThucGiayPhep;


            _context.NguoiDungs.Update(nguoiDung);
            await _context.SaveChangesAsync();

            return Ok("Image uploaded successfully.");
        }

        [HttpGet("{id}/downloadPaper")]
        public async Task<IActionResult> DownloadPaper(int id)
        {
            // Access user claims from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.Sub);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var nguoiDungHienTai = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.IdnguoiDung == int.Parse(userIdClaim.Value));

            if (nguoiDungHienTai == null)
            {
                return Unauthorized();
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null || nguoiDung.GiayPhep == null)
                return NotFound("File not found.");

            string contentType = "image/jpeg";
            // Optionally, provide a file name
            string fileName = $"giayto-{id}.jpg";


            // Return the file as a response
            return File(nguoiDung.GiayPhep, contentType, fileName);
        }

    }
}
