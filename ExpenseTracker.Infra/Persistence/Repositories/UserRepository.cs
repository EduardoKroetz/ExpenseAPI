
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infra.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ExpenseTrackerDbContext _context;

    public UserRepository(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> Get(Guid userId)
    {
       return await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Email.Equals(email));
    }
}
