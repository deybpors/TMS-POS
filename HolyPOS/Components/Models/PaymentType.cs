using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("payment_types")]
public class PaymentType : BaseModel
{
    [PrimaryKey("id", true)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("is_cash")]
    public bool IsCash { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}