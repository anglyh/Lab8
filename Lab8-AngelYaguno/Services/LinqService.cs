using Lab8_AngelYaguno.Domain.Interfaces;
using Lab8_AngelYaguno.Models;

namespace Lab8_AngelYaguno.Services;

public class LinqService : ILinqService
{
    private readonly IUnitOfWork _unitOfWork;

    public LinqService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // Ejercicio 1: Obtener los Clientes que Tienen un Nombre Específico
    public async Task<IEnumerable<Client>> GetClientsByName(string name)
    {
        var allClients = await _unitOfWork.Repository<Client>().GetAll();
        return allClients.Where(c => c.Name.Contains(name)).ToList();
    }

    // Ejercicio 2: Obtener los Productos con Precio Mayor a un Valor Específico
    public async Task<IEnumerable<Product>> GetProductsAbovePrice(decimal price)
    {
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        return allProducts.Where(c => c.Price > price).ToList();
    }

    // Ejercicio 3: Obtener el Detalle de los Productos en una Orden
    public async Task<IEnumerable<object>> GetProductsInOrder(int orderId)
    {
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();

        return allOrderDetails
            .Where(od => od.OrderId == orderId)
            .Join(allProducts,
                od => od.ProductId,
                p => p.ProductId,
                (od, p) => new
                {
                    ProductName = p.Name,
                    Quantity = od.Quantity
                })
            .ToList();
    }

    // Ejercicio 4: Obtener la Cantidad Total de Productos por Orden
    public async Task<object> GetTotalProductsInOrder(int orderId)
    {
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var totalQuantity = allOrderDetails
            .Where(od => od.OrderId == orderId)
            .Sum(od => od.Quantity);

        return new { OrderId = orderId, TotalQuantity = totalQuantity };
    }

    // Ejercicio 5: Obtener el Producto Más Caro
    public async Task<Product?> GetMostExpensiveProduct()
    {
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        return allProducts
            .OrderByDescending(p => p.Price)
            .FirstOrDefault();
    }

    // Ejercicio 6: Obtener Todos los Pedidos Realizados Después de una Fecha Específica
    public async Task<IEnumerable<Order>> GetOrdersAfterDate(DateTime date)
    {
        var allOrders = await _unitOfWork.Repository<Order>().GetAll();
        return allOrders
            .Where(o => o.OrderDate > date)
            .ToList();
    }

    // Ejercicio 7: Obtener el Promedio de Precio de los Productos
    public async Task<object> GetAveragePrice()
    {
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        var averagePrice = allProducts.Average(p => p.Price);

        return new { AveragePrice = averagePrice };
    }

    // Ejercicio 8: Obtener Todos los Productos que No Tienen Descripción
    public async Task<IEnumerable<Product>> GetProductsWithoutDescription()
    {
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        return allProducts
            .Where(p => p.Description == null || p.Description == string.Empty)
            .ToList();
    }

    // Ejercicio 9: Obtener el Cliente con Mayor Número de Pedidos
    public async Task<object?> GetClientWithMostOrders()
    {
        var allClients = await _unitOfWork.Repository<Client>().GetAll();
        var allOrders = await _unitOfWork.Repository<Order>().GetAll();
        
        var clientWithMostOrders = allClients
            .Select(c => new
            {
                Client = c,
                OrderCount = allOrders.Count(o => o.ClientId == c.ClientId)
            })
            .OrderByDescending(x => x.OrderCount)
            .FirstOrDefault();

        if (clientWithMostOrders == null)
            return null;

        return new
        {
            Client = clientWithMostOrders.Client,
            OrderCount = clientWithMostOrders.OrderCount
        };
    }

    // Ejercicio 10: Obtener Todos los Pedidos y sus Detalles
    public async Task<IEnumerable<object>> GetOrdersWithDetails()
    {
        var allOrders = await _unitOfWork.Repository<Order>().GetAll();
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        var allClients = await _unitOfWork.Repository<Client>().GetAll();
        
        return allOrders
            .Select(o => new
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ClientName = allClients.FirstOrDefault(c => c.ClientId == o.ClientId)?.Name,
                Details = allOrderDetails
                    .Where(od => od.OrderId == o.OrderId)
                    .Select(od => new
                    {
                        ProductName = allProducts.FirstOrDefault(p => p.ProductId == od.ProductId)?.Name,
                        Quantity = od.Quantity
                    })
                    .ToList()
            })
            .ToList();
    }

    // Ejercicio 11: Obtener Todos los Productos Vendidos por un Cliente Específico
    public async Task<IEnumerable<object>> GetProductsPurchasedByClient(int clientId)
    {
        var allOrders = await _unitOfWork.Repository<Order>().GetAll();
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        
        var clientOrders = allOrders.Where(o => o.ClientId == clientId).ToList();
        
        if (!clientOrders.Any())
            return new List<object>();
        
        return clientOrders
            .SelectMany(o => allOrderDetails
                .Where(od => od.OrderId == o.OrderId)
                .Select(od => new
                {
                    ProductId = od.ProductId,
                    ProductName = allProducts.FirstOrDefault(p => p.ProductId == od.ProductId)?.Name,
                    Quantity = od.Quantity
                }))
            .ToList();
    }

    // Ejercicio 12: Obtener Todos los Clientes que Han Comprado un Producto Específico
    public async Task<IEnumerable<Client>> GetClientsWhoPurchasedProduct(int productId)
    {
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var allOrders = await _unitOfWork.Repository<Order>().GetAll();
        var allClients = await _unitOfWork.Repository<Client>().GetAll();
        
        var orderIds = allOrderDetails
            .Where(od => od.ProductId == productId)
            .Select(od => od.OrderId)
            .Distinct()
            .ToList();
            
        if (!orderIds.Any())
            return new List<Client>();
            
        var clientIds = allOrders
            .Where(o => orderIds.Contains(o.OrderId))
            .Select(o => o.ClientId)
            .Distinct()
            .ToList();
            
        return allClients
            .Where(c => clientIds.Contains(c.ClientId))
            .ToList();
    }
}