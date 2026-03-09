using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key no configurada");
        var jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer no configurado");
        var jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience no configurado");
        var jwtExpiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");



        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
        new Claim(ClaimTypes.Email, user.Email),                  
        new Claim(ClaimTypes.Name, user.FullName),                
        new Claim(ClaimTypes.Role, user.Role.Name),               
        new Claim("firstName", user.FirstName),                   
        new Claim("lastName", user.LastName),                     
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken
            (
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiresInMinutes),
                signingCredentials: credentials
            );


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

