namespace TechStore.Web.Models;

public sealed record AdminDashboardViewModel(
    string AccessKey,
    int OrderCount,
    decimal SalesToday,
    int ProductCount,
    decimal WeeklySales,
    IReadOnlyList<DailySalesPoint> WeeklySalesByDay);

public sealed record DailySalesPoint(string Label, decimal Total, int HeightPercent, bool IsToday);
