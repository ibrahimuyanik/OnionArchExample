using MediatR;
using Microsoft.AspNetCore.Http;
using Onion.Application.Bases;
using Onion.Application.Features.Products.Rules;
using Onion.Application.Interfaces.AutoMapper;
using Onion.Application.Interfaces.UnitOfWorks;
using Onion.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Features.Products.Command.CreateProduct
{
    public class CreateProductCommandHandler : BaseHandler, IRequestHandler<CreateProductCommandRequest, Unit>
    {
        private readonly ProductRules _productRules;

        public CreateProductCommandHandler(ProductRules productRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
            _productRules = productRules;
        }

        public async Task<Unit> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
        {
            IList<Product> products = await _unitOfWork.GetReadRepository<Product>().GetAllAsync();


            await _productRules.ProductTitleMustNotBeSame(products, request.Title);

            Product product = new(request.Title, request.Description, request.Price, request.BrandId, request.Discount);
            
            await _unitOfWork.GetWriteRepository<Product>().AddAsync(product);
            if(await _unitOfWork.SaveAsync() > 0)
            {
                foreach (var categoryId in request.CategoryIds)
                    await _unitOfWork.GetWriteRepository<ProductCategory>().AddAsync(new()
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId
                    });
                await _unitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
