using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Social.Auth;
using Infrastructure.Social.Comments;
using Infrastructure.Social.Follows;
using Infrastructure.Social.Interactions;
using Infrastructure.Social.Interests;
using Infrastructure.Social.Notifications;
using Infrastructure.Social.Posts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Social;

/// <summary>Đăng ký repository và hạ tầng realtime của module Social.</summary>
public static class SocialModule
{
    public static IServiceCollection AddSocialModule(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();
        services.AddScoped<IInterestRepository, InterestRepository>();
        services.AddScoped<IUserInterestRepository, UserInterestRepository>();
        services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostInterestRepository, PostInterestRepository>();
        services.AddScoped<IPostLikeRepository, PostLikeRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserFollowRepository, UserFollowRepository>();
        services.AddScoped<IUserInteractionRepository, UserInteractionRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();

        return services;
    }
}
