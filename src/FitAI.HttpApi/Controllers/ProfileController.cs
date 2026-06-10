using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Users;
using Volo.Abp.Domain.Repositories;
using FitAI.Domain.Users;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FitAI.Application.Contracts.Account;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
namespace FitAI.Controllers
{ 
[ApiController] 
[Authorize]
[Route("api/profile")]
public class ProfileController : AbpController
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<KullaniciProfil, int> _repo;

    public ProfileController(
        ICurrentUser currentUser,
        IRepository<KullaniciProfil, int> repo)
    {
        _currentUser = currentUser;
        _repo = repo;
    }

    [Authorize]
[HttpPut("body-type")]
public async Task<IActionResult> UpdateBodyType([FromBody] BodyTypeDto input)
{
    if (_currentUser.Id == null)
        return Unauthorized();

    var userId = _currentUser.Id.Value;

    var profile = await _repo.FirstOrDefaultAsync(x => x.UserId == userId);

    if (profile == null)
    {
        profile = new KullaniciProfil
        {
            UserId = userId
        };

        await _repo.InsertAsync(profile);
    }

    profile.VucutTipi = input.BodyType;

    await _repo.UpdateAsync(profile);

    return Ok(new
    {
        message = "Saved",
        bodyType = profile.VucutTipi
    });
}
[HttpGet("my-profile")]
public async Task<IActionResult> GetMyProfile()
{
    try
    {
        if (_currentUser.Id == null)
            return Unauthorized();

        var userId = _currentUser.Id.Value;

        Console.WriteLine("USER ID: " + userId);

        var profile = await _repo.FirstOrDefaultAsync(x => x.UserId == userId);

        Console.WriteLine("PROFILE FOUND");

        var email = CurrentUser.Email;

        return Ok(new
        {
            name = CurrentUser.UserName,
            email = email,
            bodyType = profile?.VucutTipi,
            boy = profile?.Boy,
            kilo = profile?.Kilo
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine("===== PROFILE ERROR =====");
        Console.WriteLine(ex.ToString());

        return StatusCode(500, new
        {
            error = ex.Message,
            stack = ex.StackTrace
        });
    }
}
}
}