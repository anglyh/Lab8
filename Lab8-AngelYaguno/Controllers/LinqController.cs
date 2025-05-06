using Lab8_AngelYaguno.Domain.Interfaces;
using Lab8_AngelYaguno.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab8_AngelYaguno.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LinqController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LinqController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Ejercicio 1: Obtener los Clientes que Tienen un Nombre Específico
    [HttpGet("clients-by-name/{name}")]
    public async Task<IActionResult> GetClientsByName(string name)
    {
        var allClients = await _unitOfWork.Repository<Client>().GetAll();
        var clients = allClients.Where(c => c.Name.Contains(name)).ToList();

        return Ok(clients);
    }

    // Ejercicio 2: Obtener los Productos con Precio Mayor a un Valor Específico
    [HttpGet("products-above-price/{price}")]
    public async Task<IActionResult> GetProductsAbovePrice(decimal price)
    {
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        var products = allProducts.Where(c => c.Price > price).ToList();

        return Ok(products);
    }

    // Ejercicio 3: Obtener el Detalle de los Productos en una Orden
    [HttpGet("products-in-order/{orderId}")]
    public async Task<IActionResult> GetProductsInOrder(int orderId)
    {
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();

        var productDetails = allOrderDetails
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

        return Ok(productDetails);
    }

    // Ejercicio 4: Obtener la Cantidad Total de Productos por Orden
    [HttpGet("total-products-in-order/{orderId}")]
    public async Task<IActionResult> GetTotalProductsInOrder(int orderId)
    {
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var totalQuantity = allOrderDetails
            .Where(od => od.OrderId == orderId)
            .Sum(od => od.Quantity);

        return Ok(new { OrderId = orderId, TotalQuantity = totalQuantity });
    }

    // Ejercicio 5: Obtener el Producto Más Caro
    [HttpGet("most-expensive-product")]
    public async Task<IActionResult> GetMostExpensiveProduct()
    {
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        var mostExpensiveProduct = allProducts
            .OrderByDescending(p => p.Price)
            .FirstOrDefault();

        if (mostExpensiveProduct == null)
            return NotFound("No hay productos disponibles");

        return Ok(mostExpensiveProduct);
    }

    // Ejercicio 6: Obtener Todos los Pedidos Realizados Después de una Fecha Específica
    [HttpGet("orders-after-date/{date}")]
    public async Task<IActionResult> GetOrdersAfterDate(DateTime date)
    {
        var allOrders = await _unitOfWork.Repository<Order>().GetAll();
        var orders = allOrders
            .Where(o => o.OrderDate > date)
            .ToList();

        return Ok(orders);
    }

    // Ejercicio 7: Obtener el Promedio de Precio de los Productos
    [HttpGet("average-price")]
    public async Task<IActionResult> GetAveragePrice()
    {
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        var averagePrice = allProducts.Average(p => p.Price);

        return Ok(new { AveragePrice = averagePrice });
    }

    // Ejercicio 8: Obtener Todos los Productos que No Tienen Descripción
    [HttpGet("products-without-description")]
    public async Task<IActionResult> GetProductsWithoutDescription()
    {
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        var products = allProducts
            .Where(p => p.Description == null || p.Description == string.Empty)
            .ToList();

        return Ok(products);
    }

    // Ejercicio 9: Obtener el Cliente con Mayor Número de Pedidos
    [HttpGet("client-with-most-orders")]
    public async Task<IActionResult> GetClientWithMostOrders()
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
            return NotFound("No hay clientes con pedidos");

        return Ok(new
        {
            Client = clientWithMostOrders.Client,
            OrderCount = clientWithMostOrders.OrderCount
        });
    }

    // Ejercicio 10: Obtener Todos los Pedidos y sus Detalles
    [HttpGet("orders-with-details")]
    public async Task<IActionResult> GetOrdersWithDetails()
    {
        var allOrders = await _unitOfWork.Repository<Order>().GetAll();
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        var allClients = await _unitOfWork.Repository<Client>().GetAll();
        
        var ordersWithDetails = allOrders
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

        return Ok(ordersWithDetails);
    }

    // Ejercicio 11: Obtener Todos los Productos Vendidos por un Cliente Específico
    [HttpGet("products-purchased-by-client/{clientId}")]
    public async Task<IActionResult> GetProductsPurchasedByClient(int clientId)
    {
        var allOrders = await _unitOfWork.Repository<Order>().GetAll();
        var allOrderDetails = await _unitOfWork.Repository<Orderdetail>().GetAll();
        var allProducts = await _unitOfWork.Repository<Product>().GetAll();
        
        var clientOrders = allOrders.Where(o => o.ClientId == clientId).ToList();
        
        if (!clientOrders.Any())
            return NotFound($"No se encontraron pedidos para el cliente con ID {clientId}");
        
        var products = clientOrders
            .SelectMany(o => allOrderDetails
                .Where(od => od.OrderId == o.OrderId)
                .Select(od => new
                {
                    ProductId = od.ProductId,
                    ProductName = allProducts.FirstOrDefault(p => p.ProductId == od.ProductId)?.Name,
                    Quantity = od.Quantity
                }))
            .ToList();

        if (!products.Any())
            return NotFound($"No se encontraron productos comprados por el cliente con ID {clientId}");

        return Ok(products);
    }

    // Ejercicio 12: Obtener Todos los Clientes que Han Comprado un Producto Específico
    [HttpGet("clients-who-purchased-product/{productId}")]
    public async Task<IActionResult> GetClientsWhoPurchasedProduct(int productId)
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
            return NotFound($"No se encontraron pedidos que incluyan el producto con ID {productId}");
            
        var clientIds = allOrders
            .Where(o => orderIds.Contains(o.OrderId))
            .Select(o => o.ClientId)
            .Distinct()
            .ToList();
            
        var clients = allClients
            .Where(c => clientIds.Contains(c.ClientId))
            .ToList();

        if (!clients.Any())
            return NotFound($"No se encontraron clientes que hayan comprado el producto con ID {productId}");

        return Ok(clients);
    }
}