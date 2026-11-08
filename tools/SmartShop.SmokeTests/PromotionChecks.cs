using SmartShop.BUS;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

internal static class PromotionChecks
{
    public static void Run()
    {
        var repository = new FakePromotionRepository();
        var service = new PromotionService(repository);
        var promotion = new PromotionDraft { Code = " km000001 ", Name = "Mùa hè", Type = "Chương trình", DiscountForm = "Phần trăm", DiscountValue = 15, StartsAt = DateTime.Today, EndsAt = DateTime.Today.AddDays(1) };
        service.SavePromotion(promotion, false);
        Check(repository.LastPromotion?.Code == "KM000001", "PROMOTION normalize code");
        Reject(() => service.SavePromotion(new PromotionDraft { Code = "KM000002", Name = "Invalid", DiscountForm = "Phần trăm", DiscountValue = 101, StartsAt = DateTime.Today, EndsAt = DateTime.Today.AddDays(1) }, false), "PROMOTION reject percent greater than 100");
        var voucher = new VoucherDraft { Code = "VC000001", PromotionCode = "KM000001", VoucherCode = "SUMMER", PercentOff = 10, Quantity = 10, StartsAt = DateTime.Today, EndsAt = DateTime.Today.AddDays(1) };
        service.SaveVoucher(voucher, false);
        Check(repository.LastVoucher?.VoucherCode == "SUMMER", "VOUCHER validate and save exclusive percent discount");
        Reject(() => service.SaveVoucher(new VoucherDraft { Code = "VC000002", PromotionCode = "KM000001", VoucherCode = "BOTH", PercentOff = 10, AmountOff = 100, Quantity = 2, StartsAt = DateTime.Today, EndsAt = DateTime.Today.AddDays(1) }, false), "VOUCHER reject mixed percent and amount discounts");
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

    private sealed class FakePromotionRepository : IPromotionRepository
    {
        public PromotionDraft? LastPromotion { get; private set; }
        public VoucherDraft? LastVoucher { get; private set; }
        public IReadOnlyList<ManagedPromotion> GetPromotions() => [];
        public void InsertPromotion(PromotionDraft draft) => LastPromotion = draft;
        public void UpdatePromotion(PromotionDraft draft) => LastPromotion = draft;
        public IReadOnlyList<ManagedVoucher> GetVouchers() => [];
        public ManagedVoucher? GetVoucher(string code) => null;
        public void InsertVoucher(VoucherDraft draft) => LastVoucher = draft;
        public void UpdateVoucher(VoucherDraft draft) => LastVoucher = draft;
    }
}
