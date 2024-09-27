using ExpenseTracker.Core.DTOs.Users;
using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Services;

public interface IAuthService
{
    string GenerateJwtToken(User user);
    Task<string> ExchangeCodeForAccessToken(string code, string redirectUri);
    Task<GoogleProfileDto> GetGoogleProfileData(string accessToken);
}
