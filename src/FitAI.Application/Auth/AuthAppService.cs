using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Identity;
using FitAI.Auth;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Repositories;
using FitAI.Domain.Users;
namespace FitAI.Application.Auth
{
  [AllowAnonymous]
    public class AuthAppService : IdentityAppServiceBase, IAuthAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IRepository<KullaniciProfil, int> _profileRepo;
      public AuthAppService(
    IdentityUserManager userManager,
    SignInManager<IdentityUser> signInManager,
    IRepository<KullaniciProfil, int> profileRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profileRepo = profileRepo;
        }

        public async Task RegisterAsync(UserRegisterDto input)
{
    var user = new IdentityUser(
        GuidGenerator.Create(),
        input.UserName,
        input.Email
    );

    var result = await _userManager.CreateAsync(
        user,
        input.Password
    );

    result.CheckErrors();

    // PROFIL OLUŞTUR
    await _profileRepo.InsertAsync(
        new KullaniciProfil
        {
            UserId = user.Id,
            CihazId = Guid.NewGuid().ToString(),
            VucutTipi = null
        }
    );
}

        public async Task<string> LoginAsync(UserLoginDto input)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);

            if (user == null)
                throw new AbpAuthorizationException("User not found");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                input.Password,
                false
            );

            if (!result.Succeeded)
                throw new AbpAuthorizationException("Wrong password");

          var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.UserName ?? ""),
    new Claim(ClaimTypes.Email, user.Email ?? ""),

    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
};

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("FITAI_SUPER_SECRET_KEY_123456_ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: "FitAI",
                audience: "FitAI",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}