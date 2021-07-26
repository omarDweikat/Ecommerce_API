using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_API.Features.Authentication
{
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _context;
        public AuthenticationController(IMediator mediator,IHttpContextAccessor context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login.Command command)
        {
            var user = await _mediator.Send(command);
            if (user == null)
                return BadRequest(new { Error = "اسم المستخدم او كلمة المرور غير صحيحة" });

            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Logout.Command command)
        {
            command.Token = _context.HttpContext.Request.Headers["Authorization"];
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}