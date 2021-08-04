using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_API.Features.Categories
{
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _context;

        public CategoryController(IMediator mediator, IHttpContextAccessor context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Getcategories(Getcategories.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        
    }
}