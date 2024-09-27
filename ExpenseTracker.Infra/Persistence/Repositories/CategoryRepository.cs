using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infra.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ExpenseTrackerDbContext _context;

    public CategoryRepository(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetAsync(Guid categoryId)
    {
        return await _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(categoryId));
    }
}
