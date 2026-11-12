using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace SmartShop.Admin.Security;

// Single administrator for this local installation. Only a salted hash is stored.
public sealed class DangNhapQuanTri(IConfiguration configuration)
{
    public const string Scheme = "SmartShopAdmin";
    public const string Policy = "SmartShopAdminOnly";
    private const string InitialHash = "AQAAAAIAAYagAAAAEBcuuussIGtSF4bUzWf5gBjzcvUpgYNHPjkk7L9GlLknRsR8/GnCqUbnGN8Dx4a70w==";
    private readonly PasswordHasher<string> hasher = new();

    public string Username => configuration["Admin:Username"] ?? "admin";

    public bool KiemTraDuLieu(string username, string password)
    {
        if (password.Length is 0 or > 256) return false;
        string storedHash = configuration["Admin:PasswordHash"] ?? InitialHash;
        try
        {
            var result = hasher.VerifyHashedPassword(Username, storedHash, password);
            return string.Equals(username.Trim(), Username, StringComparison.Ordinal)
                && result != PasswordVerificationResult.Failed;
        }
        catch (FormatException) { return false; }
    }
}
