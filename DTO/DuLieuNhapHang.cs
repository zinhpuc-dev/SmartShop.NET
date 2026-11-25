using System.ComponentModel.DataAnnotations;

namespace SmartShop.DTO;

public sealed class DuLieuPhieuNhap
{
    [Required, StringLength(8, MinimumLength = 1)]
    public string Code { get; set; } = string.Empty;
    [Required, StringLength(6, MinimumLength = 1)]
    public string SupplierCode { get; set; } = string.Empty;
    [Required, StringLength(6, MinimumLength = 1)]
    public string WarehouseCode { get; set; } = string.Empty;
    public List<DuLieuDongNhap> Lines { get; set; } = [];
}

public sealed class DuLieuDongNhap
{
    [Required, StringLength(8, MinimumLength = 1)]
    public string ProductCode { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal UnitCost { get; set; }
}

public sealed record PhieuNhapDTO(string Code, string SupplierCode, string SupplierName, string WarehouseCode, string WarehouseName, DateTime ReceivedAt, decimal Total, int LineCount);
