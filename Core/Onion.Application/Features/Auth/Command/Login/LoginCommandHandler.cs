using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Onion.Application.Bases;
using Onion.Application.Features.Auth.Rules;
using Onion.Application.Interfaces.AutoMapper;
using Onion.Application.Interfaces.Tokens;
using Onion.Application.Interfaces.UnitOfWorks;
using Onion.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Features.Auth.Command.Login
{
    public class LoginCommandHandler : BaseHandler, IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly AuthRules _authRules;
        public LoginCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, AuthRules authRules, RoleManager<Role> roleManager, ITokenService tokenService, IConfiguration configuration) : base(mapper, unitOfWork, httpContextAccessor)
        {
            _userManager = userManager;
            _authRules = authRules;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            User user = await _userManager.FindByEmailAsync(request.Email);

            bool checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            await _authRules.EmailOrPasswordShouldNotBeInvalid(user, checkPassword);

            var roles = await _userManager.GetRolesAsync(user);

            JwtSecurityToken token = await _tokenService.CreateToken(user, roles);
            string refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);

            var _token = new JwtSecurityTokenHandler().WriteToken(token);

            await _userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", _token);
            // burada AspNetUserTokens tablosuna kullanıcıların aldığı token'ları kaydettik.

            return new()
            {
                Token = _token,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };
        }
    }
}
