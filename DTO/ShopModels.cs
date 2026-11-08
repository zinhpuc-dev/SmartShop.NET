namespace SmartShop.DTO;

public sealed record Product(int Id, string Name, string Category, string Color,
    decimal Price, decimal Original, int Stock, string Spec)
{
    public string Code { get; init; } = "";
    public string CategoryCode { get; init; } = "";
}

public sealed record Order(string Code, string Customer, string Phone, string Address,
    DateTime Created, IReadOnlyList<OrderLine> Items, decimal Total)
{
    public string Status { get; init; } = "Chờ xác nhận";
    public string CustomerCode { get; init; } = "";
}

public sealed record OrderLine(Product Product, int Quantity);
