namespace HolyPOS.Components.Models;

public class CartItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ProductId { get; set; } = "";

    public string ProductName { get; set; } = "";

    public string VariantName { get; set; } = "";

    public decimal BasePrice { get; set; }

    public int Quantity { get; set; }

    public List<CartModifier> Modifiers { get; set; } = new();

    public decimal Discount { get; set; }

    public decimal Total
    {
        get
        {
            decimal modifierTotal =
                Modifiers.Sum(x => x.Price);

            return
                ((BasePrice + modifierTotal) * Quantity)
                - Discount;
        }
    }
}

public class CartModifier
{
    public string ModifierId { get; set; } = "";

    public string Name { get; set; } = "";

    public decimal Price { get; set; }
}