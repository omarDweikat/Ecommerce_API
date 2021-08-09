using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_API.Features.Items
{
    [Route("api/images")]
    public class ImageController  : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _context;

        public ImageController (IMediator mediator, IHttpContextAccessor context)
        {
            _mediator = mediator;
            _context = context;
        }
        
        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetItemImages(GetItemImages.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}