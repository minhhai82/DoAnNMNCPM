using System;
using DongGopTuThien.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DongGopTuThien.Models;
using Microsoft.AspNetCore.Http;


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
        // {
        //     "Email": "test01@email.com",
        //     "DienThoai": "+8491xxxxxx",
        //     "DiaChi": "HCM",
        //     "TenDayDu": "Test01",
        //     "Matkhau": "123456",
        //     "Loai": 1,
        //     "TenDangNhap": "test01@email.com"
        // }
        // 201, { nguoiDungId: 1, Token: 'token' }
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
                        TrangThai = (int)TrangThai.ChuaXacThuc,
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

            return Ok(new { NguoiDungId = nguoiDung.IdnguoiDung, Loai = nguoiDung.Loai, Token = token });
        }

        // api/NguoiDung/verifyOtp
        [HttpPut("verifyOtp")]
        [Authorize]
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

           var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            Console.WriteLine(nguoiDung.DienThoai);

            if (nguoiDung == null || nguoiDung.DienThoai != request.DienThoai)
            {
                return BadRequest();
            }

            try
            {

                var otpCheck = await _otpService.VerifyOtp(request.DienThoai, request.Code);
                if (otpCheck)
                {
                    nguoiDung.TrangThai = (int)TrangThai.XacThucDienThoai;
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

        // api/NguoiDung/submitPaper
        [HttpPut("submitPaper")]
        [Authorize([3])]
        public async Task<IActionResult> SubmitPaper(IFormFile file)
        {
            if (file == null || file.Length == 0 ||
                (file.ContentType != "image/jpeg" && file.ContentType != "image/png" && file.ContentType != "image/jpg"))
                return BadRequest();

            var nguoiDung = HttpContext.Items["CurrentUser"] as NguoiDung;

            // Convert the file to byte array
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                nguoiDung.GiayPhep = memoryStream.ToArray();
            }

            // TAM THOI AUTO VERIFIED 
            nguoiDung.TrangThai = (int)TrangThai.XacThucGiayPhep;


            _context.NguoiDungs.Update(nguoiDung);
            await _context.SaveChangesAsync();

            return Ok("Image uploaded successfully.");
        }


        [HttpGet("{id}/downloadPaper")]
        [Authorize]
        public async Task<IActionResult> DownloadPaper(int id)
        {
            var nguoiDungHienTai = HttpContext.Items["CurrentUser"] as NguoiDung;

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);

            if (nguoiDung == null || nguoiDung.GiayPhep == null || nguoiDung.IdnguoiDung != id)
                return NotFound("File not found.");

            // Lấy loại MIME từ byte[]
            string contentType = GetImageMimeType(nguoiDung.GiayPhep);
            string fileExtension = contentType.Split('/')[1];

            // Optionally, provide a file name
            string fileName = $"giayto-{id}.{fileExtension}";


            // Return the file as a response
            return File(nguoiDung.GiayPhep, contentType, fileName);
        }

        // Phương thức để xác định loại MIME
        private string GetImageMimeType(byte[] imageData)
        {
            // Tạo MemoryStream từ byte array
            using (var stream = new MemoryStream(imageData))
            {
                // Sử dụng Image.DetectFormat để lấy định dạng
                IImageFormat format = Image.DetectFormat(stream);
                if (format == null)
                {
                    throw new InvalidOperationException("Không thể xác định định dạng ảnh.");
                }

                return format.DefaultMimeType;
            }
        }
    }
}
