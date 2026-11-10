using System.ComponentModel.DataAnnotations;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.BUS;

public sealed class EmployeeBUS : IEmployeeBUS
{
    private readonly IEmployeeRepository repository;
    private List<Employee>? employees;

    public EmployeeBUS(IEmployeeRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public void Reload() => employees = null;

    public IReadOnlyList<Employee> Search(string query = "")
    {
        var term = SearchText.Normalize((query ?? "").Trim());
        return GetEmployees()
            .Where(c => term.Length == 0 || new[] { c.Code, c.Name, c.Position, c.Phone, c.Email }
                .Any(value => SearchText.Normalize(value).Contains(term, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(c => c.Code, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public Employee Get(string code) => repository.GetByCode((code ?? string.Empty).Trim())
        ?? throw new InvalidOperationException("Không tìm thấy nhân viên.");

    public string SuggestCode()
    {
        var number = 1;
        while (GetEmployees().Any(c => c.Code.Equals($"NV{number:000000}", StringComparison.OrdinalIgnoreCase))) number++;
        if (number > 999999) throw new InvalidOperationException("Không còn mã nhân viên khả dụng.");
        return $"NV{number:000000}";
    }

    public Employee Create(EmployeeDraft draft)
    {
        var value = Validate(draft);
        EnsureUnique(value.Code);
        var saved = repository.Insert(new Employee(value.Code, value.Name, value.Position, value.Phone, value.Email));
        GetEmployees().Add(saved);
        return saved;
    }

    public Employee Update(string code, EmployeeDraft draft)
    {
        var existing = Get(code);
        var value = Validate(draft);
        if (value.Code != existing.Code) throw new InvalidOperationException("Mã nhân viên không được sửa.");
        EnsureUnique(value.Code, existing.Code);
        var saved = repository.Update(existing with
        {
            Name = value.Name, Position = value.Position, Phone = value.Phone, Email = value.Email
        });
        Replace(existing, saved);
        return saved;
    }

    public void Delete(string code)
    {
        var existing = Get(code);
        repository.Delete(existing.Code);
        GetEmployees().RemoveAll(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase));
    }

    private List<Employee> GetEmployees()
    {
        employees ??= repository.GetAll().ToList();
        return employees;
    }

    private void Replace(Employee existing, Employee saved)
    {
        var index = GetEmployees().FindIndex(value => value.Code.Equals(existing.Code, StringComparison.OrdinalIgnoreCase));
        if (index < 0) throw new InvalidOperationException("Không tìm thấy nhân viên trong danh sách hiện tại.");
        GetEmployees()[index] = saved;
    }

    private void EnsureUnique(string code, string? exceptCode = null)
    {
        if (GetEmployees().Any(c => !c.Code.Equals(exceptCode, StringComparison.OrdinalIgnoreCase)
            && c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Mã nhân viên đã tồn tại.");
    }

    private static EmployeeDraft Validate(EmployeeDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var value = new EmployeeDraft
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
