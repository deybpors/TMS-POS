using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("transaction_items")]
public class TransactionItem : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("transaction_id")]
    public Guid TransactionId { get; set; }


    // Product snapshot

    [Column("product_id")]
    public Guid? ProductId { get; set; }

    [Column("product_name")]
    public string ProductName { get; set; } = "";


    // Category snapshot

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Column("category_name")]
    public string? CategoryName { get; set; }


    // Pricing snapshot

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }
    
    // Sale data

    [Column("quantity")]
    public decimal Quantity { get; set; }

    [Column("discount_id")]
    public Guid? DiscountId { get; set; }
    
    
    
}