using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitAI.Domain.Favorites
{
    public class FavoriteItem : AuditedAggregateRoot<int>
    {
        public Guid UserId { get; set; }

        public string ProductName { get; set; } = null!;

        public string ProductImage { get; set; } = null!;

        public string Platform { get; set; } = null!;

        public string Price { get; set; } = null!;
        public int Score { get; set; }

        public string BodyType { get; set; } = null!;
    }
}