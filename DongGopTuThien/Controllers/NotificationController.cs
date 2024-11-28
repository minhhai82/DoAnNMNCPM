using DongGopTuThien.Models;
using DongGopTuThien.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DongGopTuThien.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController: ControllerBase
{
    private readonly IHubContext<NotificationService> _hubContext;
    
    public NotificationController(IHubContext<NotificationService> hubContext)
    {
        _hubContext = hubContext;
    }
    
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", request.User, request.Message);
        return Ok(new { Status = "Message sent successfully" });
    }
}