using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Onion.Application.Bases;
using Onion.Application.Features.Auth.Rules;
using Onion.Application.Interfaces.AutoMapper;
using Onion.Application.Interfaces.Tokens;
using Onion.Application.Interfaces.UnitOfWorks;
using Onion.Domain.Entitites;
using System.Security.Claims;

namespace Onion.Application.Features.Auth.Command.RefreshToken
{
    public class RefreshTokenCommandHandler : BaseHandler, IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AuthRules _authRules;
        public RefreshTokenCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, ITokenService tokenService, AuthRules authRules) : base(mapper, unitOfWork, httpContextAccessor)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _authRules = authRules;
        }

        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            string email = principal.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);

            //_authRules.RefreshTokenShouldNotBeExpired

            return new()
            {

            };


        }
    }
}
