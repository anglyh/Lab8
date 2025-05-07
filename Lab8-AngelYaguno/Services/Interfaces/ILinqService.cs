using Lab8_AngelYaguno.Models;

namespace Lab8_AngelYaguno.Services;

public interface ILinqService
{
    Task<IEnumerable<Client>> GetClientsByName(string name);
    Task<IEnumerable<Product>> GetProductsAbovePrice(decimal price);
    Task<IEnumerable<object>> GetProductsInOrder(int orderId);
    Task<object> GetTotalProductsInOrder(int orderId);
    Task<Product?> GetMostExpensiveProduct();
    Task<IEnumerable<Order>> GetOrdersAfterDate(DateTime date);
    Task<object> GetAveragePrice();
    Task<IEnumerable<Product>> GetProductsWithoutDescription();
    Task<object?> GetClientWithMostOrders();
    Task<IEnumerable<object>> GetOrdersWithDetails();
    Task<IEnumerable<object>> GetProductsPurchasedByClient(int clientId);
    Task<IEnumerable<Client>> GetClientsWhoPurchasedProduct(int productId);
    
}