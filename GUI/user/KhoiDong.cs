using SmartShop.User.Components;
using SmartShop.Admin.Security;
using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.DTO.Contracts;

static string LayChuoiKetNoiSmartShop(IServiceProvider services)
{
    string? connectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("SmartShop");
    connectionString ??= Environment.GetEnvironmentVariable("SMARTSHOP_DB_CONNECTION");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Vui lòng cấu hình kết nối SmartShop trong User Secrets hoặc SMARTSHOP_DB_CONNECTION.");
    return connectionString;
}

var builder = WebApplication.CreateBuilder(args);
// The composition root selects storage; pages and BUS depend only on contracts.
builder.Services.AddScoped<ICuaHangDAO, CuaHangGiaLapDAO>();
builder.Services.AddScoped<ICuaHangBUS, CuaHangBUS>();
builder.Services.AddScoped<IKhachHangDAO>(services => new KhachHangDAO(LayChuoiKetNoiSmartShop(services)));
builder.Services.AddScoped<IKhachHangBUS, KhachHangBUS>();
builder.Services.AddScoped<INhanVienDAO>(services => new NhanVienDAO(LayChuoiKetNoiSmartShop(services)));
builder.Services.AddScoped<INhanVienBUS, NhanVienBUS>();
builder.Services.AddScoped<INhaCungCapDAO>(services => new NhaCungCapDAO(LayChuoiKetNoiSmartShop(services)));
builder.Services.AddScoped<INhaCungCapBUS, NhaCungCapBUS>();
builder.Services.AddScoped<ISanPhamDAO>(services =>
{
    string? connectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("SmartShop");
    connectionString ??= Environment.GetEnvironmentVariable("SMARTSHOP_DB_CONNECTION");
    return string.IsNullOrWhiteSpace(connectionString)
        ? new SanPhamDAO()
        : new SanPhamDAO(connectionString);
});
builder.Services.AddScoped<SanPhamBUS>();
builder.Services.AddScoped<IKhoDAO>(services => new KhoDAO(LayChuoiKetNoiSmartShop(services)));
builder.Services.AddScoped<KhoBUS>();
builder.Services.AddScoped<IDonHangDAO>(services => new DonHangDAO(LayChuoiKetNoiSmartShop(services)));
builder.Services.AddScoped<DonHangBUS>();
builder.Services.AddScoped<IHoaDonDAO>(services => new HoaDonDAO(LayChuoiKetNoiSmartShop(services)));
builder.Services.AddScoped<HoaDonBUS>();
builder.Services.AddScoped<IKhuyenMaiDAO>(services => new KhuyenMaiDAO(LayChuoiKetNoiSmartShop(services)));
builder.Services.AddScoped<KhuyenMaiBUS>();
builder.Services.AddScoped<SmartShop.UI.Shared.TrangThaiGiaoDien>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.DangKyQuanTri();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseAntiforgery();
app.KhaiBaoDuongDanQuanTri();

app.MapStaticAssets();
app.MapRazorComponents<UngDung>()
    .AddAdditionalAssemblies(typeof(SmartShop.Admin.Components.Pages.QuanTri).Assembly)
    .AddInteractiveServerRenderMode();

app.Run();
