using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Infrastructure
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}