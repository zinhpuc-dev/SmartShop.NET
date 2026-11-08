using Microsoft.Extensions.DependencyInjection;
using SmartShop.BUS;
using SmartShop.DAO;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

internal static class CustomerChecks
{
    public static void Run()
    {
        var repository = new ObservedRepository();
        ICustomerService service = new CustomerService(repository);
        Check(service.Search().Count == 3, "Load demo customers into BUS");
        Check(service.Search("nguyen an").Single().Code == "KH000001", "Find Vietnamese names without accents");
        Check(service.Search("0900000002").Single().Code == "KH000002", "Find customers by phone");
        service.Search("KH"); service.Search("example.com");
        Check(repository.Reads == 1, "Search uses the BUS cache after loading");

        var saved = service.Create(Draft(" kh000900 "));
        Check(saved.Code == "KH000900" && saved.Name == "Khách Demo", "Create normalizes a CHAR code");
        Reject(() => service.Create(Draft("KH000900")), "Reject duplicate codes");

        var invalid = Draft("KH000901"); invalid.Phone = new string('1', 21);
        Reject(() => service.Create(invalid), "Reject phone longer than schema column");
        invalid = Draft("KH000901"); invalid.Name = " ";
        Reject(() => service.Create(invalid), "Reject blank name");
        invalid = Draft("KH000901"); invalid.Email = "bad-email";
        Reject(() => service.Create(invalid), "Reject invalid email");
        invalid = Draft("KH000901"); invalid.Address = new string('a', 301);
        Reject(() => service.Create(invalid), "Reject address longer than schema column");
        invalid = Draft("KH0000001");
        Reject(() => service.Create(invalid), "Reject code longer than CHAR(8)");

        var edit = CustomerDraft.From(saved);
        edit.Name = "Tên mới";
        Check(service.Get(saved.Code).Name == saved.Name, "Editing a draft does not mutate stored data before save");
        var updated = service.Update(saved.Code, edit);
        Check(updated.Name == "Tên mới" && updated.Code == saved.Code, "Edit retains the business key");
        edit.Code = "KH000901";
        Reject(() => service.Update(saved.Code, edit), "Reject code tampering");

        repository.FailWrites = true;
        var before = service.Get(saved.Code);
        edit = CustomerDraft.From(before); edit.Name = "Không lưu được";
        Reject(() => service.Update(saved.Code, edit), "Propagate update failure");
        Reject(() => service.Create(Draft("KH000902")), "Propagate insert failure");
        Reject(() => service.Delete(saved.Code), "Propagate delete failure");
        Check(service.Get(saved.Code) == before, "Storage failures leave existing data unchanged");
        repository.FailWrites = false;
        service.Delete(saved.Code);
        Check(!service.Search("KH000900").Any(), "Delete updates the BUS cache");
        Reject(() => service.Get(saved.Code), "Deleted customer is not found");
        Reject(() => repository.Insert(repository.GetByCode("KH000001")!), "Repository protects unique codes");

        var services = new ServiceCollection();
        services.AddScoped<ICustomerRepository, DemoCustomerRepository>();
        services.AddScoped<ICustomerService, CustomerService>();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        using var scopeA = provider.CreateScope(); using var scopeB = provider.CreateScope();
        var a = scopeA.ServiceProvider.GetRequiredService<ICustomerService>();
        var b = scopeB.ServiceProvider.GetRequiredService<ICustomerService>();
        a.Create(Draft("KH000950"));
        Check(a.Search("KH000950").Count == 1 && b.Search("KH000950").Count == 0, "Customer scopes isolate demo data");
    }

    private static CustomerDraft Draft(string code) => new()
    {
        Code = code, Name = "  Khách Demo  ", Phone = "0900000099", Email = "", Address = "123 Đường Demo, TP. Hồ Chí Minh"
    };

    private static void Check(bool value, string message)
    {
        if (!value) throw new Exception(message);
        Console.WriteLine("PASS CUSTOMER " + message);
    }

    private static void Reject(Action action, string message)
    {
        try { action(); }
        catch (InvalidOperationException) { Console.WriteLine("PASS CUSTOMER " + message); return; }
        throw new Exception(message);
    }

    private sealed class ObservedRepository : ICustomerRepository
    {
        private readonly DemoCustomerRepository inner = new();
        public int Reads { get; private set; }
        public bool FailWrites { get; set; }
        public IReadOnlyList<Customer> GetAll() { Reads++; return inner.GetAll(); }
        public Customer? GetByCode(string code) => inner.GetByCode(code);
        public Customer Insert(Customer value) { VerifyWrite(); return inner.Insert(value); }
        public Customer Update(Customer value) { VerifyWrite(); return inner.Update(value); }
        public void Delete(string code) { VerifyWrite(); inner.Delete(code); }
        private void VerifyWrite()
        {
            if (FailWrites) throw new InvalidOperationException("Simulated customer storage failure.");
        }
    }
}
