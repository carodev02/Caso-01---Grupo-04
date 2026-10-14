using Microsoft.AspNetCore.Mvc;

namespace TechStore.Web.Controllers;

public sealed class AdminController : Controller
{
    public IActionResult Index() => View();
}
