using Microsoft.AspNetCore.Mvc;
using TechStore.Web.Models;

namespace TechStore.Web.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index() => View(ProductCatalog.All);
}
