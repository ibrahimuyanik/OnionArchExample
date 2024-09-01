using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Onion.Application.Bases;
using Onion.Application.Features.Auth.Rules;
using Onion.Application.Interfaces.AutoMapper;
using Onion.Application.Interfaces.UnitOfWorks;
using Onion.Domain.Entitites;

namespace Onion.Application.Features.Auth.Command.Register
{
    public class RegisterCommandHandler : BaseHandler, IRequestHandler<RegisterCommandRequest, Unit>
    {
        private readonly AuthRules _authRules;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public RegisterCommandHandler(AuthRules authRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, RoleManager<Role> roleManager) : base(mapper, unitOfWork, httpContextAccessor)
        {
            _authRules = authRules;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Unit> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            await _authRules.UserShouldNotBeExist(await _userManager.FindByEmailAsync(request.Email));
            // yapılan istekteki email adresine sahip kullanıcı varsa hata dönecek çünkü aynı email'e sahip kullanıcı olmamalıdır

            User user = _mapper.Map<User, RegisterCommandRequest>(request);
            user.UserName = request.Email;
            // identity'de username alanı zorunludur girilmezse hata verir
            user.SecurityStamp = Guid.NewGuid().ToString(); 

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);
            if(result.Succeeded)
            {
                if(!await _roleManager.RoleExistsAsync("user"))
                {
                    await _roleManager.CreateAsync(new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "user",
                        NormalizedName = "USER",
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    });
                }

                await _userManager.AddToRoleAsync(user, "user");
            }

            return Unit.Value;

        }
    }
}
