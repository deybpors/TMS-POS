using HolyPOS.Components;
using HolyPOS.Components.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<DatabaseData>();
builder.Services.AddScoped<ProtectedLocalStorage>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<
    AuthenticationStateProvider,
    CustomAuthStateProvider
>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();