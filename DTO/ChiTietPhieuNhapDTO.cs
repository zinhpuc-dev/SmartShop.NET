namespace SmartShop.DTO;

public sealed record ChiTietPhieuNhapDTO(string ProductCode, string ProductName, int Quantity, decimal UnitCost, decimal Total);
public sealed record TonKhoDTO(string WarehouseCode, string WarehouseName, string ProductCode, string ProductName, int Quantity);
