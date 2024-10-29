using DongGopTuThien.Entities;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;


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
                string.IsNullOrEmpty(request.FirebaseUid)
                )
            {
                return BadRequest("Invalid");
            }

            try
            {
                //Get a firebase user from FirebaseUid
                var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserAsync(request.FirebaseUid);
                var firebaseUpdate = new UserRecordArgs
                {
                    Uid = request.FirebaseUid,  // Firebase UID of the user
                    Email = request.Email,
                    Password = request.MatKhau
                };
                await FirebaseAuth.DefaultInstance.UpdateUserAsync(firebaseUpdate);

                // Create NguoiDung
                // If To Chuc -> trang thai 0 unverified, Otherwise trang thai 1 verified
                var TrangThai = request.Loai == 2 ? 0 : 1;
                var nguoiDung = new NguoiDung
                {
                    Email = request.Email,
                    MatKhau = request.MatKhau,
                    DienThoai = request.DienThoai,
                    TenDayDu = request.TenDayDu,
                    DiaChi = request.DiaChi,
                    TrangThai = TrangThai,
                    Loai = request.Loai,
                    FirebaseUid = request.FirebaseUid,
                };

                _context.NguoiDungs.Add(nguoiDung);
                await _context.SaveChangesAsync();
                var nguoiDungId = nguoiDung.IdnguoiDung;

                //Generate a custom token for the newly created user
                var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(firebaseUser.Uid);


                return Ok(new { NguoiDungId = nguoiDungId, Token = customToken });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                return BadRequest(new { Error = ex.Message });
            }
        }

        // [NONE] Endpoint to login
        // handle by firebase on client side. After successfully signin with Firebase on client
        // use Token send with BE request. E.g
        // fetch(apiUrl, {method: 'GET', headers: {'Authorization': `Bearer ${ token}`, 'Content-Type': 'application/json' }});



        // Endpoint to submit giay phep


    }
}
