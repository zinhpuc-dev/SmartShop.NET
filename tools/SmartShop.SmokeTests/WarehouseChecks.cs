using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

internal static class WarehouseChecks
{
    public static void Run()
    {
        var repository = new FakeWarehouseRepository();
        var service = new WarehouseService(repository, new FakeSupplierRepository(), new FakeProductRepository());
        Check(service.SuggestCode() == "K00002", "WAREHOUSE suggest next schema-sized warehouse code");
        service.Save(new Warehouse { Code = " k00003 ", Name = " Kho thử " }, editing: false);
        Check(repository.LastWarehouse?.Code == "K00003" && repository.LastWarehouse.Name == "Kho thử", "WAREHOUSE normalize and validate warehouse writes");
        var draft = new ReceivingDraft
        {
            Code = " pn00009 ", SupplierCode = " n00001 ", WarehouseCode = " k00001 ",
            Lines = [new ReceivingLineDraft { ProductCode = " sp000001 ", Quantity = 3, UnitCost = 125.50m }]
        };
        service.CreateReceiving(draft);
        Check(repository.LastReceiving?.Code == "PN00009" && repository.LastReceiving.Lines[0].ProductCode == "SP000001", "RECEIVING normalize codes before persistence");
        Check(repository.CreateReceivingCalls == 1, "RECEIVING dispatch one atomic DAO operation");
        try
        {
            service.CreateReceiving(new ReceivingDraft { Code = "PN00010", SupplierCode = "N00001", WarehouseCode = "K00001", Lines = [] });
            throw new Exception("RECEIVING empty line list should be rejected");
        }
        catch (ArgumentException) { Console.WriteLine("PASS RECEIVING reject empty receipt"); }
        try
        {
            service.CreateReceiving(new ReceivingDraft
            {
                Code = "PN00011", SupplierCode = "N00001", WarehouseCode = "K00001",
                Lines = [new ReceivingLineDraft { ProductCode = "SP000001", Quantity = 1, UnitCost = 0 }]
            });
            throw new Exception("RECEIVING zero cost should be rejected");
        }
        catch (ArgumentException) { Console.WriteLine("PASS RECEIVING reject invalid unit cost"); }
    }

    private static void Check(bool value, string name)
    {
        if (!value) throw new Exception(name);
        Console.WriteLine("PASS " + name);
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        public Warehouse? LastWarehouse { get; private set; }
        public ReceivingDraft? LastReceiving { get; private set; }
        public int CreateReceivingCalls { get; private set; }
        public IReadOnlyList<Warehouse> GetAll() => [new Warehouse { Code = "K00001", Name = "Kho hiện có" }];
        public Warehouse? GetByCode(string code) => GetAll().FirstOrDefault(w => w.Code == code);
        public void Insert(Warehouse warehouse) => LastWarehouse = warehouse;
        public void Update(Warehouse warehouse) => LastWarehouse = warehouse;
        public void Delete(string code) { }
        public IReadOnlyList<ReceivingRecord> GetReceivings() => [];
        public void CreateReceiving(ReceivingDraft draft) { LastReceiving = draft; CreateReceivingCalls++; }
    }

    private sealed class FakeSupplierRepository : ISupplierRepository
    {
        public IReadOnlyList<Supplier> GetAll() => [new Supplier("N00001", "Supplier", "", "", "")];
        public Supplier? GetByCode(string code) => code == "N00001" ? GetAll()[0] : null;
        public Supplier Insert(Supplier supplier) => supplier;
        public Supplier Update(Supplier supplier) => supplier;
        public void Delete(string code) { }
    }

    private sealed class FakeProductRepository : IProductManageDAO
    {
        public List<ProductManage> ReadData() => [];
        public List<ProductManage> Search(string keyword) => [];
        public ProductManage? GetByCode(string maSP) => maSP == "SP000001" ? new ProductManage { MaSP = maSP } : null;
        public List<ProductCategory> ReadCategories() => [];
        public bool CheckCategory(string maLoai) => false;
        public void InsertCategory(ProductCategory category) { }
        public void UpdateCategory(ProductCategory category) { }
        public void DeleteCategory(string maLoai) { }
        public List<ProductStock> ReadStockByProduct(string maSP) => [];
        public bool CheckProduct(string maSP) => GetByCode(maSP) is not null;
        public void Insert(ProductManage product) { }
        public void Update(ProductManage product) { }
        public void Delete(string maSP) { }
    }
}
