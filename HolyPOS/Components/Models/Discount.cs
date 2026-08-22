using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("discounts")]
public class Discount : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("type")]
    public DiscountType DiscountType { get; set; }

    [Column("value")]
    public decimal Value { get; set; }
}
public enum DiscountType
{
    Percentage = 0,
    FixedAmount = 1
}