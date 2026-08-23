using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("cash_counts")]
public class CashCount : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }


    // The business day this cash count is for.
    [Column("date")]
    public DateTime Date { get; set; }


    // Store this cash count was taken at.
    [Column("store_id")]
    public Guid? StoreId { get; set; }


    // Whoever counted the drawer — just a plain string,
    // e.g. "Juan, Maria"
    [Column("sellers")]
    public string Sellers { get; set; } = "";


    // ============================================================
    // CASH FIGURES
    // ============================================================

    [Column("beginning_balance")]
    public decimal BeginningBalance { get; set; }


    // Stored as JSON in the database — a simple list of
    // { description, amount } entries logged during the day.
    [Column("expenses")]
    public List<CashCountExpense> Expenses { get; set; } = new();


    // The actual physical cash counted at end of day.
    [Column("cash_count_amount")]
    public decimal CashCountAmount { get; set; }


    // ============================================================
    // DATABASE TIMESTAMP
    // ============================================================

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("is_verified")]
    public bool IsVerified { get; set; }
}


// A single expense line item, stored as part of the
// CashCount's "expenses" JSON column.
public class CashCountExpense
{
    public string Description { get; set; } = "";

    public decimal Amount { get; set; }
}