using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("payment_types")]
public class PaymentType : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";
    
    [Column("is_cash")]
    public bool IsCash { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}