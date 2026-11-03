using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Marketplace;

/// <summary>Đăng ký toàn bộ repository, worker và các dịch vụ hạ tầng đặc thù của module Marketplace.</summary>
public static class MarketplaceModule
{
    public static IServiceCollection AddMarketplaceModule(this IServiceCollection services)
    {
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Shops.IShopRepository, Infrastructure.Marketplace.Shops.ShopRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Shops.IShopReviewRepository, Infrastructure.Marketplace.Shops.ShopReviewRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Shops.IShopSanctionRepository, Infrastructure.Marketplace.Shops.ShopSanctionRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Categories.ICategoryRepository, Infrastructure.Marketplace.Categories.CategoryRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Categories.IAttributeRepository, Infrastructure.Marketplace.Categories.AttributeRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Categories.ICategoryRequestRepository, Infrastructure.Marketplace.Categories.CategoryRequestRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Products.IProductRepository, Infrastructure.Marketplace.Products.ProductRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Products.IProductVariantRepository, Infrastructure.Marketplace.Products.ProductVariantRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Products.IProductReviewRepository, Infrastructure.Marketplace.Products.ProductReviewRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Inventory.IDigitalCodeRepository, Infrastructure.Marketplace.Inventory.DigitalCodeRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Inventory.IDigitalFileRepository, Infrastructure.Marketplace.Inventory.DigitalFileRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Orders.ICartRepository, Infrastructure.Marketplace.Orders.CartRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Orders.ICheckoutRepository, Infrastructure.Marketplace.Orders.CheckoutRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Orders.IOrderRepository, Infrastructure.Marketplace.Orders.OrderRepository>();

        services.AddScoped<Application.Interfaces.Marketplace.Shops.IShopService, Infrastructure.Marketplace.Shops.ShopService>();
        services.AddScoped<Application.Interfaces.Marketplace.Shops.IAdminShopService, Infrastructure.Marketplace.Shops.AdminShopService>();
        services.AddScoped<Application.Interfaces.Marketplace.Products.IExcelProductImportService, Infrastructure.Marketplace.Products.ExcelProductImportService>();
        services.AddScoped<Application.Interfaces.Marketplace.Orders.ICheckoutService, Infrastructure.Marketplace.Orders.CheckoutService>();
        services.AddScoped<Application.Interfaces.Marketplace.Orders.IExcelOrderImportService, Infrastructure.Marketplace.Orders.ExcelOrderImportService>();
        services.AddHostedService<Infrastructure.Marketplace.Orders.CheckoutExpirationWorker>();
        services.AddHostedService<Infrastructure.Marketplace.Orders.FundReleaseWorker>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Payments.IPaymentRepository, Infrastructure.Marketplace.Payments.PaymentRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Ledger.ILedgerRepository, Infrastructure.Marketplace.Ledger.LedgerRepository>();
        services.AddScoped<Application.Interfaces.Marketplace.Payments.IPaymentGateway, Infrastructure.Marketplace.Payments.MockPaymentGateway>();
        services.AddHttpClient<Infrastructure.Marketplace.Payments.PayOsPaymentGateway>();
        services.AddScoped<Application.Interfaces.Marketplace.Payments.IPaymentGateway>(sp => sp.GetRequiredService<Infrastructure.Marketplace.Payments.PayOsPaymentGateway>());
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Complaints.IComplaintRepository, Infrastructure.Marketplace.Complaints.ComplaintRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Complaints.IRefundRepository, Infrastructure.Marketplace.Complaints.RefundRepository>();
        services.AddHostedService<Infrastructure.Marketplace.Complaints.ComplaintEscalationWorker>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Payouts.IShopBankAccountRepository, Infrastructure.Marketplace.Payouts.ShopBankAccountRepository>();
        services.AddScoped<Application.Interfaces.Repositories.Marketplace.Payouts.IPayoutRepository, Infrastructure.Marketplace.Payouts.PayoutRepository>();
        services.AddScoped<Application.Interfaces.Marketplace.Audit.IReconciliationService, Infrastructure.Marketplace.Audit.ReconciliationService>();
        services.AddHostedService<Infrastructure.Marketplace.Audit.DailyReconciliationWorker>();
        services.AddScoped<Application.Interfaces.Marketplace.Reports.IAbuseProtectionService, Infrastructure.Marketplace.Reports.AbuseProtectionService>();

        // Marketplace Recommendations (Ingestion Channel & Background Workers)
        services.AddSingleton<Application.Interfaces.Marketplace.Recommendations.IProductEventChannel, Infrastructure.Marketplace.Recommendations.Events.ProductEventChannel>();
        services.AddHostedService<Infrastructure.Marketplace.Recommendations.Events.ProductEventConsumerWorker>();
        services.AddHostedService<Infrastructure.Marketplace.Recommendations.Jobs.ProductCoOccurrenceWorker>();

        // Marketplace Recommendations Engine & Generators
        services.AddScoped<Infrastructure.Marketplace.Recommendations.Generators.SocialAffinityCandidateGenerator>();
        services.AddScoped<Infrastructure.Marketplace.Recommendations.Generators.RecentIntentCandidateGenerator>();
        services.AddScoped<Infrastructure.Marketplace.Recommendations.Generators.ContentSimilarityCandidateGenerator>();
        services.AddScoped<Infrastructure.Marketplace.Recommendations.Generators.TrendingCandidateGenerator>();
        services.AddScoped<Application.Interfaces.Marketplace.Recommendations.IMarketplaceRecommendationService, Infrastructure.Marketplace.Recommendations.MarketplaceRecommendationService>();

        return services;
    }
}
