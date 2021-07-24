using System.Threading.Tasks;
using api.Utilities;
using api.Features;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.User
{
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _context;

        public UserController(IMediator mediator, IHttpContextAccessor context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(GetUsers.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("nextID")]
        public async Task<IActionResult> GetNextID(GetNextUserId.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(GetUsers.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUser.Command command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUser.Command command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}