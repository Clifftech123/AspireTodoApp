using AspireTodoApp.Server.Common;
using AspireTodoApp.Server.Entities;
using AspireTodoApp.Server.Features.Auth.Dtos;
using AspireTodoApp.Server.Features.Validation;
using Microsoft.AspNetCore.Identity;

namespace AspireTodoApp.Server.Features.Auth;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", Register)
             .WithParameterValidation(typeof(RegisterRequest));

        group.MapPost("/login", Login)
             .WithParameterValidation(typeof(LoginRequest));

        group.MapPost("/refresh", Refresh)
             .WithParameterValidation(typeof(RefreshRequest));

        group.MapPost("/logout", Logout)
             .RequireAuthorization("CurrentUser")
             .WithParameterValidation(typeof(RefreshRequest));

        group.MapPost("/logout-all", LogoutAll)
             .RequireAuthorization("CurrentUser");

        group.MapGet("/me", Me)
             .RequireAuthorization("CurrentUser");

        return group;
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        UserManager<AppUser> userManager)
    {
        var user = new AppUser { Email = request.Email, UserName = request.Email };
        user.SetDisplayName(request.DisplayName);

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ToDictionary(
                e => e.Code, e => new[] { e.Description }));

        return Results.Ok(ApiResponse.Ok("Registration successful."));
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        UserManager<AppUser> userManager,
        IJwtTokenService jwtService,
        IRefreshTokenService refreshService)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Results.Problem("Invalid email or password.", statusCode: 401);

        var (accessToken, expiresAt) = jwtService.GenerateAccessToken(user);
        var rawRefresh = jwtService.GenerateRefreshToken();
        await refreshService.CreateAsync(user.Id, rawRefresh);

        return Results.Ok(ApiResponse.Ok(new TokenResponse(accessToken, rawRefresh, expiresAt)));
    }

    private static async Task<IResult> Refresh(
        RefreshRequest request,
        UserManager<AppUser> userManager,
        IJwtTokenService jwtService,
        IRefreshTokenService refreshService)
    {
        var existing = await refreshService.GetActiveTokenAsync(request.RefreshToken);
        if (existing is null)
            return Results.Problem("Invalid or expired refresh token.", statusCode: 401);

        var user = await userManager.FindByIdAsync(existing.UserId);
        if (user is null)
            return Results.Problem("User not found.", statusCode: 401);

        var rawNew = jwtService.GenerateRefreshToken();
        await refreshService.RotateAsync(request.RefreshToken, user.Id, rawNew);

        var (accessToken, expiresAt) = jwtService.GenerateAccessToken(user);

        return Results.Ok(ApiResponse.Ok(new TokenResponse(accessToken, rawNew, expiresAt)));
    }

    private static async Task<IResult> Logout(
        RefreshRequest request,
        IRefreshTokenService refreshService)
    {
        await refreshService.RevokeAsync(request.RefreshToken);
        return Results.NoContent();
    }

    private static async Task<IResult> LogoutAll(
        CurrentUser currentUser,
        IRefreshTokenService refreshService)
    {
        await refreshService.RevokeAllForUserAsync(currentUser.Id);
        return Results.NoContent();
    }

    private static IResult Me(CurrentUser currentUser) =>
        Results.Ok(ApiResponse.Ok(new
        {
            currentUser.Id,
            currentUser.User?.Email,
            currentUser.User?.DisplayName,
            currentUser.User?.FirstName,
            currentUser.User?.LastName,
        }));
}
