using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("products")]
public class Product : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("sold_by")]
    public SellUnit SoldBy { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("cost")]
    public decimal Cost { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("code")] 
    public string ProductCode { get; set; }
}