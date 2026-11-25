using System.Collections;
using System.ComponentModel.DataAnnotations;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

// Dùng chung cache, tìm kiếm và kiểm tra dữ liệu; CRUD viết rõ tại từng BUS danh mục.
public abstract class DanhMucBUS<TDto, TForm>
    where TDto : class, IHoSoLienHe
    where TForm : class
{
    private readonly IDanhMucDAO<TDto> repository;
    private readonly string tenDanhMuc, tienToMa, dinhDangMa;
    private readonly int gioiHanMa;
    private ArrayList? danhSach;

    protected DanhMucBUS(IDanhMucDAO<TDto> repository, string tenDanhMuc, string tienToMa, int soChuSo)
    {
        ArgumentNullException.ThrowIfNull(repository);
        if (soChuSo is < 1 or > 8) throw new ArgumentOutOfRangeException(nameof(soChuSo));
        this.repository = repository;
        this.tenDanhMuc = tenDanhMuc;
        this.tienToMa = tienToMa;
        dinhDangMa = new string('0', soChuSo);
        gioiHanMa = (int)Math.Pow(10, soChuSo) - 1;
    }

    protected virtual bool EmailDuyNhat => true;
    protected abstract TForm ChuanHoa(TForm value);
    protected abstract TDto TaoDTO(TForm value);
    protected abstract string LayTruongRieng(TDto value);
    protected abstract Func<TDto, bool> TaoBoLoc(BoLocDanhMuc? filter);

    public void TaiLai() => danhSach = null;

    public IReadOnlyList<TDto> TimKiem(string query = "", BoLocDanhMuc? filter = null)
    {
        // Kiểm tra biểu thức trước khi đọc danh sách, kể cả danh mục đang rỗng.
        var matches = TaoBoLoc(filter);
        var term = XuLyChuoiTimKiem.ChuanHoa(ChuanHoaChuoi(query));
        return LayDanhSach().Where(matches)
            .Where(value => term.Length == 0 || Chua(value.Code, term) || Chua(value.Name, term)
                || Chua(value.Phone, term) || Chua(value.Email, term) || Chua(LayTruongRieng(value), term))
            .OrderBy(value => value.Code, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public TDto LayTheoMa(string code)
    {
        string ma = ChuanHoaChuoi(code);
        foreach (var value in LayDanhSach())
        {
            if (value.Code.Equals(ma, StringComparison.OrdinalIgnoreCase)) return value;
        }
        throw new InvalidOperationException($"Không tìm thấy {tenDanhMuc}.");
    }

    public string GoiYMa()
    {
        // Lập tập mã một lần thay vì quét toàn bộ ArrayList cho mỗi mã thử.
        var maDaDung = new HashSet<string>(LayDanhSach().Select(value => value.Code), StringComparer.OrdinalIgnoreCase);
        for (int number = 1; number <= gioiHanMa; number++)
        {
            string ma = tienToMa + number.ToString(dinhDangMa);
            if (!maDaDung.Contains(ma)) return ma;
        }
        throw new InvalidOperationException($"Không còn mã {tenDanhMuc} khả dụng.");
    }

    protected IDanhMucDAO<TDto> KhoDuLieu => repository;

    protected ArrayList DanhSachDaNap
    {
        get
        {
            _ = LayDanhSach();
            return danhSach!;
        }
    }

    private IEnumerable<TDto> LayDanhSach()
    {
        danhSach ??= new ArrayList(repository.DocDanhSach().ToArray());
        return danhSach.Cast<TDto>();
    }

    protected TDto KiemTraDuLieu(TForm draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var value = ChuanHoa(draft);
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(value, new ValidationContext(value), errors, validateAllProperties: true))
            throw new InvalidOperationException(string.Join(" ", errors.Select(error => error.ErrorMessage)));
        var dto = TaoDTO(value);
        if (dto.Email.Length > 0 && !new EmailAddressAttribute().IsValid(dto.Email))
            throw new InvalidOperationException("Email không đúng định dạng.");
        return dto;
    }

    protected void KiemTraTrung(TDto dto, string? exceptCode = null)
    {
        foreach (var value in LayDanhSach())
        {
            if (value.Code.Equals(exceptCode, StringComparison.OrdinalIgnoreCase)) continue;
            if (value.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Mã {tenDanhMuc} đã tồn tại.");
            if (EmailDuyNhat && dto.Email.Length > 0 && value.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Email đã được sử dụng cho {tenDanhMuc} khác.");
        }
    }

    protected static string ChuanHoaChuoi(string? value) => (value ?? "").Trim();

    private static bool Chua(string value, string term) => XuLyChuoiTimKiem.ChuanHoa(value)
        .Contains(term, StringComparison.OrdinalIgnoreCase);
}
