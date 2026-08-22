using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("product_stores")]
public class ProductStore : BaseModel
{
    [PrimaryKey("product_id", false)]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    [PrimaryKey("store_id", false)]
    [Column("store_id")]
    public Guid StoreId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("price_override")]
    public decimal? PriceOverride { get; set; }
}