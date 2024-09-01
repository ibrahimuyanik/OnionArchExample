using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Onion.Application.Bases;
using Onion.Application.DTOs;
using Onion.Application.Interfaces.AutoMapper;
using Onion.Application.Interfaces.UnitOfWorks;
using Onion.Domain.Entitites;

namespace Onion.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : BaseHandler, IRequestHandler<GetAllProductsQueryRequest, IList<GetAllProductsQueryResponse>>
    {
        public GetAllProductsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, httpContextAccessor)
        {
           
        }

        public async Task<IList<GetAllProductsQueryResponse>> Handle(GetAllProductsQueryRequest request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.GetReadRepository<Product>().GetAllAsync(include: x => x.Include(b => b.Brand));

            var brand = _mapper.Map<BrandDto, Brand>(new Brand());

            var map = _mapper.Map<GetAllProductsQueryResponse, Product>(products);
            foreach (var item in map)
                item.Price -= (item.Price * item.Discount / 100);

            return map;
            
        }
    }
}
