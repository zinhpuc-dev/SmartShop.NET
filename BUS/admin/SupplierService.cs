using System.ComponentModel.DataAnnotations;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class SupplierService : ISupplierService
{
    private readonly ISupplierRepository repository;
    private List<Supplier>? suppliers;

    public SupplierService(ISupplierRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public IReadOnlyList<Supplier> Search(string query = "")
    {
        var term = SearchText.Normalize((query ?? "").Trim());
        // Search the BUS cache, never re-query storage while typing.
        return GetSuppliers()
            .Where(c => term.Length == 0 || new[] { c.Code, c.Name, c.Phone, c.Email, c.Address }
                    .Any(value => SearchText.Normalize(value).Contains(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(c => c.Code, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public Supplier Get(string code) => repository.GetByCode((code ?? string.Empty).Trim())
        ?? throw new InvalidOperationException("Không tìm thấy nhà cung cấp.");

    public string SuggestCode()
    {
        var number = 1;
        while (GetSuppliers().Any(c => c.Code.Equals($"N{number:00000}", StringComparison.OrdinalIgnoreCase))) number++;
        if (number > 99999) throw new InvalidOperationException("Không còn mã nhà cung cấp khả dụng.");
        return $"N{number:00000}";
    }

    public Supplier Create(SupplierDraft draft)
    {
        var value = Validate(draft);
        EnsureUnique(value.Code);
        var saved = repository.Insert(new Supplier(value.Code, value.Name, value.Phone, value.Email, value.Address));
        GetSuppliers().Add(saved);
        return saved;
    }

    public Supplier Update(string code, SupplierDraft draft)
    {
        var existing = Get(code);
        var value = Validate(draft);
        if (value.Code != existing.Code) throw new InvalidOperationException("Mã nhà cung cấp không được sửa.");
        EnsureUnique(value.Code, existing.Code);
        var saved = repository.Update(existing with
        {
            Name = value.Name, Phone = value.Phone, Email = value.Email, Address = value.Address
        });
        var index = GetSuppliers().IndexOf(existing);
        GetSuppliers()[index] = saved;
        return saved;
    }

    public void Delete(string code)
    {
        var existing = Get(code);
        repository.Delete(existing.Code);
        GetSuppliers().RemoveAll(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase));
    }

    private List<Supplier> GetSuppliers()
    {
        suppliers ??= repository.GetAll().ToList();
        return suppliers;
    }

    private void EnsureUnique(string code, string? exceptCode = null)
    {
        if (GetSuppliers().Any(c => !c.Code.Equals(exceptCode, StringComparison.OrdinalIgnoreCase)
            && c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Mã nhà cung cấp đã tồn tại.");
    }

    private static SupplierDraft Validate(SupplierDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var value = new SupplierDraft
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
