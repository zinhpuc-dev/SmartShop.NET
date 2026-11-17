using System.Collections;
using System.ComponentModel.DataAnnotations;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class NhanVienBUS : INhanVienBUS
{
    private readonly INhanVienDAO repository;
    private ArrayList? employees;

    public NhanVienBUS(INhanVienDAO repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public void TaiLai() => employees = null;

    public IReadOnlyList<NhanVienDTO> TimKiem(string query = "", BoLocDanhMuc? filter = null)
    {
        var matches = TimKiemDanhMucNangCao.LocNhanVien(filter);
        var term = XuLyChuoiTimKiem.ChuanHoa((query ?? "").Trim());
        return LayDanhSachNhanVien()
            .Where(matches)
            .Where(c => term.Length == 0 || new[] { c.Code, c.Name, c.Position, c.Phone, c.Email }
                .Any(value => XuLyChuoiTimKiem.ChuanHoa(value).Contains(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(c => c.Code, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public NhanVienDTO LayTheoMa(string code)
    {
        var ma = (code ?? string.Empty).Trim();
        foreach (var value in LayDanhSachNhanVien())
        {
            if (value.Code.Equals(ma, StringComparison.OrdinalIgnoreCase)) return value;
        }
        throw new InvalidOperationException("Không tìm thấy nhân viên.");
    }

    public string GoiYMa()
    {
        var number = 1;
        while (LayDanhSachNhanVien().Any(c => c.Code.Equals($"NV{number:000000}", StringComparison.OrdinalIgnoreCase))) number++;
        if (number > 999999) throw new InvalidOperationException("Không còn mã nhân viên khả dụng.");
        return $"NV{number:000000}";
    }

    public NhanVienDTO Them(DuLieuNhanVien draft)
    {
        var value = KiemTraDuLieu(draft);
        KiemTraTrung(value.Code, value.Email);
        var dto = new NhanVienDTO(value.Code, value.Name, value.Position, value.Phone, value.Email);
        var saved = repository.Them(dto);
        employees!.Add(saved);
        return saved;
    }

    public NhanVienDTO Sua(string code, DuLieuNhanVien draft)
    {
        var existing = LayTheoMa(code);
        var index = employees!.IndexOf(existing);
        var value = KiemTraDuLieu(draft);
        if (value.Code != existing.Code) throw new InvalidOperationException("Mã nhân viên không được sửa.");
        KiemTraTrung(value.Code, value.Email, existing.Code);
        var dto = new NhanVienDTO(existing.Code, value.Name, value.Position, value.Phone, value.Email);
        var saved = repository.Sua(dto);
        employees![index] = saved;
        return saved;
    }

    public void Xoa(string code)
    {
        var existing = LayTheoMa(code);
        repository.Xoa(existing.Code);
        employees!.Remove(existing);
    }

    private IEnumerable<NhanVienDTO> LayDanhSachNhanVien()
    {
        employees ??= new ArrayList(repository.DocDanhSach().ToArray());
        return employees.Cast<NhanVienDTO>();
    }

    private void KiemTraTrung(string code, string email, string? exceptCode = null)
    {
        foreach (var value in LayDanhSachNhanVien().Where(value => !value.Code.Equals(exceptCode, StringComparison.OrdinalIgnoreCase)))
        {
            if (value.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Mã nhân viên đã tồn tại.");
            if (email.Length > 0 && value.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Email đã được sử dụng cho nhân viên khác.");
        }
    }

    private static DuLieuNhanVien KiemTraDuLieu(DuLieuNhanVien draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var value = new DuLieuNhanVien
        {
            Code = (draft.Code ?? "").Trim().ToUpperInvariant(), Name = (draft.Name ?? "").Trim(),
            Position = (draft.Position ?? "").Trim(), Phone = (draft.Phone ?? "").Trim(),
            Email = (draft.Email ?? "").Trim()
        };
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(value, new ValidationContext(value), errors, validateAllProperties: true))
            throw new InvalidOperationException(string.Join(" ", errors.Select(e => e.ErrorMessage)));
        if (!string.IsNullOrEmpty(value.Email) && !new EmailAddressAttribute().IsValid(value.Email))
            throw new InvalidOperationException("Email không đúng định dạng.");
        return value;
    }

}
