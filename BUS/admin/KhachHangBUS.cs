using System.Collections;
using System.ComponentModel.DataAnnotations;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class KhachHangBUS : IKhachHangBUS
{
    private readonly IKhachHangDAO repository;
    private ArrayList? customers;

    public KhachHangBUS(IKhachHangDAO repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public void TaiLai() => customers = null;

    public IReadOnlyList<KhachHangDTO> TimKiem(string query = "", BoLocDanhMuc? filter = null)
    {
        var matches = TimKiemDanhMucNangCao.LocKhachHang(filter);
        var term = XuLyChuoiTimKiem.ChuanHoa((query ?? "").Trim());
        return LayDanhSachKhachHang()
            .Where(matches)
            .Where(c => term.Length == 0 || new[] { c.Code, c.Name, c.Phone, c.Email, c.Address }
                .Any(value => XuLyChuoiTimKiem.ChuanHoa(value).Contains(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(c => c.Code, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public KhachHangDTO LayTheoMa(string code) => repository.DocTheoMa((code ?? string.Empty).Trim())
        ?? throw new InvalidOperationException("Không tìm thấy khách hàng.");

    public string GoiYMa()
    {
        var number = 1;
        while (LayDanhSachKhachHang().Any(c => c.Code.Equals($"KH{number:000000}", StringComparison.OrdinalIgnoreCase))) number++;
        if (number > 999999) throw new InvalidOperationException("Không còn mã khách hàng khả dụng.");
        return $"KH{number:000000}";
    }

    public KhachHangDTO Them(DuLieuKhachHang draft)
    {
        var value = KiemTraDuLieu(draft);
        KiemTraTrung(value.Code, value.Email);
        var saved = repository.Them(new KhachHangDTO(value.Code, value.Name, value.Phone, value.Email, value.Address));
        customers!.Add(saved);
        return saved;
    }

    public KhachHangDTO Sua(string code, DuLieuKhachHang draft)
    {
        var existing = LayTheoMa(code);
        _ = LayDanhSachKhachHang();
        var value = KiemTraDuLieu(draft);
        if (value.Code != existing.Code) throw new InvalidOperationException("Mã khách hàng không được sửa.");
        KiemTraTrung(value.Code, value.Email, existing.Code);
        var saved = repository.Sua(existing with
        {
            Name = value.Name, Phone = value.Phone, Email = value.Email, Address = value.Address
        });
        ThayTheTrongDanhSach(existing, saved);
        return saved;
    }

    public void Xoa(string code)
    {
        var existing = LayTheoMa(code);
        _ = LayDanhSachKhachHang();
        repository.Xoa(existing.Code);
        customers!.Remove(LayDanhSachKhachHang().SingleOrDefault(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase)));
    }

    private IEnumerable<KhachHangDTO> LayDanhSachKhachHang()
    {
        customers ??= new ArrayList(repository.DocDanhSach().ToArray());
        return customers.Cast<KhachHangDTO>();
    }

    private void ThayTheTrongDanhSach(KhachHangDTO existing, KhachHangDTO saved)
    {
        var cached = LayDanhSachKhachHang().SingleOrDefault(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase));
        var index = customers!.IndexOf(cached);
        if (index < 0) customers!.Add(saved);
        else customers![index] = saved;
    }

    private void KiemTraTrung(string code, string email, string? exceptCode = null)
    {
        foreach (var value in LayDanhSachKhachHang().Where(value => !value.Code.Equals(exceptCode, StringComparison.OrdinalIgnoreCase)))
        {
            if (value.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Mã khách hàng đã tồn tại.");
            if (email.Length > 0 && value.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Email đã được sử dụng cho khách hàng khác.");
        }
    }

    private static DuLieuKhachHang KiemTraDuLieu(DuLieuKhachHang draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var value = new DuLieuKhachHang
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
