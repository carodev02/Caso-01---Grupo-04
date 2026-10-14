namespace TechStore.Web.Models;

public sealed record Product(
    string Name,
    string Category,
    string CategoryLabel,
    decimal Price,
    string Tag,
    string ArtClass);
