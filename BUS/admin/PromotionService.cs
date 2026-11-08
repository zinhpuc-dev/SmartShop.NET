using System.ComponentModel.DataAnnotations;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class PromotionService(IPromotionRepository repository)
{
    public IReadOnlyList<ManagedPromotion> GetPromotions() => repository.GetPromotions();
    public IReadOnlyList<ManagedVoucher> GetVouchers() => repository.GetVouchers();

    public void SavePromotion(PromotionDraft draft, bool editing)
    {
        Validate(draft);
        if (draft.DiscountForm.Equals("Phần trăm", StringComparison.OrdinalIgnoreCase) && draft.DiscountValue > 100)
            throw new ArgumentException("Mức giảm theo phần trăm phải từ 0 đến 100.");
        if (editing) repository.UpdatePromotion(draft); else repository.InsertPromotion(draft);
    }

    public void SaveVoucher(VoucherDraft draft, bool editing)
    {
        Validate(draft);
        if (draft.PercentOff > 0 && draft.AmountOff > 0) throw new ArgumentException("Voucher chỉ dùng giảm phần trăm hoặc số tiền, không dùng đồng thời.");
        if (draft.PercentOff == 0 && draft.AmountOff == 0) throw new ArgumentException("Voucher cần mức giảm phần trăm hoặc số tiền lớn hơn 0.");
        if (draft.PercentOff > 0 && draft.PercentOff > 100) throw new ArgumentException("Phần trăm giảm không được vượt quá 100.");
        ManagedPromotion program = repository.GetPromotions().SingleOrDefault(p => p.Code == draft.PromotionCode)
            ?? throw new ArgumentException("Không tìm thấy chương trình khuyến mại.");
        if (draft.StartsAt < program.StartsAt || draft.EndsAt > program.EndsAt)
            throw new ArgumentException("Thời gian voucher phải nằm trong thời gian chương trình.");
        if (draft.IsActive && !program.IsActive)
            throw new ArgumentException("Cần bật chương trình trước khi kích hoạt voucher.");
        if (editing)
        {
            ManagedVoucher existing = repository.GetVoucher(draft.Code) ?? throw new InvalidOperationException("Không tìm thấy voucher.");
            if (existing.Used > 0 && (draft.PromotionCode != existing.PromotionCode || draft.VoucherCode != existing.VoucherCode
                || draft.PercentOff != existing.PercentOff || draft.AmountOff != existing.AmountOff))
                throw new ArgumentException("Voucher đã sử dụng không được đổi chương trình, mã áp dụng hoặc mức giảm.");
            if (draft.Quantity < existing.Used) throw new ArgumentException("Số lượng voucher không thể nhỏ hơn số lượt đã sử dụng.");
            repository.UpdateVoucher(draft);
        }
        else
            repository.InsertVoucher(draft);
    }

    private static void Validate(object model)
    {
        switch (model)
        {
            case PromotionDraft promotion:
                promotion.Code = (promotion.Code ?? string.Empty).Trim().ToUpperInvariant();
                promotion.Name = (promotion.Name ?? string.Empty).Trim();
                promotion.Description = (promotion.Description ?? string.Empty).Trim();
                promotion.Type = (promotion.Type ?? string.Empty).Trim();
                promotion.DiscountForm = (promotion.DiscountForm ?? string.Empty).Trim();
                break;
            case VoucherDraft voucher:
                voucher.Code = (voucher.Code ?? string.Empty).Trim().ToUpperInvariant();
                voucher.PromotionCode = (voucher.PromotionCode ?? string.Empty).Trim().ToUpperInvariant();
                voucher.VoucherCode = (voucher.VoucherCode ?? string.Empty).Trim().ToUpperInvariant();
                break;
        }
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(model, new ValidationContext(model), errors, true))
            throw new ArgumentException(string.Join(" ", errors.Select(error => error.ErrorMessage)));
        switch (model)
        {
            case PromotionDraft promotion:
                promotion.Code = Normalize(promotion.Code, 8, "Mã khuyến mại");
                if (promotion.StartsAt >= promotion.EndsAt) throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc.");
                if (promotion.MinimumOrderValue < 0 || promotion.DiscountValue <= 0) throw new ArgumentException("Giá trị khuyến mại không hợp lệ.");
                if (promotion.DiscountForm is not ("Phần trăm" or "Số tiền")) throw new ArgumentException("Hình thức giảm không hợp lệ.");
                break;
            case VoucherDraft voucher:
                voucher.Code = Normalize(voucher.Code, 8, "Mã voucher");
                voucher.PromotionCode = Normalize(voucher.PromotionCode, 8, "Mã khuyến mại");
                if (voucher.StartsAt >= voucher.EndsAt) throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc.");
                if (voucher.Quantity < 1 || voucher.MinimumOrderValue < 0 || voucher.AmountOff < 0) throw new ArgumentException("Thông tin voucher không hợp lệ.");
                break;
        }
    }

    private static string Normalize(string value, int maxLength, string label)
    {
        string result = (value ?? string.Empty).Trim().ToUpperInvariant();
        if (result.Length is 0 || result.Length > maxLength) throw new ArgumentException($"{label} phải từ 1 đến {maxLength} ký tự.");
        return result;
    }
}
