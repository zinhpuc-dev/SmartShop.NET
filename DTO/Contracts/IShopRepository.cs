using System.Collections;

namespace SmartShop.DTO.Contracts;

/// <summary>Storage contract shared by BUS and DAO, independent of a database provider.</summary>
public interface IShopRepository
{
    IReadOnlyList<Product> GetProducts();
    ArrayList LoadProductArray();
    IReadOnlyList<Order> GetOrders(string phone);
    void UpdateProduct(Product value);
    void UpdateOrderStatus(string code, string status);

    /// <summary>Persist the order and stock changes atomically, or leave storage unchanged.</summary>
    Order CommitOrder(Order order);
}
