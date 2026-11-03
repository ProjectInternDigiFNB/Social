using Application.Interfaces.Recommendation;
using Infrastructure.Recommendation.CandidateGenerators;
using Infrastructure.Recommendation.Collaborative;
using Infrastructure.Recommendation.Semantic;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Recommendation;

/// <summary>Đăng ký các bộ sinh ứng viên (Candidate Generators) và truy vấn đặc thù CSDL của module Recommendation.</summary>
public static class RecommendationModule
{
    public static IServiceCollection AddRecommendationModule(this IServiceCollection services)
    {
        services.AddScoped<ICollaborativeFilteringService, CollaborativeFilteringService>();
        services.AddScoped<ISemanticVectorSearchService, SemanticVectorSearchService>();

        // Candidate Generators
        services.AddScoped<ICandidateGenerator, RecentCandidateGenerator>();
        services.AddScoped<ICandidateGenerator, InterestCandidateGenerator>();
        services.AddScoped<ICandidateGenerator, FollowingCandidateGenerator>();
        services.AddScoped<ICandidateGenerator, TrendingCandidateGenerator>();
        services.AddScoped<ICandidateGenerator, ExplorationCandidateGenerator>();
        services.AddScoped<ICandidateGenerator, CollaborativeCandidateGenerator>();
        services.AddScoped<ICandidateGenerator, SemanticCandidateGenerator>();

        // Orchestration Service
        services.AddScoped<IRecommendationService, RecommendationService>();

        return services;
    }
}
