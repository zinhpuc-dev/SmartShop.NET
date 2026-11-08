using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class WarehouseService(IWarehouseRepository dao, ISupplierRepository suppliers, IProductManage products)
{
    public IReadOnlyList<Warehouse> GetAll() => dao.GetAll();
    public IReadOnlyList<ReceivingRecord> GetReceivings() => dao.GetReceivings();
    public IReadOnlyList<ReceivingDetail> GetReceivingDetails(string code) => dao.GetReceivingDetails(Normalize(code, 8, "Mã phiếu nhập"));
    public IReadOnlyList<WarehouseStock> GetStock() => dao.GetStock();

    public void Save(Warehouse warehouse, bool editing)
    {
        ArgumentNullException.ThrowIfNull(warehouse);
        warehouse.Code = Normalize(warehouse.Code, 6, "Mã kho");
        warehouse.Name = warehouse.Name.Trim();
        if (warehouse.Name.Length is 0 or > 150) throw new ArgumentException("Tên kho phải từ 1 đến 150 ký tự.");
        if (editing) dao.Update(warehouse); else dao.Insert(warehouse);
    }

    public string SuggestCode()
    {
        HashSet<string> existing = GetAll().Select(item => item.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        for (int i = 1; i <= 99999; i++)
        {
            string code = $"K{i:00000}";
            if (!existing.Contains(code)) return code;
        }
        throw new InvalidOperationException("Không còn mã kho khả dụng.");
    }

    public void Delete(string code) => dao.Delete(Normalize(code, 6, "Mã kho"));

    public void CreateReceiving(ReceivingDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        draft.Code = Normalize(draft.Code, 8, "Mã phiếu nhập");
        draft.SupplierCode = Normalize(draft.SupplierCode, 6, "Mã nhà cung cấp");
        draft.WarehouseCode = Normalize(draft.WarehouseCode, 6, "Mã kho");
        if (dao.GetByCode(draft.WarehouseCode) is null) throw new InvalidOperationException("Không tìm thấy kho.");
        if (draft.Lines.Count == 0) throw new ArgumentException("Phiếu nhập cần ít nhất một sản phẩm.");
        if (draft.Lines.Select(x => x.ProductCode.Trim().ToUpperInvariant()).Distinct(StringComparer.Ordinal).Count() != draft.Lines.Count)
            throw new ArgumentException("Mỗi sản phẩm chỉ được xuất hiện một lần trong phiếu nhập.");
        foreach (ReceivingLineDraft line in draft.Lines)
        {
            line.ProductCode = Normalize(line.ProductCode, 8, "Mã sản phẩm");
            if (line.Quantity <= 0) throw new ArgumentException("Số lượng nhập phải lớn hơn 0.");
            if (line.UnitCost <= 0 || line.UnitCost > 9999999999999999.99m) throw new ArgumentException("Đơn giá nhập không hợp lệ.");
            if (products.GetByCode(line.ProductCode) is null) throw new InvalidOperationException($"Không tìm thấy sản phẩm {line.ProductCode}.");
            decimal total = checked(line.UnitCost * line.Quantity);
            if (total > 9999999999999999.99m) throw new ArgumentException("Thành tiền dòng nhập vượt giới hạn lưu trữ.");
        }
        if (suppliers.GetByCode(draft.SupplierCode) is null) throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");
        if (dao.GetByCode(draft.WarehouseCode) is null) throw new InvalidOperationException("Không tìm thấy kho.");
        dao.CreateReceiving(draft);
    }

    private static string Normalize(string value, int maxLength, string label)
    {
        string result = value?.Trim().ToUpperInvariant() ?? string.Empty;
        if (result.Length is 0 || result.Length > maxLength) throw new ArgumentException($"{label} phải từ 1 đến {maxLength} ký tự.");
        return result;
    }
}
