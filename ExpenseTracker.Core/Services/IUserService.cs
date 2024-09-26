using ExpenseTracker.Core.DTOs.Users;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserDto createUserDto);
}
