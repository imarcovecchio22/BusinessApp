using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using BusinessApp.Domain.Entities;
using BusinessApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessApp.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _context.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt
            });
        }

        return userDtos;
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        var user = new ApplicationUser
        {
            UserName = createUserDto.Email,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        
        if (!result.Succeeded)
        {
            throw new Exception($"Error al crear usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // Asignar roles
        if (createUserDto.Roles.Any())
        {
            await _userManager.AddToRolesAsync(user, createUserDto.Roles);
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto> UpdateAsync(string id, UpdateUserDto updateUserDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new Exception("Usuario no encontrado");
        }

        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.IsActive = updateUserDto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new Exception($"Error al actualizar usuario: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
        }

        // Actualizar roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        
        if (updateUserDto.Roles.Any())
        {
            await _userManager.AddToRolesAsync(user, updateUserDto.Roles);
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
