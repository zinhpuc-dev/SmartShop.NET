using SmartShop.User.Components;
using SmartShop.Admin.Security;
using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.DTO.Contracts;

static string GetSmartShopConnectionString(IServiceProvider services)
{
    string? connectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("SmartShop");
    connectionString ??= Environment.GetEnvironmentVariable("SMARTSHOP_DB_CONNECTION");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Vui lòng cấu hình kết nối SmartShop trong User Secrets hoặc SMARTSHOP_DB_CONNECTION.");
    return connectionString;
}

var builder = WebApplication.CreateBuilder(args);
// The composition root selects storage; pages and BUS depend only on contracts.
builder.Services.AddScoped<IShopRepository, DemoShopDao>();
builder.Services.AddScoped<IShopService, ShopState>();
builder.Services.AddScoped<ICustomerRepository>(services => new CustomerDAO(GetSmartShopConnectionString(services)));
builder.Services.AddScoped<ICustomerBUS, CustomerBUS>();
builder.Services.AddScoped<IEmployeeRepository>(services => new EmployeeDAO(GetSmartShopConnectionString(services)));
builder.Services.AddScoped<IEmployeeBUS, EmployeeBUS>();
builder.Services.AddScoped<ISupplierRepository>(services => new SupplierDAO(GetSmartShopConnectionString(services)));
builder.Services.AddScoped<ISupplierBUS, SupplierBUS>();
builder.Services.AddScoped<IProductManage>(services =>
{
    string? connectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("SmartShop");
    connectionString ??= Environment.GetEnvironmentVariable("SMARTSHOP_DB_CONNECTION");
    return string.IsNullOrWhiteSpace(connectionString)
        ? new ProductDAO()
        : new ProductDAO(connectionString);
});
builder.Services.AddScoped<ProductBUS>();
builder.Services.AddScoped<IWarehouseRepository>(services => new WarehouseDAO(GetSmartShopConnectionString(services)));
builder.Services.AddScoped<WarehouseBUS>();
builder.Services.AddScoped<IOrderManagementRepository>(services => new OrderDAO(GetSmartShopConnectionString(services)));
builder.Services.AddScoped<OrderBUS>();
builder.Services.AddScoped<IInvoiceManagementRepository>(services => new InvoiceDAO(GetSmartShopConnectionString(services)));
builder.Services.AddScoped<InvoiceBUS>();
builder.Services.AddScoped<IPromotionRepository>(services => new PromotionDAO(GetSmartShopConnectionString(services)));
builder.Services.AddScoped<PromotionBUS>();
builder.Services.AddScoped<SmartShop.UI.Shared.ShopUiState>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSmartShopAdmin();
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
app.MapSmartShopAdmin();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(SmartShop.Admin.Components.Pages.Admin).Assembly)
    .AddInteractiveServerRenderMode();

app.Run();
