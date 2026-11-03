namespace Infrastructure.External.Caching;

public static class CacheKeys
{
    public const string InterestsAllActive = Application.Common.CacheKeys.InterestsAllActive;
    public static string UserProfile(string username) => Application.Common.CacheKeys.UserProfile(username);
    public const string CategoriesTree = Application.Common.CacheKeys.CategoriesTree;
    public static string CategoryAttributes(Guid categoryId) => Application.Common.CacheKeys.CategoryAttributes(categoryId);
    public static string ShopPublic(string slug) => Application.Common.CacheKeys.ShopPublic(slug);
    public static string ProductDetail(string slug) => Application.Common.CacheKeys.ProductDetail(slug);
    public static string ShopBalance(Guid shopId) => Application.Common.CacheKeys.ShopBalance(shopId);
}
