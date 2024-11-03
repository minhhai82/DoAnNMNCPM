using Microsoft.AspNetCore.Mvc;

namespace DongGopTuThien.Controllers
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(int[]? Roles = null) 
            : base(typeof(AuthorizeUserAttribute))
        {
            Arguments = new object[] { Roles ?? new int[0] };
        }
    }
}