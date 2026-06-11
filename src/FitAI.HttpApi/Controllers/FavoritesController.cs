using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

using FitAI.Domain.Favorites;
using FitAI.Application.Contracts.Favorites;

namespace FitAI.Controllers
{
    [Authorize]
    [Route("api/favorites")]
    public class FavoritesController : ControllerBase
    {
        private readonly IRepository<FavoriteItem, int> _repo;
        private readonly ICurrentUser _currentUser;

        public FavoritesController(
            IRepository<FavoriteItem, int> repo,
            ICurrentUser currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite(
            [FromBody] FavoriteDto input)
        {
            if (_currentUser.Id == null)
                return Unauthorized();

            var favorite = new FavoriteItem
            {
                UserId = _currentUser.Id.Value,
                ProductName = input.ProductName,
                ProductImage = input.ProductImage,
                Platform = input.Platform,
                Price = input.Price,
                Score = input.Score,
                BodyType = input.BodyType
            };

            await _repo.InsertAsync(favorite);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            if (_currentUser.Id == null)
                return Unauthorized();

            var items = await _repo.GetListAsync(
                x => x.UserId == _currentUser.Id.Value
            );

            return Ok(items.Select(x => new
            {
                x.Id,
                x.ProductName,
                x.ProductImage,
                x.Platform,
                x.Price,
                x.Score,
                x.BodyType
            }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            await _repo.DeleteAsync(id);

            return Ok();
        }
    }
}