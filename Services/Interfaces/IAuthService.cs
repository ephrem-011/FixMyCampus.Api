using FixMyCampus.Api.DTOs.Auth;

namespace FixMyCampus.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}