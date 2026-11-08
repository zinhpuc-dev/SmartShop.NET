using SmartShop.BUS;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

internal static class OrderManagementChecks
{
    public static void Run()
    {
        var repository = new FakeOrderRepository();
        var service = new OrderManagementService(repository);
        service.UpdateStatus(" dh000001 ", "Đã xác nhận", "k00001");
        Check(repository.LastCode == "DH000001" && repository.LastStatus == "Đã xác nhận" && repository.LastWarehouse == "K00001",
            "ORDERS normalize order and warehouse keys through BUS");
        Reject(() => service.UpdateStatus("DH000001", "Unknown", "K00001"), "ORDERS reject unsupported status");
        Reject(() => service.UpdateStatus("DH000001", "Đã giao", ""), "ORDERS require a warehouse before delivery completion");
        Reject(() => service.GetSalesSummary(DateTime.Today, DateTime.Today), "REPORT reject empty date range");
        Check(service.GetSalesSummary(DateTime.Today.AddDays(-1), DateTime.Today).Revenue == 100m,
            "REPORT delegates revenue summary to persistence");
    }

    private static void Check(bool value, string message)
    {
        if (!value) throw new Exception(message);
        Console.WriteLine("PASS " + message);
    }

    private static void Reject(Action action, string message)
    {
        try { action(); } catch (ArgumentException) { Console.WriteLine("PASS " + message); return; }
        throw new Exception(message);
    }

    private sealed class FakeOrderRepository : IOrderManagementRepository
    {
        public string? LastCode { get; private set; }
        public string? LastStatus { get; private set; }
        public string? LastWarehouse { get; private set; }
        public IReadOnlyList<ManagedOrder> GetOrders() => [];
        public void UpdateStatus(string orderCode, string status, string warehouseCode)
        {
            LastCode = orderCode;
            LastStatus = status;
            LastWarehouse = warehouseCode;
        }
        public IReadOnlyList<Warehouse> GetWarehouses() => [];
        public SalesSummary GetSalesSummary(DateTime from, DateTime to) => new(from, to, 1, 1, 100m, 1);
    }
}
