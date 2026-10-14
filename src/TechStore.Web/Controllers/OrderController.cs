using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TechStore.Web.Models;

namespace TechStore.Web.Controllers;

public sealed class OrderController : Controller
{
    private const decimal StandardShipping = 12m;
    private const decimal FreeShippingMinimum = 250m;
    private static readonly string[] LimaDistricts =
    [
        "Cercado de Lima", "Miraflores", "San Isidro", "Surco", "San Borja",
        "La Molina", "Barranco", "Jesús María", "Lince", "Pueblo Libre", "Otro distrito de Lima"
    ];
    private static readonly JsonSerializerOptions CartJsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly ConcurrentDictionary<string, OrderConfirmationViewModel> Orders = new();

    [HttpGet]
    public IActionResult Checkout() => View(new CheckoutViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Checkout(CheckoutViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.District) &&
            !LimaDistricts.Contains(model.District, StringComparer.Ordinal))
        {
            ModelState.AddModelError(nameof(model.District), "Selecciona un distrito disponible.");
        }

        List<CartItemInput>? cartItems = null;
        var invalidCartFormat = false;
        if (!string.IsNullOrWhiteSpace(model.CartJson))
        {
            try
            {
                cartItems = JsonSerializer.Deserialize<List<CartItemInput>>(model.CartJson, CartJsonOptions);
            }
            catch (JsonException)
            {
                invalidCartFormat = true;
                ModelState.AddModelError(nameof(model.CartJson), "No se pudo leer el carrito. Regresa a la tienda e inténtalo nuevamente.");
            }
        }

        if ((cartItems is null || cartItems.Count == 0) && !invalidCartFormat)
        {
            ModelState.Remove(nameof(model.CartJson));
            ModelState.AddModelError(nameof(model.CartJson), "Tu carrito está vacío. Agrega productos antes de continuar.");
        }

        var orderLines = new List<OrderLine>();
        if (cartItems is not null)
        {
            foreach (var item in cartItems)
            {
                if (item is null || string.IsNullOrWhiteSpace(item.Name))
                {
                    ModelState.AddModelError(nameof(model.CartJson), "El carrito contiene un producto inválido.");
                    continue;
                }

                var product = ProductCatalog.All.FirstOrDefault(candidate =>
                    string.Equals(candidate.Name, item.Name, StringComparison.OrdinalIgnoreCase));

                if (product is null)
                {
                    ModelState.AddModelError(nameof(model.CartJson), "El carrito contiene un producto no disponible. Actualiza la tienda e inténtalo nuevamente.");
                    continue;
                }

                if (item.Quantity is < 1 or > 10)
                {
                    ModelState.AddModelError(nameof(model.CartJson), $"La cantidad de {product.Name} debe estar entre 1 y 10 unidades.");
                    continue;
                }

                orderLines.Add(new OrderLine(product.Name, item.Quantity, product.Price));
            }
        }

        orderLines = orderLines
            .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => new OrderLine(group.First().Name, group.Sum(item => item.Quantity), group.First().UnitPrice))
            .ToList();
        foreach (var item in orderLines.Where(item => item.Quantity > 10))
        {
            ModelState.AddModelError(nameof(model.CartJson), $"La cantidad total de {item.Name} no puede superar 10 unidades.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var subtotal = orderLines.Sum(item => item.Total);
        var shipping = subtotal >= FreeShippingMinimum ? 0 : StandardShipping;
        var orderNumber = $"NX-{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        var order = new OrderConfirmationViewModel(
            orderNumber,
            DateTime.Now,
            $"{model.FirstName.Trim()} {model.LastName.Trim()}",
            model.Email,
            model.Phone,
            model.Address,
            model.District,
            model.PaymentMethod,
            orderLines,
            subtotal,
            shipping,
            subtotal + shipping);

        Orders[orderNumber] = order;
        return RedirectToAction(nameof(Confirmation), new { id = orderNumber });
    }

    [HttpGet]
    public IActionResult Confirmation(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !Orders.TryGetValue(id, out var order))
        {
            return NotFound("No encontramos ese pedido.");
        }

        return View(order);
    }
}
