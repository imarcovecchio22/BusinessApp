using BusinessApp.Application.DTOs;

namespace BusinessApp.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(string id);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateAsync(string id, UpdateUserDto updateUserDto);
    Task<bool> DeleteAsync(string id);
}
