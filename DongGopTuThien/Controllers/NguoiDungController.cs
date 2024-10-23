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
    }
}
