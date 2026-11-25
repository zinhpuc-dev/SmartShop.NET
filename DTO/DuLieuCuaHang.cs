namespace SmartShop.DTO;

public sealed record SanPhamDTO(int Id, string Name, string Category, string Color,
    decimal Price, decimal Original, int Stock, string Spec)
{
    public string Code { get; init; } = "";
    public string CategoryCode { get; init; } = "";
}

public sealed record DonHangDTO(string Code, string Customer, string Phone, string Address,
    DateTime Created, IReadOnlyList<ChiTietDonHangDTO> Items, decimal Total)
{
    public string Status { get; init; } = "Chờ xác nhận";
    public string CustomerCode { get; init; } = "";
}

public sealed record ChiTietDonHangDTO(SanPhamDTO Product, int Quantity);
