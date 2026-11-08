namespace SmartShop.DTO;

public sealed record ReceivingDetail(string ProductCode, string ProductName, int Quantity, decimal UnitCost, decimal Total);
public sealed record WarehouseStock(string WarehouseCode, string WarehouseName, string ProductCode, string ProductName, int Quantity);
