using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HolyPOS.Components.Models;

[Table("stores")]
public class Store : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("address")]
    public string Address { get; set; } = "";


    [Column("is_active")]
    public bool Active { get; set; } = true;
}