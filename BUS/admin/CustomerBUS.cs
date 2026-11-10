using System.Collections;
using System.ComponentModel.DataAnnotations;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class CustomerBUS : ICustomerBUS
{
    private readonly ICustomerRepository repository;
    private ArrayList? customers;

    public CustomerBUS(ICustomerRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public void Reload() => customers = null;

    public IReadOnlyList<Customer> Search(string query = "", CatalogFilter? filter = null)
    {
        var matches = AdvancedCatalogSearch.Customers(filter);
        var term = SearchText.Normalize((query ?? "").Trim());
        return GetCustomers()
            .Where(matches)
            .Where(c => term.Length == 0 || new[] { c.Code, c.Name, c.Phone, c.Email, c.Address }
                .Any(value => SearchText.Normalize(value).Contains(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(c => c.Code, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public Customer Get(string code) => repository.GetByCode((code ?? string.Empty).Trim())
        ?? throw new InvalidOperationException("Không tìm thấy khách hàng.");

    public string SuggestCode()
    {
        var number = 1;
        while (GetCustomers().Any(c => c.Code.Equals($"KH{number:000000}", StringComparison.OrdinalIgnoreCase))) number++;
        if (number > 999999) throw new InvalidOperationException("Không còn mã khách hàng khả dụng.");
        return $"KH{number:000000}";
    }

    public Customer Create(CustomerDraft draft)
    {
        var value = Validate(draft);
        EnsureUnique(value.Code, value.Email);
        var saved = repository.Insert(new Customer(value.Code, value.Name, value.Phone, value.Email, value.Address));
        customers!.Add(saved);
        return saved;
    }

    public Customer Update(string code, CustomerDraft draft)
    {
        var existing = Get(code);
        _ = GetCustomers();
        var value = Validate(draft);
        if (value.Code != existing.Code) throw new InvalidOperationException("Mã khách hàng không được sửa.");
        EnsureUnique(value.Code, value.Email, existing.Code);
        var saved = repository.Update(existing with
        {
            Name = value.Name, Phone = value.Phone, Email = value.Email, Address = value.Address
        });
        Replace(existing, saved);
        return saved;
    }

    public void Delete(string code)
    {
        var existing = Get(code);
        _ = GetCustomers();
        repository.Delete(existing.Code);
        customers!.Remove(GetCustomers().SingleOrDefault(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase)));
    }

    private IEnumerable<Customer> GetCustomers()
    {
        customers ??= new ArrayList(repository.GetAll().ToArray());
        return customers.Cast<Customer>();
    }

    private void Replace(Customer existing, Customer saved)
    {
        var cached = GetCustomers().SingleOrDefault(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase));
        var index = customers!.IndexOf(cached);
        if (index < 0) customers!.Add(saved);
        else customers![index] = saved;
    }

    private void EnsureUnique(string code, string email, string? exceptCode = null)
    {
        foreach (var value in GetCustomers().Where(value => !value.Code.Equals(exceptCode, StringComparison.OrdinalIgnoreCase)))
        {
            if (value.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Mã khách hàng đã tồn tại.");
            if (email.Length > 0 && value.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Email đã được sử dụng cho khách hàng khác.");
        }
    }

    private static CustomerDraft Validate(CustomerDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var value = new CustomerDraft
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
