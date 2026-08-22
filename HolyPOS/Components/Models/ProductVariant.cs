using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("product_variants")]
public class ProductVariant : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("price")]
    public decimal Price { get; set; }

    [Column("cost")]
    public decimal Cost { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
