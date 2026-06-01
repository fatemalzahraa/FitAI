using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Identity;
using FitAI.Auth;

namespace FitAI.Application.Auth
{
    public class AuthAppService : IdentityAppServiceBase, IAuthAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthAppService(
            IdentityUserManager userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task RegisterAsync(UserRegisterDto input)
        {
            var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.Email);
            var result = await _userManager.CreateAsync(user, input.Password);
            result.CheckErrors();
        }

public async Task<string> LoginAsync(UserLoginDto input)
{
    var user = await _userManager.FindByNameAsync(input.UserName);
    if (user == null)
        throw new AbpAuthorizationException("Kullanıcı bulunamadı.");

    var result = await _signInManager.PasswordSignInAsync(
        user, input.Password, isPersistent: false, lockoutOnFailure: false);

    if (!result.Succeeded)
        throw new AbpAuthorizationException("Kullanıcı adı veya şifre hatalı.");

    return user.UserName!;  // Interface'in beklediği string
}
    }
}