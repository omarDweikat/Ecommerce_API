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
        public async Task<IActionResult> GetItems(GetItems.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        //    [HttpGet]
        // public async Task<IActionResult> GetItem(GetItem.Query query)
        // {
        //     var result = await _mediator.Send(query);
        //     return Ok(result);
        //  }
         [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(GetItem.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
          [HttpGet("{id}/images")]
        public async Task<IActionResult> GetItemImages(GetItemImages.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
          [HttpGet("SearchByName/{name}")]
        public async Task<IActionResult> GetItemsByName(GetItemsByName.Query query)
        {   
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("SearchByPrice/{MinPrice}/{MaxPrice}")]
        public async Task<IActionResult> GetItemsByPrice(GetItemsByPrice.Query query)
        {   
            var result = await _mediator.Send(query);
            return Ok(result);
        }
         [HttpGet("SearchByCategory/{Category}")]
        public async Task<IActionResult> GetItemsByCategories(GetItemsByCategories.Query query)
        {   
            var result = await _mediator.Send(query);
            return Ok(result);
        }
         [HttpGet("SearchByDate/{Date}")]
        public async Task<IActionResult> GetItemsByDate(GetItemsByDate.Query query)
        {   
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost("uploadfile")]
        public async Task<IActionResult> UploadImage(UploadImage.Query query)
        {   
            var result = await _mediator.Send(query);
            return Ok(result);
        }
       
    }
}