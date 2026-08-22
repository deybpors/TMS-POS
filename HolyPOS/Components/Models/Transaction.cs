using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("transactions")]
public class Transaction : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }


    // Receipt number shown to the customer
    [Column("receipt_id")]
    public string ReceiptId { get; set; } = "";


    // Store where the transaction happened
    [Column("store_id")]
    public Guid? StoreId { get; set; }


    // Actual date/time of the transaction
    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; }


    // ============================================================
    // TRANSACTION TOTALS
    // ============================================================

    [Column("gross_sales")]
    public decimal GrossSales { get; set; }


    [Column("refunds")]
    public decimal Refunds { get; set; }


    [Column("discounts")]
    public decimal Discounts { get; set; }


    [Column("net_sales")]
    public decimal NetSales { get; set; }


    [Column("cost")]
    public decimal Cost { get; set; }


    [Column("gross_profit")]
    public decimal GrossProfit { get; set; }


    // ============================================================
    // REFUND
    // ============================================================

    [Column("is_refunded")]
    public bool IsRefunded { get; set; }


    // If this is a refund, this points to
    // the original sale.
    [Column("original_transaction_id")]
    public Guid? OriginalTransactionId { get; set; }


    // ============================================================
    // DATABASE TIMESTAMP
    // ============================================================

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}