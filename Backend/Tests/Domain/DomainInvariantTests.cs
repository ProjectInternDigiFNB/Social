using Domain.Entities.Marketplace.Ledger;
using Domain.Entities.Marketplace.Orders;
using Domain.Entities.Marketplace.Shops;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Tests.Domain;

public class DomainInvariantTests
{
    [Fact]
    public void ShopSanction_Warning_ShouldNotBeConsideredSuspensionActive()
    {
        var now = new DateTime(2026, 11, 3, 10, 0, 0, DateTimeKind.Utc);
        var sanction = new ShopSanction(
            shopId: Guid.NewGuid(),
            type: ShopSanctionType.Warning,
            reason: "Nhắc nhở cập nhật mô tả sản phẩm",
            createdByAdminId: Guid.NewGuid(),
            startAtUtc: now.AddMinutes(-10),
            endAtUtc: null);

        sanction.IsActive(now).Should().BeTrue();
        sanction.IsSuspensionActive(now).Should().BeFalse();
    }

    [Fact]
    public void ShopSanction_TemporarySuspension_ShouldRespectEndAtUtcAndLift()
    {
        var now = new DateTime(2026, 11, 3, 10, 0, 0, DateTimeKind.Utc);
        var sanction = new ShopSanction(
            shopId: Guid.NewGuid(),
            type: ShopSanctionType.TemporarySuspension,
            reason: "Vi phạm chính sách giao hàng",
            createdByAdminId: Guid.NewGuid(),
            startAtUtc: now.AddHours(-1),
            endAtUtc: now.AddHours(2));

        sanction.IsSuspensionActive(now).Should().BeTrue();
        sanction.IsSuspensionActive(now.AddHours(3)).Should().BeFalse();

        sanction.Lift(Guid.NewGuid());
        sanction.IsSuspensionActive(now).Should().BeFalse();
    }

    [Fact]
    public void LedgerTransaction_EnsureBalanced_ShouldThrowWhenFewerThanTwoEntries()
    {
        var tx = new LedgerTransaction(
            LedgerTransactionType.OrderSettlement,
            "ShopOrder",
            Guid.NewGuid(),
            "idem-key-1",
            "Test memo");

        tx.AddEntry(new LedgerEntry(tx.Id, Guid.NewGuid(), debit: 100_000, credit: 0));

        var act = () => tx.EnsureBalanced();
        act.Should().Throw<ArgumentException>()
            .WithMessage("*ít nhất 2 bút toán*");
    }

    [Fact]
    public void LedgerTransaction_EnsureBalanced_ShouldThrowWhenDebitDoesNotEqualCredit()
    {
        var tx = new LedgerTransaction(
            LedgerTransactionType.OrderSettlement,
            "ShopOrder",
            Guid.NewGuid(),
            "idem-key-2",
            "Test memo");

        tx.AddEntry(new LedgerEntry(tx.Id, Guid.NewGuid(), debit: 100_000, credit: 0));
        tx.AddEntry(new LedgerEntry(tx.Id, Guid.NewGuid(), debit: 0, credit: 90_000));

        var act = () => tx.EnsureBalanced();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Tổng Nợ (100000) != Tổng Có (90000)*");
    }

    [Fact]
    public void LedgerTransaction_EnsureBalanced_ShouldSucceedWhenBalanced()
    {
        var tx = new LedgerTransaction(
            LedgerTransactionType.OrderSettlement,
            "ShopOrder",
            Guid.NewGuid(),
            "idem-key-3",
            "Test memo");

        tx.AddEntry(new LedgerEntry(tx.Id, Guid.NewGuid(), debit: 100_000, credit: 0));
        tx.AddEntry(new LedgerEntry(tx.Id, Guid.NewGuid(), debit: 0, credit: 95_000));
        tx.AddEntry(new LedgerEntry(tx.Id, Guid.NewGuid(), debit: 0, credit: 5_000));

        var act = () => tx.EnsureBalanced();
        act.Should().NotThrow();
    }

    [Fact]
    public void Checkout_Constructor_ShouldAssignExplicitIdWhenProvided()
    {
        var explicitId = Guid.NewGuid();
        var checkout = new Checkout(
            checkoutNo: "CHK-20261103-001",
            buyerUserId: Guid.NewGuid(),
            itemsTotal: 250_000,
            discountTotal: 0,
            grandTotal: 250_000,
            expiresAtUtc: DateTime.UtcNow.AddMinutes(15),
            idempotencyKey: "chk-idem-1",
            id: explicitId);

        checkout.Id.Should().Be(explicitId);
    }
}
