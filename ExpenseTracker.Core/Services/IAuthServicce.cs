using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Services;

public interface IAuthService
{
    string GenerateJwtToken(User user);
}
