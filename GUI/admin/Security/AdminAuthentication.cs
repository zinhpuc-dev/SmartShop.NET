using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace SmartShop.Admin.Security;

public static class AdminAuthentication
{
    public static IServiceCollection AddSmartShopAdmin(this IServiceCollection services)
    {
        services.AddSingleton<AdminLoginService>();
        services.AddSingleton<AdminSessions>();
        services.AddCascadingAuthenticationState();
        services.AddScoped<AuthenticationStateProvider, AdminAuthenticationStateProvider>();
        services.AddAuthentication(AdminLoginService.Scheme).AddCookie(AdminLoginService.Scheme, options =>
        {
            options.Cookie.Name = "SmartShop.Admin";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.LoginPath = "/admin/login";
            options.AccessDeniedPath = "/admin/login";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = false;
            options.Events.OnValidatePrincipal = context =>
            {
                if (!context.HttpContext.RequestServices.GetRequiredService<AdminSessions>().IsValid(context.Principal!))
                    context.RejectPrincipal();
                return Task.CompletedTask;
            };
        });
        services.AddAuthorization(options => options.AddPolicy(AdminLoginService.Policy,
            policy => policy.AddAuthenticationSchemes(AdminLoginService.Scheme).RequireAuthenticatedUser().RequireRole("Admin")));
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("admin-signin", context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "local", _ => new FixedWindowRateLimiterOptions
                { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
        });
        return services;
    }

    public static WebApplication MapSmartShopAdmin(this WebApplication app)
    {
        // A normal HTTP form is needed to set the authentication cookie, outside the Blazor circuit.
        app.MapGet("/admin/login", IResult (HttpContext context, IAntiforgery antiforgery) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            if (context.User.IsInRole("Admin")) return Results.Redirect("/admin");
            var tokens = antiforgery.GetAndStoreTokens(context);
            string token = HtmlEncoder.Default.Encode(tokens.RequestToken ?? "");
            string field = HtmlEncoder.Default.Encode(tokens.FormFieldName);
            string message = context.Request.Query.ContainsKey("error")
                ? "Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng thử lại." : "";
            string returnUrl = HtmlEncoder.Default.Encode(LocalAdminUrl(context.Request.Query["returnUrl"].ToString()));
            string html = $$"""
                <!doctype html><html lang="vi"><head><meta charset="utf-8">
                <meta name="viewport" content="width=device-width,initial-scale=1"><title>Đăng nhập quản trị — SmartShop</title>
                <link rel="stylesheet" href="/app.css"><style>
                body{min-height:100vh;display:grid;place-items:center;margin:0}.login{width:min(420px,92vw);padding:32px}
                .login label{display:grid;gap:8px;margin:18px 0}.login input{width:100%;box-sizing:border-box;padding:12px}
                .login button{width:100%;padding:12px}.login .error{color:#ff8e99}.login a{display:block;margin-top:22px}
                </style></head><body><main class="panel login"><h1>Đăng nhập quản trị</h1>
                <p>Chào mừng bạn quay lại SmartShop.</p><p class="error" role="alert">{{message}}</p>
                <form method="post" action="/admin/sign-in">
                <input type="hidden" name="{{field}}" value="{{token}}">
                <input type="hidden" name="returnUrl" value="{{returnUrl}}">
                <label>Tên đăng nhập<input name="username" autocomplete="username" maxlength="100" required autofocus></label>
                <label>Mật khẩu<input type="password" name="password" autocomplete="current-password" maxlength="256" required></label>
                <button class="primary" type="submit">Đăng nhập</button></form><a href="/">← Về cửa hàng</a>
                </main></body></html>
                """;
            return Results.Content(html, "text/html; charset=utf-8");
        }).AllowAnonymous();

        app.MapPost("/admin/sign-in", async Task<IResult> (HttpContext context, IAntiforgery antiforgery,
            AdminLoginService login, AdminSessions sessions) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            try { await antiforgery.ValidateRequestAsync(context); }
            catch (AntiforgeryValidationException) { return Results.BadRequest("Phiên biểu mẫu đã hết hạn. Vui lòng tải lại trang đăng nhập."); }
            if (!context.Request.HasFormContentType) return Results.BadRequest();
            var form = await context.Request.ReadFormAsync();
            string next = LocalAdminUrl(form["returnUrl"].ToString());
            if (!login.Validate(form["username"].ToString(), form["password"].ToString()))
                return Results.Redirect("/admin/login?error=1&returnUrl=" + Uri.EscapeDataString(next));
            DateTimeOffset expires = DateTimeOffset.UtcNow.AddHours(8);
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, login.Username), new Claim(ClaimTypes.Role, "Admin"),
                new Claim("admin_session", sessions.Create(expires)) }, AdminLoginService.Scheme);
            await context.SignInAsync(AdminLoginService.Scheme, new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = false, ExpiresUtc = expires });
            return Results.Redirect(next);
        }).AllowAnonymous().RequireRateLimiting("admin-signin");

        app.MapPost("/admin/sign-out", async Task<IResult> (HttpContext context, IAntiforgery antiforgery, AdminSessions sessions) =>
        {
            try { await antiforgery.ValidateRequestAsync(context); }
            catch (AntiforgeryValidationException) { return Results.BadRequest("Vui lòng tải lại trang trước khi đăng xuất."); }
            sessions.Revoke(context.User);
            await context.SignOutAsync(AdminLoginService.Scheme);
            return Results.Redirect("/admin/login");
        }).RequireAuthorization(AdminLoginService.Policy);
        return app;
    }

    private static string LocalAdminUrl(string value)
        => value == "/admin" || (value.StartsWith("/admin/", StringComparison.Ordinal)
            && !value.Contains('\\') && !value.Contains('\r') && !value.Contains('\n')
            && !value.StartsWith("/admin/login", StringComparison.Ordinal)
            && !value.StartsWith("/admin/sign-", StringComparison.Ordinal)) ? value : "/admin";
}
