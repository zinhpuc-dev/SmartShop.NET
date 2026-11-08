using Microsoft.Extensions.DependencyInjection;
using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

internal static class EmployeeChecks
{
    public static void Run()
    {
        var repository = new ObservedRepository();
        IEmployeeService service = new EmployeeService(repository);
        Check(service.Search().Count == 3, "Load demo employees into BUS");
        Check(service.Search("nguyen minh").Single().Code == "NV000001", "Find Vietnamese names without accents");
        Check(service.Search("0910000002").Single().Code == "NV000002", "Find employees by phone");
        service.Search("NV"); service.Search("example.com");
        Check(repository.Reads == 1, "Search stays on the BUS ArrayList without re-reading DAO");

        var saved = service.Create(Draft("  nv000900  "));
        Check(saved.Code == "NV000900" && saved.Name == "Nhân viên Demo", "Create normalizes a CHAR code");
        Check(saved.Email == "", "Email is optional");
        Check(saved.Position == "Nhân viên", "Employee DTO preserves ChucVu");
        Reject(() => service.Create(Draft(" NV000900 ")), "Reject duplicate code case-insensitively");
        Check(service.Search().Count == 4, "Duplicate rejection does not change the list");

        var invalid = Draft("NV000901"); invalid.Phone = new string('1', 21);
        Reject(() => service.Create(invalid), "Reject phone longer than schema column");
        invalid = Draft("NV000901"); invalid.Name = "   ";
        Reject(() => service.Create(invalid), "Reject blank name in BUS");
        invalid = Draft("NV000901"); invalid.Email = "bad-email";
        Reject(() => service.Create(invalid), "Reject invalid email in BUS");
        invalid = Draft("NV000901"); invalid.Position = "";
        Reject(() => service.Create(invalid), "Reject missing ChucVu");
        invalid = Draft(" ");
        Reject(() => service.Create(invalid), "Reject missing employee code");
        invalid = Draft("NV0000001");
        Reject(() => service.Create(invalid), "Reject employee code longer than CHAR(8)");

        var edit = EmployeeDraft.From(saved);
        edit.Name = "Tên mới";
        Check(service.Get(saved.Code).Name == saved.Name, "Editing a draft does not mutate stored data before save");
        var updated = service.Update(saved.Code, edit);
        Check(updated.Name == "Tên mới" && updated.Code == saved.Code, "Edit preserves the immutable CHAR code");
        edit.Code = "NV999";
        Reject(() => service.Update(saved.Code, edit), "Reject code tampering even if UI readonly is bypassed");

        repository.FailWrites = true;
        var before = service.Get(saved.Code);
        edit = EmployeeDraft.From(before); edit.Name = "Không lưu được";
        Reject(() => service.Update(saved.Code, edit), "Propagate an update failure");
        Reject(() => service.Create(Draft("NV000902")), "Propagate an insert failure");
        Reject(() => service.Delete(saved.Code), "Propagate a delete failure");
        Check(service.Get(saved.Code) == before && service.Search().Count == 4,
            "DAO failure leaves the BUS cache unchanged");
        repository.FailWrites = false;
        service.Delete(saved.Code);
        Check(!service.Search("NV000900").Any(), "Delete removes employee from the BUS list");
        Reject(() => service.Get(saved.Code), "Deleted employee is no longer found");
        Reject(() => repository.Insert(repository.GetByCode("NV000001")!), "DAO also protects unique codes");

        var services = new ServiceCollection();
        services.AddScoped<IEmployeeRepository, DemoEmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        using var scopeA = provider.CreateScope(); using var scopeB = provider.CreateScope();
        var a = scopeA.ServiceProvider.GetRequiredService<IEmployeeService>();
        var b = scopeB.ServiceProvider.GetRequiredService<IEmployeeService>();
        a.Create(Draft("NV950"));
        Check(a.Search("NV950").Count == 1 && b.Search("NV950").Count == 0, "Employee scopes do not leak data between sessions");
        Check(ReferenceEquals(a, scopeA.ServiceProvider.GetRequiredService<IEmployeeService>()),
            "Admin and employee picker share the same scoped employee service");
    }

    private static EmployeeDraft Draft(string code) => new()
    {
        Code = code, Name = "  Nhân viên Demo  ", Position = "Nhân viên", Phone = "0900000099", Email = ""
    };
    private static void Check(bool value, string message)
    {
        if (!value) throw new Exception(message);
        Console.WriteLine("PASS EMPLOYEE " + message);
    }
    private static void Reject(Action action, string message)
    {
        try { action(); }
        catch (InvalidOperationException) { Console.WriteLine("PASS EMPLOYEE " + message); return; }
        throw new Exception(message);
    }

    private sealed class ObservedRepository : IEmployeeRepository
    {
        private readonly DemoEmployeeRepository inner = new();
        public int Reads { get; private set; }
        public bool FailWrites { get; set; }
        public IReadOnlyList<Employee> GetAll() { Reads++; return inner.GetAll(); }
        public Employee? GetByCode(string code) => inner.GetByCode(code);
        public Employee Insert(Employee value) { VerifyWrite(); return inner.Insert(value); }
        public Employee Update(Employee value) { VerifyWrite(); return inner.Update(value); }
        public void Delete(string code) { VerifyWrite(); inner.Delete(code); }
        private void VerifyWrite()
        {
            if (FailWrites) throw new InvalidOperationException("Simulated employee storage failure.");
        }
    }
}
