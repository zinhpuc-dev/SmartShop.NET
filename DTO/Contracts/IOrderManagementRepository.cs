namespace SmartShop.DTO.Contracts;

public interface IOrderManagementRepository
{
    IReadOnlyList<ManagedOrder> GetOrders();
    void UpdateStatus(string orderCode, string status, string warehouseCode);
    IReadOnlyList<Warehouse> GetWarehouses();
    SalesSummary GetSalesSummary(DateTime from, DateTime to);
}
