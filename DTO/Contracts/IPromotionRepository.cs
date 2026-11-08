namespace SmartShop.DTO.Contracts;

public interface IPromotionRepository
{
    IReadOnlyList<ManagedPromotion> GetPromotions();
    void InsertPromotion(PromotionDraft draft);
    void UpdatePromotion(PromotionDraft draft);
    IReadOnlyList<ManagedVoucher> GetVouchers();
    ManagedVoucher? GetVoucher(string code);
    void InsertVoucher(VoucherDraft draft);
    void UpdateVoucher(VoucherDraft draft);
}
