using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_API.Features.Items
{
    [Route("api/item")]
    public class ItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _context;

        public ItemController(IMediator mediator, IHttpContextAccessor context)
        {
            _mediator = mediator;
            _context = context;
        }
         [HttpGet]
        public async Task<IActionResult> GetItem(GetItem.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
       
    }
}