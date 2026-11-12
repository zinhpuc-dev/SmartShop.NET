using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartShop.Admin.Components.Layout;
using SmartShop.Admin.Security;
using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.UI.Shared;

internal static class KiemTraKhungQuanTri
{
    public static async Task Chay()
    {
        var phien = new PhienDangNhapGiaLap();
        var services = new ServiceCollection();
        services.AddLogging();
        // Dùng đúng policy đăng ký trong ứng dụng, chỉ thay nguồn trạng thái phiên.
        services.DangKyQuanTri();
        services.AddSingleton<AuthenticationStateProvider>(phien);
        services.AddSingleton<NavigationManager>(new DieuHuongGiaLap());
        services.AddSingleton<AntiforgeryStateProvider, TokenBieuMauGiaLap>();
        services.AddSingleton<ICuaHangBUS>(new CuaHangBUS(new CuaHangGiaLapDAO()));
        services.AddSingleton<TrangThaiGiaoDien>();
        await using var provider = services.BuildServiceProvider();
        await using var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());
        var soLanRenderNoiDung = 0;
        RenderFragment noiDung = builder =>
        {
            soLanRenderNoiDung++;
            builder.AddContent(0, "NOI_DUNG_QUAN_TRI");
        };
        RenderFragment khung = builder =>
        {
            builder.OpenComponent<KhungQuanTri>(0);
            builder.AddAttribute(1, "ChildContent", noiDung);
            builder.CloseComponent();
        };
        var root = await renderer.Dispatcher.InvokeAsync(() => renderer.RenderComponentAsync<CascadingAuthenticationState>(
            ParameterView.FromDictionary(new Dictionary<string, object?> { ["ChildContent"] = khung })));
        async Task<string> DocHtml() => await renderer.Dispatcher.InvokeAsync(root.ToHtmlString);
        async Task DoiPhien(ClaimsPrincipal user) => await renderer.Dispatcher.InvokeAsync(() => phien.DoiPhien(user));
        static void KiemTra(bool dung, string moTa)
        {
            if (!dung) throw new Exception(moTa);
            Console.WriteLine("PASS SHELL " + moTa);
        }
        var html = await DocHtml();
        KiemTra(!html.Contains("admin-shell") && !html.Contains("NOI_DUNG_QUAN_TRI") && soLanRenderNoiDung == 0,
            "Khách ẩn danh không render menu hoặc nội dung quản trị");

        await DoiPhien(TaoNguoiDung("nhan-vien", "User"));
        html = await DocHtml();
        KiemTra(!html.Contains("admin-shell") && soLanRenderNoiDung == 0,
            "Tài khoản đã đăng nhập thiếu quyền Admin vẫn bị chặn");

        await DoiPhien(TaoNguoiDung("quan-tri-thu", "Admin"));
        html = await DocHtml();
        KiemTra(html.Contains("admin-shell") && html.Contains("NOI_DUNG_QUAN_TRI") && html.Contains("quan-tri-thu"),
            "Admin xem nội dung và tên từ phiên đăng nhập");
        KiemTra(html.Contains("class=\"nav-item active\"") && html.Contains("class=\"active\""),
            "Trang đang mở được đánh dấu ở menu desktop và mobile");

        var daRender = soLanRenderNoiDung;
        await DoiPhien(new ClaimsPrincipal(new ClaimsIdentity()));
        html = await DocHtml();
        KiemTra(!html.Contains("admin-shell") && !html.Contains("NOI_DUNG_QUAN_TRI")
            && html.Contains("/admin/login") && soLanRenderNoiDung == daRender,
            "Phiên hết hạn gỡ menu và nội dung, hiện lối đăng nhập lại");

        await DoiPhien(TaoNguoiDung("<script>alert(1)</script>", "Admin"));
        html = await DocHtml();
        KiemTra(!html.Contains("<script>") && html.Contains("&lt;script&gt;"),
            "Tên đăng nhập được HTML encode trước khi hiển thị");
    }

    private static ClaimsPrincipal TaoNguoiDung(string ten, string quyen) => new(new ClaimsIdentity(
        [new Claim(ClaimTypes.Name, ten), new Claim(ClaimTypes.Role, quyen)], DangNhapQuanTri.Scheme));

    private sealed class PhienDangNhapGiaLap : AuthenticationStateProvider
    {
        private AuthenticationState phien = new(new ClaimsPrincipal(new ClaimsIdentity()));
        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(phien);
        public void DoiPhien(ClaimsPrincipal user)
        {
            phien = new(user);
            NotifyAuthenticationStateChanged(Task.FromResult(phien));
        }
    }

    private sealed class DieuHuongGiaLap : NavigationManager
    {
        public DieuHuongGiaLap() => Initialize("http://localhost/", "http://localhost/admin/customers");
        protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException();
    }

    private sealed class TokenBieuMauGiaLap : AntiforgeryStateProvider
    {
        public override AntiforgeryRequestToken GetAntiforgeryToken() => new("test-token", "test-field");
    }
}
