using System.Collections;
using System.ComponentModel.DataAnnotations;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class NhaCungCapBUS : INhaCungCapBUS
{
    private readonly INhaCungCapDAO repository;
    private ArrayList? suppliers;

    public NhaCungCapBUS(INhaCungCapDAO repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public void TaiLai() => suppliers = null;

    public IReadOnlyList<NhaCungCapDTO> TimKiem(string query = "", BoLocDanhMuc? filter = null)
    {
        var matches = TimKiemDanhMucNangCao.LocNhaCungCap(filter);
        var term = XuLyChuoiTimKiem.ChuanHoa((query ?? "").Trim());
        // Tìm trên danh sách BUS đã nạp, không truy vấn lại CSDL khi gõ.
        return LayDanhSachNhaCungCap()
            .Where(matches)
            .Where(c => term.Length == 0 || new[] { c.Code, c.Name, c.Phone, c.Email, c.Address }
                    .Any(value => XuLyChuoiTimKiem.ChuanHoa(value).Contains(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(c => c.Code, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public NhaCungCapDTO LayTheoMa(string code) => repository.DocTheoMa((code ?? string.Empty).Trim())
        ?? throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");

    public string GoiYMa()
    {
        var number = 1;
        while (LayDanhSachNhaCungCap().Any(c => c.Code.Equals($"N{number:00000}", StringComparison.OrdinalIgnoreCase))) number++;
        if (number > 99999) throw new InvalidOperationException("Không còn mã nhà cung cấp khả dụng.");
        return $"N{number:00000}";
    }

    public NhaCungCapDTO Them(DuLieuNhaCungCap draft)
    {
        var value = KiemTraDuLieu(draft);
        KiemTraTrung(value.Code);
        var saved = repository.Them(new NhaCungCapDTO(value.Code, value.Name, value.Phone, value.Email, value.Address));
        suppliers!.Add(saved);
        return saved;
    }

    public NhaCungCapDTO Sua(string code, DuLieuNhaCungCap draft)
    {
        var existing = LayTheoMa(code);
        _ = LayDanhSachNhaCungCap();
        var value = KiemTraDuLieu(draft);
        if (value.Code != existing.Code) throw new InvalidOperationException("Mã nhà cung cấp không được sửa.");
        KiemTraTrung(value.Code, existing.Code);
        var saved = repository.Sua(existing with
        {
            Name = value.Name, Phone = value.Phone, Email = value.Email, Address = value.Address
        });
        var cached = LayDanhSachNhaCungCap().SingleOrDefault(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase));
        var index = suppliers!.IndexOf(cached);
        if (index < 0) suppliers!.Add(saved);
        else suppliers![index] = saved;
        return saved;
    }

    public void Xoa(string code)
    {
        var existing = LayTheoMa(code);
        _ = LayDanhSachNhaCungCap();
        repository.Xoa(existing.Code);
        suppliers!.Remove(LayDanhSachNhaCungCap().SingleOrDefault(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase)));
    }

    private IEnumerable<NhaCungCapDTO> LayDanhSachNhaCungCap()
    {
        suppliers ??= new ArrayList(repository.DocDanhSach().ToArray());
        return suppliers.Cast<NhaCungCapDTO>();
    }

    private void KiemTraTrung(string code, string? exceptCode = null)
    {
        if (LayDanhSachNhaCungCap().Any(c => !c.Code.Equals(exceptCode, StringComparison.OrdinalIgnoreCase)
            && c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Mã nhà cung cấp đã tồn tại.");
    }

    private static DuLieuNhaCungCap KiemTraDuLieu(DuLieuNhaCungCap draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var value = new DuLieuNhaCungCap
        {
            Code = (draft.Code ?? "").Trim().ToUpperInvariant(), Name = (draft.Name ?? "").Trim(),
            Phone = (draft.Phone ?? "").Trim(), Email = (draft.Email ?? "").Trim(),
            Address = (draft.Address ?? "").Trim()
        };
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(value, new ValidationContext(value), errors, validateAllProperties: true))
            throw new InvalidOperationException(string.Join(" ", errors.Select(e => e.ErrorMessage)));
        if (!string.IsNullOrEmpty(value.Email) && !new EmailAddressAttribute().IsValid(value.Email))
            throw new InvalidOperationException("Email không đúng định dạng.");
        return value;
    }

}
