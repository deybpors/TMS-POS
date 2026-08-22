using Supabase;

namespace HolyPOS.Components.Services;

public class SupabaseService
{
    private readonly Client _client;

    public Client Client => _client;

    public SupabaseService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"];
        var key = configuration["Supabase:PublishableKey"];

        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException("Supabase URL is missing.");

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Supabase publishable key is missing.");

        _client = new Client(
            url,
            key,
            new SupabaseOptions
            {
                AutoConnectRealtime = false
            });
    }

    public async Task InitializeAsync()
    {
        await _client.InitializeAsync();
    }
}