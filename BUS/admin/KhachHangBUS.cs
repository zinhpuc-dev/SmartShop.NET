using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class KhachHangBUS : DanhMucBUS<KhachHangDTO, DuLieuKhachHang>, IKhachHangBUS
{
    public KhachHangBUS(IKhachHangDAO repository)
        : base(repository, "khách hàng", "KH", 6) { }

    public KhachHangDTO Them(DuLieuKhachHang draft)
    {
        var dto = KiemTraDuLieu(draft);
        KiemTraTrung(dto);
        var saved = KhoDuLieu.Them(dto);
        DanhSachDaNap.Add(saved);
        return saved;
    }

    public KhachHangDTO Sua(string code, DuLieuKhachHang draft)
    {
        var existing = LayTheoMa(code);
        int index = DanhSachDaNap.IndexOf(existing);
        var dto = KiemTraDuLieu(draft);
        if (dto.Code != existing.Code) throw new InvalidOperationException("Mã khách hàng không được sửa.");
        KiemTraTrung(dto, existing.Code);
        var saved = KhoDuLieu.Sua(dto);
        DanhSachDaNap[index] = saved;
        return saved;
    }

    public void Xoa(string code)
    {
        var existing = LayTheoMa(code);
        KhoDuLieu.Xoa(existing.Code);
        DanhSachDaNap.Remove(existing);
    }

    protected override DuLieuKhachHang ChuanHoa(DuLieuKhachHang value) => new()
    {
        Code = ChuanHoaChuoi(value.Code).ToUpperInvariant(),
        Name = ChuanHoaChuoi(value.Name),
        Phone = ChuanHoaChuoi(value.Phone),
        Email = ChuanHoaChuoi(value.Email),
        Address = ChuanHoaChuoi(value.Address)
    };

    protected override KhachHangDTO TaoDTO(DuLieuKhachHang value) => new(value.Code, value.Name, value.Phone, value.Email, value.Address);

    protected override string LayTruongRieng(KhachHangDTO value) => value.Address;

    protected override Func<KhachHangDTO, bool> TaoBoLoc(BoLocDanhMuc? filter) =>
        TimKiemDanhMucNangCao.LocKhachHang(filter);
}
