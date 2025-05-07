using Lab8_AngelYaguno.Domain.Interfaces;
using Lab8_AngelYaguno.Models;
using Lab8_AngelYaguno.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab8_AngelYaguno.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LinqController : ControllerBase
{
    private readonly ILinqService _linqService;

    public LinqController(ILinqService linqService)
    {
        _linqService = linqService;
    }

    [HttpGet("clients-by-name/{name}")]
    public async Task<IActionResult> GetClientsByName(string name)
    {
        var clients = await _linqService.GetClientsByName(name);
        
        if (!clients.Any())
            return NotFound($"No se encontraron clientes con el nombre '{name}'");
            
        return Ok(clients);
    }

    [HttpGet("products-above-price/{price}")]
    public async Task<IActionResult> GetProductsAbovePrice(decimal price)
    {
        var products = await _linqService.GetProductsAbovePrice(price);
        
        if (!products.Any())
            return NotFound($"No se encontraron productos con precio mayor a {price}");
            
        return Ok(products);
    }

    [HttpGet("products-in-order/{orderId}")]
    public async Task<IActionResult> GetProductsInOrder(int orderId)
    {
        var productDetails = await _linqService.GetProductsInOrder(orderId);
        
        if (!productDetails.Any())
            return NotFound($"No se encontraron detalles para la orden con ID {orderId}");
            
        return Ok(productDetails);
    }

    [HttpGet("total-products-in-order/{orderId}")]
    public async Task<IActionResult> GetTotalProductsInOrder(int orderId)
    {
        var totalQuantity = await _linqService.GetTotalProductsInOrder(orderId);
        return Ok(totalQuantity);
    }

    [HttpGet("most-expensive-product")]
    public async Task<IActionResult> GetMostExpensiveProduct()
    {
        var mostExpensiveProduct = await _linqService.GetMostExpensiveProduct();

        if (mostExpensiveProduct == null)
            return NotFound("No hay productos disponibles");

        return Ok(mostExpensiveProduct);
    }

    [HttpGet("orders-after-date/{date}")]
    public async Task<IActionResult> GetOrdersAfterDate(DateTime date)
    {
        var orders = await _linqService.GetOrdersAfterDate(date);
        
        if (!orders.Any())
            return NotFound($"No se encontraron pedidos después de {date}");
            
        return Ok(orders);
    }

    [HttpGet("average-price")]
    public async Task<IActionResult> GetAveragePrice()
    {
        var averagePrice = await _linqService.GetAveragePrice();
        return Ok(averagePrice);
    }

    [HttpGet("products-without-description")]
    public async Task<IActionResult> GetProductsWithoutDescription()
    {
        var products = await _linqService.GetProductsWithoutDescription();
        
        if (!products.Any())
            return NotFound("No se encontraron productos sin descripción");
            
        return Ok(products);
    }

    [HttpGet("client-with-most-orders")]
    public async Task<IActionResult> GetClientWithMostOrders()
    {
        var clientWithMostOrders = await _linqService.GetClientWithMostOrders();

        if (clientWithMostOrders == null)
            return NotFound("No hay clientes con pedidos");

        return Ok(clientWithMostOrders);
    }
    
    [HttpGet("orders-with-details")]
    public async Task<IActionResult> GetOrdersWithDetails()
    {
        var ordersWithDetails = await _linqService.GetOrdersWithDetails();
        
        if (!ordersWithDetails.Any())
            return NotFound("No se encontraron pedidos");
            
        return Ok(ordersWithDetails);
    }

    [HttpGet("products-purchased-by-client/{clientId}")]
    public async Task<IActionResult> GetProductsPurchasedByClient(int clientId)
    {
        var products = await _linqService.GetProductsPurchasedByClient(clientId);
        
        if (!products.Any())
            return NotFound($"No se encontraron productos comprados por el cliente con ID {clientId}");
            
        return Ok(products);
    }

    [HttpGet("clients-who-purchased-product/{productId}")]
    public async Task<IActionResult> GetClientsWhoPurchasedProduct(int productId)
    {
        var clients = await _linqService.GetClientsWhoPurchasedProduct(productId);
        
        if (!clients.Any())
            return NotFound($"No se encontraron clientes que hayan comprado el producto con ID {productId}");
            
        return Ok(clients);
    }
}