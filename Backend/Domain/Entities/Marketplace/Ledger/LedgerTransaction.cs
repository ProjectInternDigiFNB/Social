using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Marketplace.Ledger;

/// <summary>
/// Giao dịch kế toán tổng thể (chứa tập hợp các bút toán LedgerEntry)
/// Bất biến: Tổng Nợ (Debit) PHẢI BẰNG Tổng Có (Credit) trong mọi Transaction
/// </summary>
public class LedgerTransaction : BaseEntity
{
    public LedgerTransactionType Type { get; private set; }
    public string RefType { get; private set; } = string.Empty; // "Checkout", "ShopOrder", "Payout", "Refund"
    public Guid RefId { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;
    public string Memo { get; private set; } = string.Empty;

    // Navigation
    public ICollection<LedgerEntry> Entries { get; private set; } = new List<LedgerEntry>();

    protected LedgerTransaction() { }

    public LedgerTransaction(
        LedgerTransactionType type,
        string refType,
        Guid refId,
        string idempotencyKey,
        string memo)
    {
        if (string.IsNullOrWhiteSpace(refType)) throw new ArgumentException("RefType không được để trống.", nameof(refType));
        if (refId == Guid.Empty) throw new ArgumentException("RefId không hợp lệ.", nameof(refId));
        if (string.IsNullOrWhiteSpace(idempotencyKey)) throw new ArgumentException("IdempotencyKey không được để trống.", nameof(idempotencyKey));

        Type = type;
        RefType = refType.Trim();
        RefId = refId;
        IdempotencyKey = idempotencyKey.Trim();
        Memo = string.IsNullOrWhiteSpace(memo) ? string.Empty : memo.Trim();
    }

    public void AddEntry(LedgerEntry entry)
    {
        Entries.Add(entry);
    }

    /// <summary>
    /// Kiểm tra bất biến sổ cái kép: Có ít nhất 2 bút toán, Tổng Nợ == Tổng Có và lớn hơn 0.
    /// </summary>
    public void EnsureBalanced()
    {
        if (Entries.Count < 2)
        {
            throw new ArgumentException("Một giao dịch sổ cái phải có ít nhất 2 bút toán (Nợ và Có).");
        }

        var totalDebit = Entries.Sum(e => e.Debit);
        var totalCredit = Entries.Sum(e => e.Credit);

        if (totalDebit != totalCredit)
        {
            throw new InvalidOperationException($"Bút toán sổ cái không cân: Tổng Nợ ({totalDebit}) != Tổng Có ({totalCredit}).");
        }

        if (totalDebit <= 0)
        {
            throw new InvalidOperationException("Tổng số tiền của giao dịch sổ cái phải lớn hơn 0.");
        }
    }
}
