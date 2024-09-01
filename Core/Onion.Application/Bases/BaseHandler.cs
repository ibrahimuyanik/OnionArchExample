using Microsoft.AspNetCore.Http;
using Onion.Application.Interfaces.AutoMapper;
using Onion.Application.Interfaces.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Bases
{
    public class BaseHandler
    {
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly string userId;

        public BaseHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
