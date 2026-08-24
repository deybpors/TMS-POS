using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("transaction_item_modifiers")]
public class TransactionItemModifier : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("transaction_item_id")]
    public Guid TransactionItemId { get; set; }

    [Column("modifier_id")]
    public Guid ModifierId { get; set; }

    [Column("option_id")]
    public Guid? OptionId { get; set; }
}