using ExpenseTracker.Core.DTOs;
using ExpenseTracker.Core.DTOs.Users;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ExpenseTracker.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public AuthController(IUserService userService, IUserRepository userRepository, IConfiguration configuration, HttpClient httpClient, IAuthService authService)
    {
        _userService = userService;
        _userRepository = userRepository;
        _configuration = configuration;
        _httpClient = httpClient;
        _authService = authService;
    }

    [HttpGet("sso/google")]
    public IActionResult RegisterAsync()
    {
        var redirectUri = new Uri($"{Request.Scheme}://{Request.Host}/api/Auth/google-callback");

        var redirectEndpointBuilder = new StringBuilder()
        .Append("https://accounts.google.com/o/oauth2/v2/auth?")
        .Append($"client_id={Configuration.GoogleClientId}")
        .Append($"&redirect_uri={redirectUri.AbsoluteUri}")
        .Append($"&response_type=code")
        .Append($"&scope=https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile");

        var redirectEndpoint = redirectEndpointBuilder.ToString();

        return Redirect(redirectEndpoint);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> OAuthGoogleCallback([FromQuery] string code)
    {
        var redirectUri = new Uri($"{Request.Scheme}://{Request.Host}/api/Auth/google-callback");
        var accessToken = await _authService.ExchangeCodeForAccessToken(code, redirectUri.AbsoluteUri);

        var profileContent = await _authService.GetGoogleProfileData(accessToken);

        var user = await _userRepository.GetByEmail(profileContent.Email);

        //Create user if not exists
        if (user == null)
        {
            var createUserDto = new CreateUserDto 
            {
                Name = profileContent.Name,
                Email = profileContent.Email 
            };
            user = await _userService.CreateUserAsync(createUserDto);
        }

        var jwtToken = _authService.GenerateJwtToken(user);

        return Ok(new ResultDto(new { Token = jwtToken }, "Usuário autenticado com sucesso!"));
    }
}

