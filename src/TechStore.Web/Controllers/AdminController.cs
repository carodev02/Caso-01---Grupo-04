using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Web.Models;
using TechStore.Web.Services;

namespace TechStore.Web.Controllers;

public sealed class AdminController : Controller
{
    private readonly InMemoryOrderStore _orderStore;

    public AdminController(InMemoryOrderStore orderStore)
    {
        _orderStore = orderStore;
    }

    public IActionResult Index()
    {
        var orders = _orderStore.GetAll();
        var today = DateTime.Today;
        var rawDailySales = Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var date = today.AddDays(offset - 6);
                var total = orders.Where(order => order.CreatedAt.Date == date).Sum(order => order.Total);
                return (Date: date, Total: total);
            })
            .ToArray();
        var highestDay = rawDailySales.Max(day => day.Total);
        var culture = CultureInfo.GetCultureInfo("es-PE");
        var dailySales = rawDailySales.Select(day => new DailySalesPoint(
            culture.TextInfo.ToTitleCase(day.Date.ToString("ddd", culture)),
            day.Total,
            day.Total == 0 ? 1 : Math.Clamp((int)Math.Round(day.Total / highestDay * 92), 8, 92),
            day.Date == today)).ToArray();

        return View(new AdminDashboardViewModel(
            Request.Query["clave"].ToString(),
            orders.Count,
            orders.Where(order => order.CreatedAt.Date == today).Sum(order => order.Total),
            ProductCatalog.All.Count,
            rawDailySales.Sum(day => day.Total),
            dailySales));
    }

    public IActionResult Orders() => View(new AdminOrdersViewModel(
        Request.Query["clave"].ToString(),
        _orderStore.GetAll()));
}
