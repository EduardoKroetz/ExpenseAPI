using ExpenseTracker.Core.DTOs;
using ExpenseTracker.Core.DTOs.Users;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ExpenseTracker.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AuthService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(8), // Expiração do token
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token); // Retorna o JWT como string
    }

    public async Task<string> ExchangeCodeForAccessToken(string code, string redirectUri)
    {
        var googleClientId = _configuration["Google:ClientId"] ?? throw new Exception("Não foi possível concluir a autenticação");
        var googleClientSecret = _configuration["Google:ClientSecret"] ?? throw new Exception("Não foi possível concluir a autenticação");

        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", googleClientId),
            new KeyValuePair<string, string>("client_secret", googleClientSecret),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),

        });

        //Exchange code for access token
        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", requestData);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Ocorreu um erro ao fazer a requisição para os serviços google");

        dynamic content = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) ?? throw new Exception("Não foi possível concluir a autenticação");

        return content.access_token;
    }

    public async Task<GoogleProfileDto> GetGoogleProfileData(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var profileResponse = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        if (!profileResponse.IsSuccessStatusCode)
            throw new Exception("Não foi possível obter os dados do usuário");

        var profileContent = JsonConvert.DeserializeObject<GoogleProfileDto>(await profileResponse.Content.ReadAsStringAsync()) ?? throw new Exception("Não foi possível obter os dados do usuário");
        return profileContent;
    }


}

