namespace SmartShop.DTO;

public sealed record ManagedInvoice(string Code, string OrderCode, string EmployeeCode, string EmployeeName, DateTime Created, decimal Total, string PaymentMethod, string Status, IReadOnlyList<ManagedOrderLine> Lines);
