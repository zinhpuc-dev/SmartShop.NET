namespace SmartShop.DTO;

public sealed record ManagedOrder(string Code, string CustomerCode, string CustomerName, DateTime Created, string Status, string Address, decimal Total, string? WarehouseCode, string? InvoiceCode, string? EmployeeCode, string? EmployeeName, IReadOnlyList<ManagedOrderLine> Lines);

public sealed record ManagedOrderLine(string ProductCode, string ProductName, int Quantity, decimal UnitPrice, decimal ActualUnitPrice, decimal LineTotal);

public sealed record SalesSummary(DateTime From, DateTime To, int OrderCount, int InvoiceCount, decimal Revenue, int ProductsSold);
