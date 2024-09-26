using ExpenseTracker.Core.DTOs.Users;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Services;

namespace ExpenseTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = createUserDto.Email,
            Name = createUserDto.Name
        };

        await _userRepository.CreateAsync(user);
        return user;
    }
}
