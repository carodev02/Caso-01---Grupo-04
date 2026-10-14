using System.ComponentModel.DataAnnotations;

namespace TechStore.Web.Models;

public sealed class CheckoutViewModel
{
    [Required(ErrorMessage = "Ingresa tu nombre.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa tu apellido.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa tu correo electrónico.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa tu celular.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "El celular debe contener exactamente 9 dígitos.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la dirección de entrega.")]
    [StringLength(160, MinimumLength = 8, ErrorMessage = "La dirección debe tener entre 8 y 160 caracteres.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona un distrito.")]
    public string District { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona un método de pago.")]
    [RegularExpression("^(Tarjeta|Transferencia|Contraentrega)$", ErrorMessage = "Selecciona un método de pago válido.")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tu carrito está vacío. Agrega productos antes de continuar.")]
    public string CartJson { get; set; } = string.Empty;
}

public sealed record CartItemInput(string Name, int Quantity);

public sealed record OrderLine(string Name, int Quantity, decimal UnitPrice)
{
    public decimal Total => Quantity * UnitPrice;
}

public sealed record OrderConfirmationViewModel(
    string OrderNumber,
    DateTime CreatedAt,
    string FullName,
    string Email,
    string Phone,
    string Address,
    string District,
    string PaymentMethod,
    IReadOnlyList<OrderLine> Items,
    decimal Subtotal,
    decimal Shipping,
    decimal Total);
