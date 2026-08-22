using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("transaction_payments")]
public class TransactionPayment : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("transaction_id")]
    public Guid TransactionId { get; set; }

    [Column("payment_type_id")]
    public Guid? PaymentTypeId { get; set; }

    [Column("payment_type_name")]
    public string PaymentTypeName { get; set; } = "";

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("cash_received")]
    public decimal CashReceived { get; set; }

    [Column("change_amount")]
    public decimal ChangeAmount { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}