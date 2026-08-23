namespace HolyPOS.Components.Models;

public class CurrentUser
{
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";

    public bool IsAdmin =>
        Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

    public bool IsUser =>
        Role.Equals("User", StringComparison.OrdinalIgnoreCase);
}