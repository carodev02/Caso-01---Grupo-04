namespace TechStore.Web.Models;

public static class ProductCatalog
{
    public static IReadOnlyList<Product> All { get; } =
    [
        new("Pulse Pro", "audio", "Audio", 549, "Más vendido", "art-headphones"),
        new("Echo Buds", "audio", "Audio", 229, "Nuevo", "art-headphones"),
        new("Studio Max", "audio", "Audio", 899, "Premium", "art-headphones"),
        new("Boom Mini", "audio", "Audio", 329, "", "art-headphones"),
        new("Wave ANC", "audio", "Audio", 459, "Cancelación de ruido", "art-headphones"),
        new("Loop Watch", "movilidad", "Movilidad", 419, "Más vendido", "art-watch"),
        new("Nova Band", "movilidad", "Movilidad", 279, "Nuevo", "art-watch"),
        new("Motion Fit", "movilidad", "Movilidad", 349, "", "art-watch"),
        new("AirGlide", "movilidad", "Movilidad", 1899, "Edición 2026", "art-scooter"),
        new("Urban Ride", "movilidad", "Movilidad", 2299, "Autonomía extendida", "art-scooter"),
        new("Arc Mini", "trabajo", "Trabajo", 289, "Nuevo", "art-keyboard"),
        new("Flow Mouse", "trabajo", "Trabajo", 169, "", "art-mouse"),
        new("AeroBook 14", "trabajo", "Trabajo", 2899, "Más vendido", "art-laptop"),
        new("Key Pro", "trabajo", "Trabajo", 359, "", "art-keyboard"),
        new("DeskMate Ergo", "trabajo", "Trabajo", 249, "Diseño ergonómico", "art-mouse")
    ];
}
