using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onion.Application.Features.Products.Command.CreateProduct;
using Onion.Application.Features.Products.Command.DeleteProduct;
using Onion.Application.Features.Products.Command.UpdateProduct;
using Onion.Application.Features.Products.Queries.GetAllProducts;
using Onion.Application.Interfaces.UnitOfWorks;
using Onion.Domain.Entitites;

namespace OnionAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public ValuesController(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpGet]
        
        public async Task<IActionResult> Get()
        {
            await _unitOfWork.GetReadRepository<Product>().GetAllAsync();
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _mediator.Send(new GetAllProductsQueryRequest());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommandRequest request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductCommandRequest request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(DeleteProductCommandRequest request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
