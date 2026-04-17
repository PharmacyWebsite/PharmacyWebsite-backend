
using Microsoft.EntityFrameworkCore;
using PharmacyOrderingSystem.Data;
using PharmacyOrderingSystem.DTOs;
using PharmacyOrderingSystem.Helpers;
using PharmacyOrderingSystem.Models;

namespace PharmacyOrderingSystem.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly JwtHelper _jwt;
    private readonly PasswordHasher _hasher;

    public AuthService(AppDbContext context, JwtHelper jwt, PasswordHasher hasher)
    {
        _context = context;
        _jwt = jwt;
        _hasher = hasher;
    }

    public async Task<AuthResponseDto> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("User already exists");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _hasher.Hash(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = _jwt.GenerateToken(user),
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto> Login(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        return new AuthResponseDto
        {
            Token = _jwt.GenerateToken(user),
            Email = user.Email,
            Role = user.Role
        };
    }
}