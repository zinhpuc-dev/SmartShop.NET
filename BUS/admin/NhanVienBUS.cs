using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class NhanVienBUS : DanhMucBUS<NhanVienDTO, DuLieuNhanVien>, INhanVienBUS
{
    public NhanVienBUS(INhanVienDAO repository)
        : base(repository, "nhân viên", "NV", 6) { }

    public NhanVienDTO Them(DuLieuNhanVien draft)
    {
        var dto = KiemTraDuLieu(draft);
        KiemTraTrung(dto);
        var saved = KhoDuLieu.Them(dto);
        DanhSachDaNap.Add(saved);
        return saved;
    }

    public NhanVienDTO Sua(string code, DuLieuNhanVien draft)
    {
        var existing = LayTheoMa(code);
        int index = DanhSachDaNap.IndexOf(existing);
        var dto = KiemTraDuLieu(draft);
        if (dto.Code != existing.Code) throw new InvalidOperationException("Mã nhân viên không được sửa.");
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

    protected override DuLieuNhanVien ChuanHoa(DuLieuNhanVien value) => new()
    {
        Code = ChuanHoaChuoi(value.Code).ToUpperInvariant(),
        Name = ChuanHoaChuoi(value.Name),
        Position = ChuanHoaChuoi(value.Position),
        Phone = ChuanHoaChuoi(value.Phone),
        Email = ChuanHoaChuoi(value.Email)
    };

    protected override NhanVienDTO TaoDTO(DuLieuNhanVien value) => new(value.Code, value.Name, value.Position, value.Phone, value.Email);

    protected override string LayTruongRieng(NhanVienDTO value) => value.Position;

    protected override Func<NhanVienDTO, bool> TaoBoLoc(BoLocDanhMuc? filter) =>
        TimKiemDanhMucNangCao.LocNhanVien(filter);
}
