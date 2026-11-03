using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Social.Auth;
using Infrastructure.Social.Comments;
using Infrastructure.Social.Follows;
using Infrastructure.Social.Interactions;
using Infrastructure.Social.Interests;
using Infrastructure.Social.Notifications;
using Infrastructure.Social.Posts;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SocialDbContext _context;
    private readonly IServiceProvider? _serviceProvider;
    private IDbContextTransaction? _currentTransaction;
    private int _transactionDepth;

    private IUserRepository? _users;
    private IRefreshTokenRepository? _refreshTokens;
    private IVerificationTokenRepository? _verificationTokens;
    private IInterestRepository? _interests;
    private IUserInterestRepository? _userInterests;
    private IUserPreferenceRepository? _userPreferences;
    private IPostRepository? _posts;
    private IPostInterestRepository? _postInterests;
    private IPostLikeRepository? _postLikes;
    private ICommentRepository? _comments;
    private IUserFollowRepository? _userFollows;
    private IUserInteractionRepository? _userInteractions;
    private INotificationRepository? _notifications;
    private Application.Interfaces.Repositories.Marketplace.Shops.IShopRepository? _shops;
    private Application.Interfaces.Repositories.Marketplace.Shops.IShopReviewRepository? _shopReviews;
    private Application.Interfaces.Repositories.Marketplace.Shops.IShopSanctionRepository? _shopSanctions;
    private Application.Interfaces.Repositories.Marketplace.Categories.ICategoryRepository? _categories;
    private Application.Interfaces.Repositories.Marketplace.Categories.IAttributeRepository? _attributes;
    private Application.Interfaces.Repositories.Marketplace.Categories.ICategoryRequestRepository? _categoryRequests;
    private Application.Interfaces.Repositories.Marketplace.Products.IProductRepository? _products;
    private Application.Interfaces.Repositories.Marketplace.Products.IProductVariantRepository? _productVariants;
    private Application.Interfaces.Repositories.Marketplace.Products.IProductReviewRepository? _productReviews;
    private Application.Interfaces.Repositories.Marketplace.Inventory.IDigitalCodeRepository? _digitalCodes;
    private Application.Interfaces.Repositories.Marketplace.Inventory.IDigitalFileRepository? _digitalFiles;
    private Application.Interfaces.Repositories.Marketplace.Orders.ICartRepository? _carts;
    private Application.Interfaces.Repositories.Marketplace.Orders.ICheckoutRepository? _checkouts;
    private Application.Interfaces.Repositories.Marketplace.Orders.IOrderRepository? _orders;
    private Application.Interfaces.Repositories.Marketplace.Payments.IPaymentRepository? _payments;
    private Application.Interfaces.Repositories.Marketplace.Ledger.ILedgerRepository? _ledgers;
    private Application.Interfaces.Repositories.Marketplace.Complaints.IComplaintRepository? _complaints;
    private Application.Interfaces.Repositories.Marketplace.Complaints.IRefundRepository? _refunds;
    private Application.Interfaces.Repositories.Marketplace.Payouts.IShopBankAccountRepository? _shopBankAccounts;
    private Application.Interfaces.Repositories.Marketplace.Payouts.IPayoutRepository? _payouts;

    public UnitOfWork(SocialDbContext context, IServiceProvider? serviceProvider = null)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    private TRepository Resolve<TRepository>(Func<SocialDbContext, TRepository> fallbackFactory)
        where TRepository : class
    {
        return _serviceProvider?.GetService<TRepository>() ?? fallbackFactory(_context);
    }

    public IUserRepository Users => _users ??= Resolve<IUserRepository>(ctx => new UserRepository(ctx));
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= Resolve<IRefreshTokenRepository>(ctx => new RefreshTokenRepository(ctx));
    public IVerificationTokenRepository VerificationTokens => _verificationTokens ??= Resolve<IVerificationTokenRepository>(ctx => new VerificationTokenRepository(ctx));
    public IInterestRepository Interests => _interests ??= Resolve<IInterestRepository>(ctx => new InterestRepository(ctx));
    public IUserInterestRepository UserInterests => _userInterests ??= Resolve<IUserInterestRepository>(ctx => new UserInterestRepository(ctx));
    public IUserPreferenceRepository UserPreferences => _userPreferences ??= Resolve<IUserPreferenceRepository>(ctx => new UserPreferenceRepository(ctx));
    public IPostRepository Posts => _posts ??= Resolve<IPostRepository>(ctx => new PostRepository(ctx));
    public IPostInterestRepository PostInterests => _postInterests ??= Resolve<IPostInterestRepository>(ctx => new PostInterestRepository(ctx));
    public IPostLikeRepository PostLikes => _postLikes ??= Resolve<IPostLikeRepository>(ctx => new PostLikeRepository(ctx));
    public ICommentRepository Comments => _comments ??= Resolve<ICommentRepository>(ctx => new CommentRepository(ctx));
    public IUserFollowRepository UserFollows => _userFollows ??= Resolve<IUserFollowRepository>(ctx => new UserFollowRepository(ctx));
    public IUserInteractionRepository UserInteractions => _userInteractions ??= Resolve<IUserInteractionRepository>(ctx => new UserInteractionRepository(ctx));
    public INotificationRepository Notifications => _notifications ??= Resolve<INotificationRepository>(ctx => new NotificationRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Shops.IShopRepository Shops => _shops ??= Resolve<Application.Interfaces.Repositories.Marketplace.Shops.IShopRepository>(ctx => new Infrastructure.Marketplace.Shops.ShopRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Shops.IShopReviewRepository ShopReviews => _shopReviews ??= Resolve<Application.Interfaces.Repositories.Marketplace.Shops.IShopReviewRepository>(ctx => new Infrastructure.Marketplace.Shops.ShopReviewRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Shops.IShopSanctionRepository ShopSanctions => _shopSanctions ??= Resolve<Application.Interfaces.Repositories.Marketplace.Shops.IShopSanctionRepository>(ctx => new Infrastructure.Marketplace.Shops.ShopSanctionRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Categories.ICategoryRepository Categories => _categories ??= Resolve<Application.Interfaces.Repositories.Marketplace.Categories.ICategoryRepository>(ctx => new Infrastructure.Marketplace.Categories.CategoryRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Categories.IAttributeRepository Attributes => _attributes ??= Resolve<Application.Interfaces.Repositories.Marketplace.Categories.IAttributeRepository>(ctx => new Infrastructure.Marketplace.Categories.AttributeRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Categories.ICategoryRequestRepository CategoryRequests => _categoryRequests ??= Resolve<Application.Interfaces.Repositories.Marketplace.Categories.ICategoryRequestRepository>(ctx => new Infrastructure.Marketplace.Categories.CategoryRequestRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Products.IProductRepository Products => _products ??= Resolve<Application.Interfaces.Repositories.Marketplace.Products.IProductRepository>(ctx => new Infrastructure.Marketplace.Products.ProductRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Products.IProductVariantRepository ProductVariants => _productVariants ??= Resolve<Application.Interfaces.Repositories.Marketplace.Products.IProductVariantRepository>(ctx => new Infrastructure.Marketplace.Products.ProductVariantRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Products.IProductReviewRepository ProductReviews => _productReviews ??= Resolve<Application.Interfaces.Repositories.Marketplace.Products.IProductReviewRepository>(ctx => new Infrastructure.Marketplace.Products.ProductReviewRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Inventory.IDigitalCodeRepository DigitalCodes => _digitalCodes ??= Resolve<Application.Interfaces.Repositories.Marketplace.Inventory.IDigitalCodeRepository>(ctx => new Infrastructure.Marketplace.Inventory.DigitalCodeRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Inventory.IDigitalFileRepository DigitalFiles => _digitalFiles ??= Resolve<Application.Interfaces.Repositories.Marketplace.Inventory.IDigitalFileRepository>(ctx => new Infrastructure.Marketplace.Inventory.DigitalFileRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Orders.ICartRepository Carts => _carts ??= Resolve<Application.Interfaces.Repositories.Marketplace.Orders.ICartRepository>(ctx => new Infrastructure.Marketplace.Orders.CartRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Orders.ICheckoutRepository Checkouts => _checkouts ??= Resolve<Application.Interfaces.Repositories.Marketplace.Orders.ICheckoutRepository>(ctx => new Infrastructure.Marketplace.Orders.CheckoutRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Orders.IOrderRepository Orders => _orders ??= Resolve<Application.Interfaces.Repositories.Marketplace.Orders.IOrderRepository>(ctx => new Infrastructure.Marketplace.Orders.OrderRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Payments.IPaymentRepository Payments => _payments ??= Resolve<Application.Interfaces.Repositories.Marketplace.Payments.IPaymentRepository>(ctx => new Infrastructure.Marketplace.Payments.PaymentRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Ledger.ILedgerRepository Ledgers => _ledgers ??= Resolve<Application.Interfaces.Repositories.Marketplace.Ledger.ILedgerRepository>(ctx => new Infrastructure.Marketplace.Ledger.LedgerRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Complaints.IComplaintRepository Complaints => _complaints ??= Resolve<Application.Interfaces.Repositories.Marketplace.Complaints.IComplaintRepository>(ctx => new Infrastructure.Marketplace.Complaints.ComplaintRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Complaints.IRefundRepository Refunds => _refunds ??= Resolve<Application.Interfaces.Repositories.Marketplace.Complaints.IRefundRepository>(ctx => new Infrastructure.Marketplace.Complaints.RefundRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Payouts.IShopBankAccountRepository ShopBankAccounts => _shopBankAccounts ??= Resolve<Application.Interfaces.Repositories.Marketplace.Payouts.IShopBankAccountRepository>(ctx => new Infrastructure.Marketplace.Payouts.ShopBankAccountRepository(ctx));
    public Application.Interfaces.Repositories.Marketplace.Payouts.IPayoutRepository Payouts => _payouts ??= Resolve<Application.Interfaces.Repositories.Marketplace.Payouts.IPayoutRepository>(ctx => new Infrastructure.Marketplace.Payouts.PayoutRepository(ctx));

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            _transactionDepth++;
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _transactionDepth = 1;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_currentTransaction != null)
            {
                _transactionDepth = Math.Max(0, _transactionDepth - 1);
                if (_transactionDepth == 0)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            _transactionDepth = 0;
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
