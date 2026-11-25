using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class KhoBUS(IKhoDAO dao, INhaCungCapDAO suppliers, ISanPhamDAO products)
{
    public IReadOnlyList<KhoDTO> DocDanhSach() => dao.DocDanhSach();
    public IReadOnlyList<PhieuNhapDTO> DocPhieuNhap() => dao.DocPhieuNhap();
    public IReadOnlyList<ChiTietPhieuNhapDTO> DocChiTietPhieuNhap(string code) => dao.DocChiTietPhieuNhap(ChuanHoa(code, 8, "Mã phiếu nhập"));
    public IReadOnlyList<TonKhoDTO> DocTonKho() => dao.DocTonKho();

    public void Luu(KhoDTO warehouse, bool editing)
    {
        ArgumentNullException.ThrowIfNull(warehouse);
        warehouse.Code = ChuanHoa(warehouse.Code, 6, "Mã kho");
        warehouse.Name = warehouse.Name.Trim();
        if (warehouse.Name.Length is 0 or > 150) throw new ArgumentException("Tên kho phải từ 1 đến 150 ký tự.");
        if (editing) dao.Sua(warehouse); else dao.Them(warehouse);
    }

    public string GoiYMa()
    {
        HashSet<string> existing = DocDanhSach().Select(item => item.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        for (int i = 1; i <= 99999; i++)
        {
            string code = $"K{i:00000}";
            if (!existing.Contains(code)) return code;
        }
        throw new InvalidOperationException("Không còn mã kho khả dụng.");
    }

    public void Xoa(string code) => dao.Xoa(ChuanHoa(code, 6, "Mã kho"));

    public void LapPhieuNhap(DuLieuPhieuNhap draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        draft.Code = ChuanHoa(draft.Code, 8, "Mã phiếu nhập");
        draft.SupplierCode = ChuanHoa(draft.SupplierCode, 6, "Mã nhà cung cấp");
        draft.WarehouseCode = ChuanHoa(draft.WarehouseCode, 6, "Mã kho");
        if (dao.DocTheoMa(draft.WarehouseCode) is null) throw new InvalidOperationException("Không tìm thấy kho.");
        if (draft.Lines.Count == 0) throw new ArgumentException("Phiếu nhập cần ít nhất một sản phẩm.");
        if (draft.Lines.Select(x => x.ProductCode.Trim().ToUpperInvariant()).Distinct(StringComparer.Ordinal).Count() != draft.Lines.Count)
            throw new ArgumentException("Mỗi sản phẩm chỉ được xuất hiện một lần trong phiếu nhập.");
        foreach (DuLieuDongNhap line in draft.Lines)
        {
            line.ProductCode = ChuanHoa(line.ProductCode, 8, "Mã sản phẩm");
            if (line.Quantity <= 0) throw new ArgumentException("Số lượng nhập phải lớn hơn 0.");
            if (line.UnitCost <= 0 || line.UnitCost > 9999999999999999.99m) throw new ArgumentException("Đơn giá nhập không hợp lệ.");
            if (products.DocTheoMa(line.ProductCode) is null) throw new InvalidOperationException($"Không tìm thấy sản phẩm {line.ProductCode}.");
            decimal total = checked(line.UnitCost * line.Quantity);
            if (total > 9999999999999999.99m) throw new ArgumentException("Thành tiền dòng nhập vượt giới hạn lưu trữ.");
        }
        if (suppliers.DocTheoMa(draft.SupplierCode) is null) throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");
        if (dao.DocTheoMa(draft.WarehouseCode) is null) throw new InvalidOperationException("Không tìm thấy kho.");
        dao.LapPhieuNhap(draft);
    }

    private static string ChuanHoa(string value, int maxLength, string label)
    {
        string result = value?.Trim().ToUpperInvariant() ?? string.Empty;
        if (result.Length is 0 || result.Length > maxLength) throw new ArgumentException($"{label} phải từ 1 đến {maxLength} ký tự.");
        return result;
    }
}
