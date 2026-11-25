using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class NhaCungCapBUS : DanhMucBUS<NhaCungCapDTO, DuLieuNhaCungCap>, INhaCungCapBUS
{
    public NhaCungCapBUS(INhaCungCapDAO repository)
        : base(repository, "nhà cung cấp", "N", 5) { }

    public NhaCungCapDTO Them(DuLieuNhaCungCap draft)
    {
        var dto = KiemTraDuLieu(draft);
        KiemTraTrung(dto);
        var saved = KhoDuLieu.Them(dto);
        DanhSachDaNap.Add(saved);
        return saved;
    }

    public NhaCungCapDTO Sua(string code, DuLieuNhaCungCap draft)
    {
        var existing = LayTheoMa(code);
        int index = DanhSachDaNap.IndexOf(existing);
        var dto = KiemTraDuLieu(draft);
        if (dto.Code != existing.Code) throw new InvalidOperationException("Mã nhà cung cấp không được sửa.");
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

    protected override bool EmailDuyNhat => false;

    protected override DuLieuNhaCungCap ChuanHoa(DuLieuNhaCungCap value) => new()
    {
        Code = ChuanHoaChuoi(value.Code).ToUpperInvariant(),
        Name = ChuanHoaChuoi(value.Name),
        Phone = ChuanHoaChuoi(value.Phone),
        Email = ChuanHoaChuoi(value.Email),
        Address = ChuanHoaChuoi(value.Address)
    };

    protected override NhaCungCapDTO TaoDTO(DuLieuNhaCungCap value) => new(value.Code, value.Name, value.Phone, value.Email, value.Address);

    protected override string LayTruongRieng(NhaCungCapDTO value) => value.Address;

    protected override Func<NhaCungCapDTO, bool> TaoBoLoc(BoLocDanhMuc? filter) =>
        TimKiemDanhMucNangCao.LocNhaCungCap(filter);
}
