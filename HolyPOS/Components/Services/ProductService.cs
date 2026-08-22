using HolyPOS.Components.Models;

namespace HolyPOS.Components.Services;

public class ProductService
{
    private readonly SupabaseService _supabase;

    public ProductService(SupabaseService supabase)
    {
        _supabase = supabase;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        // SUPABASE:
        // Get products from the products table.

        var response = await _supabase.Client
            .From<Product>()
            .Get();

        return response.Models;
    }

    public async Task<Product?> GetProductAsync(Guid id)
    {
        // SUPABASE:
        // Get one product.

        var response = await _supabase.Client
            .From<Product>()
            .Where(x => x.Id == id)
            .Single();

        return response;
    }

    public async Task AddProductAsync(Product product)
    {
        // SUPABASE:
        // Insert product.

        await _supabase.Client
            .From<Product>()
            .Insert(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        // SUPABASE:
        // Update product.

        await _supabase.Client
            .From<Product>()
            .Update(product);
    }

    public async Task DeleteProductAsync(Product product)
    {
        // SUPABASE:
        // Delete product.

        await _supabase.Client
            .From<Product>()
            .Delete(product);
    }
}