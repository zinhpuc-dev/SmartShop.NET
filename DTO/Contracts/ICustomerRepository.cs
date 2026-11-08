namespace SmartShop.DTO.Contracts;

public interface ICustomerRepository
{
    IReadOnlyList<Customer> GetAll();
    Customer? GetByCode(string code);
    Customer Insert(Customer customer);
    Customer Update(Customer customer);
    void Delete(string code);
}
