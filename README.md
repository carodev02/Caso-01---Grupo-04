# Nexa Tech Store · Caso 1

Aplicación **ASP.NET Core Web App (Model-View-Controller)** para demostrar middleware personalizados en el contexto de una tienda tecnológica. La interfaz pública y el panel son vistas Razor; el catálogo se entrega desde un modelo en memoria y no requiere base de datos.

## Abrir y ejecutar en Visual Studio

1. Abrir `TechStore.slnx` con Visual Studio 2022 17.12 o Visual Studio 2026.
2. Seleccionar el perfil **http** y ejecutar con **F5** o **Ctrl+F5**.
3. La aplicación queda disponible en `http://localhost:5291`.

También se puede ejecutar desde la raíz del repositorio:

```powershell
dotnet run --project src/TechStore.Web --urls http://localhost:5080
```

## Rutas de demostración

| Ruta | Resultado |
| --- | --- |
| `/` | `HomeController` entrega el catálogo a `Views/Home/Index.cshtml` |
| `/admin` | Panel de `AdminController`; sin clave devuelve 403 |
| `/admin?clave=nexa-2026` | Panel de administración de demostración |
| `/estado` | Rama terminal `app.Map` responde `Nexa está operativo` |
| `/Order/Checkout` | Formulario de compra con resumen y validación |
| `/ruta-inexistente` | Respuesta 404 |

## Catálogo y compra

El catálogo carga quince productos en memoria, cinco por categoría (**Audio**, **Movilidad** y **Trabajo**), con filtros Todo/Audio/Movilidad/Trabajo. El carrito persiste en el navegador durante la demostración.

Al finalizar la compra, el checkout valida en servidor nombre, apellido, correo, celular de nueve dígitos, dirección, distrito, método de pago y contenido del carrito. El servidor verifica que cada producto exista, limita cantidades a diez y vuelve a calcular los precios desde el catálogo; no confía en los importes enviados por el navegador. Muestra errores por campo y, si todo es válido, crea una confirmación de pedido en memoria.

El envío cuesta S/ 12 y es gratis desde S/ 250. Las opciones de pago son simuladas; no se almacenan datos de tarjetas ni se realizan transacciones. Los pedidos en memoria se pierden al reiniciar la aplicación.

## Estructura MVC y middleware

- `Controllers/HomeController.cs`: obtiene la lista en memoria para la tienda.
- `Controllers/AdminController.cs`: presenta la vista administrativa.
- `Controllers/OrderController.cs`: valida y registra pedidos en memoria.
- `Models/Product.cs` y `Models/ProductCatalog.cs`: catálogo en memoria.
- `Models/CheckoutViewModel.cs`: reglas de validación y modelos del checkout.
- `Views/Order/Checkout.cshtml` y `Views/Order/Confirmation.cshtml`: compra y confirmación.
- `Views/Home/Index.cshtml` y `Views/Admin/Index.cshtml`: vistas Razor.
- `Middleware/RequestLogMiddleware.cs`: registra método, ruta, estado y duración con `ILogger` y `Stopwatch`.
- `Extensions/RequestLogMiddlewareExtensions.cs`: registra el componente mediante `UseRequestLog()`.
- `Program.cs`: configura MVC, el cortocircuito de `/admin`, archivos estáticos, `/estado` y las rutas MVC.
- `wwwroot/`: CSS, JavaScript e icono estáticos; la interfaz HTML ya no se sirve directamente desde esta carpeta.

## Comparación del orden del logger

En `appsettings.json`, `RequestLogging:AfterStaticFiles` controla la demostración:

- `false`: el logger está antes de `UseStaticFiles()` y también registra CSS, JavaScript e iconos.
- `true`: el logger queda después de `UseStaticFiles()`; los archivos estáticos servidos antes no aparecen en esos registros.

Reiniciar la aplicación después de modificar la propiedad. En ambas posiciones, una solicitud bloqueada por el control de `/admin` puede no llegar al logger cuando este middleware está después del control de acceso.

## Seguridad

La clave `nexa-2026` es únicamente didáctica y se lee desde configuración. El control por query string no sustituye autenticación y autorización reales; no debe usarse en producción.
