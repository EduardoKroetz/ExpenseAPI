using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetAsync(Guid categoryId);
    Task<IEnumerable<Category>> GetAsync();
}
