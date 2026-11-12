using System.ComponentModel.DataAnnotations;

namespace SmartShop.DTO;

public sealed class DuLieuKhuyenMai
{
    [Required, StringLength(8, MinimumLength = 1)] public string Code { get; set; } = string.Empty;
    [Required, StringLength(200, MinimumLength = 1)] public string Name { get; set; } = string.Empty;
    [StringLength(500)] public string Description { get; set; } = string.Empty;
    [Required, StringLength(20)] public string Type { get; set; } = "Khuyến mại";
    [Required, StringLength(20)] public string DiscountForm { get; set; } = "Phần trăm";
    [Range(typeof(decimal), "0.01", "9999999999999999.99")] public decimal DiscountValue { get; set; }
    [Range(typeof(decimal), "0", "9999999999999999.99")] public decimal MinimumOrderValue { get; set; }
    public DateTime StartsAt { get; set; } = DateTime.Today;
    public DateTime EndsAt { get; set; } = DateTime.Today.AddDays(7);
    public bool IsActive { get; set; } = true;
}

public sealed record KhuyenMaiDTO(string Code, string Name, string Description, string Type, string DiscountForm, decimal DiscountValue, decimal MinimumOrderValue, DateTime StartsAt, DateTime EndsAt, bool IsActive, int ProductCount, int VoucherCount);

public sealed class DuLieuMaGiamGia
{
    [Required, StringLength(8, MinimumLength = 1)] public string Code { get; set; } = string.Empty;
    [Required, StringLength(8, MinimumLength = 1)] public string PromotionCode { get; set; } = string.Empty;
    [Required, StringLength(50, MinimumLength = 1)] public string VoucherCode { get; set; } = string.Empty;
    [Range(typeof(decimal), "0", "100")] public decimal PercentOff { get; set; }
    [Range(typeof(decimal), "0", "9999999999999999.99")] public decimal AmountOff { get; set; }
    [Range(typeof(decimal), "0", "9999999999999999.99")] public decimal MinimumOrderValue { get; set; }
    [Range(1, int.MaxValue)] public int Quantity { get; set; } = 1;
    public DateTime StartsAt { get; set; } = DateTime.Today;
    public DateTime EndsAt { get; set; } = DateTime.Today.AddDays(7);
    public bool IsActive { get; set; } = true;
}

public sealed record MaGiamGiaDTO(string Code, string PromotionCode, string PromotionName, string VoucherCode, decimal PercentOff, decimal AmountOff, decimal MinimumOrderValue, int Quantity, int Used, DateTime StartsAt, DateTime EndsAt, bool IsActive);
