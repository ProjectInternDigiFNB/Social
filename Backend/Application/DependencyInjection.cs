using System.Reflection;
using Application.Common.Mappings;
using Application.Interfaces;
using Application.Interfaces.Admin;
using Application.Interfaces.Marketplace.Categories;
using Application.Interfaces.Marketplace.Complaints;
using Application.Interfaces.Marketplace.Inventory;
using Application.Interfaces.Marketplace.Ledger;
using Application.Interfaces.Marketplace.Orders;
using Application.Interfaces.Marketplace.Payments;
using Application.Interfaces.Marketplace.Payouts;
using Application.Interfaces.Marketplace.Products;
using Application.Interfaces.Recommendation;
using Application.Services.Admin;
using FluentValidation;
using Infrastructure.Marketplace.Categories;
using Infrastructure.Marketplace.Complaints;
using Infrastructure.Marketplace.Inventory;
using Infrastructure.Marketplace.Ledger;
using Infrastructure.Marketplace.Orders;
using Infrastructure.Marketplace.Payments;
using Infrastructure.Marketplace.Payouts;
using Infrastructure.Marketplace.Products;
using Infrastructure.Recommendation.Aggregator;
using Infrastructure.Recommendation.Diversity;
using Infrastructure.Recommendation.Ranking;
using Infrastructure.Social.Auth;
using Infrastructure.Social.Comments;
using Infrastructure.Social.Follows;
using Infrastructure.Social.Interactions;
using Infrastructure.Social.Interests;
using Infrastructure.Social.Notifications;
using Infrastructure.Social.Posts;
using Infrastructure.Social.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 1. AutoMapper & FluentValidation
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // 2. Admin Use-Case Services
        services.AddScoped<IAdminUserService, AdminUserService>();

        // 3. Social Use-Case Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IInterestService, InterestService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IFeedService, FeedService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IFollowService, FollowService>();
        services.AddScoped<IInteractionService, InteractionService>();
        services.AddScoped<INotificationService, NotificationService>();

        // 4. Recommendation Pipeline Use-Case Services
        services.AddScoped<ICandidateAggregator, CandidateAggregator>();
        services.AddScoped<IRecommendationRankingService, RecommendationRankingService>();
        services.AddScoped<IDiversityService, DiversityService>();

        // 5. Marketplace Use-Case Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAdminCategoryService, AdminCategoryService>();
        services.AddScoped<ISellerCategoryService, SellerCategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IAdminProductService, AdminProductService>();
        services.AddScoped<IProductReviewService, ProductReviewService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ILedgerService, LedgerService>();
        services.AddScoped<IComplaintService, ComplaintService>();
        services.AddScoped<IPayoutService, PayoutService>();

        return services;
    }
}
