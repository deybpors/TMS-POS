using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("product_modifiers")]
public class ProductModifier : BaseModel
{
    [PrimaryKey("product_id", false)]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    [PrimaryKey("modifier_id", false)]
    [Column("modifier_id")]
    public Guid ModifierId { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }
}