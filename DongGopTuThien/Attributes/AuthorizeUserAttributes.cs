using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using DongGopTuThien.Entities;
using System.Linq;

public class AuthorizeUserAttribute :  Attribute, IAsyncAuthorizationFilter
{

    private DaQltuThienContext _context;
    private int[] _roles;

    public AuthorizeUserAttribute(DaQltuThienContext context, int[] roles)
    {
        _context = context;
        _roles = roles;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Get user ID claim and validate
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.Sub);
        if (userIdClaim == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userId = int.Parse(userIdClaim.Value);
        var nguoiDung = await _context.NguoiDungs.FindAsync(userId);

        // Check roles
        if (nguoiDung == null || (_roles.Length > 0 && !_roles.Contains((int)nguoiDung.Loai)))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["CurrentUser"] = nguoiDung;

    }
}