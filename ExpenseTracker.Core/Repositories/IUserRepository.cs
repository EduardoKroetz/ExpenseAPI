
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Repositories;

public interface IUserRepository
{
    Task<User?> Get(Guid userId);
    Task<User?> GetByEmail(string email);
    Task CreateAsync(User user);
}
