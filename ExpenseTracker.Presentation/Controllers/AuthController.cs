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
        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", Configuration.GoogleClientId),
            new KeyValuePair<string, string>("client_secret", Configuration.GoogleClientSecret),
            new KeyValuePair<string, string>("redirect_uri", redirectUri.AbsoluteUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),

        });

        //Exchange code for access token
        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", requestData);
        if (!response.IsSuccessStatusCode)
            return BadRequest(new ResultDto("Ocorreu um erro ao fazer a requisição para os serviços google"));
        
        dynamic content = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

        if (content == null)
            return BadRequest(new ResultDto("Não foi possível concluir a autenticação"));

        //Get user profile data with access token
        string accessToken = content.access_token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var profileResponse = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        if (!profileResponse.IsSuccessStatusCode)
            return BadRequest(new ResultDto("Não foi possível obter os dados do usuário"));

        var profileContent = JsonConvert.DeserializeObject<GoogleProfileResponse>(await profileResponse.Content.ReadAsStringAsync()) ?? throw new Exception("Não foi possível obter os dados do usuário");
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

public record GoogleProfileResponse(string Name, string Email);
