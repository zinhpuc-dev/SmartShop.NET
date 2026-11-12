namespace SmartShop.DTO;

public sealed record DonHangQuanTriDTO(string Code, string CustomerCode, string CustomerName, DateTime Created, string Status, string Address, decimal Total, string? WarehouseCode, string? InvoiceCode, string? EmployeeCode, string? EmployeeName, IReadOnlyList<ChiTietDonHangQuanTriDTO> Lines);

public sealed record ChiTietDonHangQuanTriDTO(string ProductCode, string ProductName, int Quantity, decimal UnitPrice, decimal ActualUnitPrice, decimal LineTotal);

public sealed record ThongKeBanHangDTO(DateTime From, DateTime To, int OrderCount, int InvoiceCount, decimal Revenue, int ProductsSold);
