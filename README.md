# Nexa · Auditoría y protección con middleware

Nexa es una tienda tecnológica demostrativa desarrollada con ASP.NET Core 8. El proyecto usa una interfaz comercial completa para mostrar, en un contexto realista, cómo una canalización de middleware registra solicitudes, protege una zona administrativa y atiende una ruta terminal.

## Requisitos

- .NET SDK 8 o superior.
- Navegador web moderno.

## Ejecución

Desde la carpeta raíz:

```powershell
dotnet run --project src/TechStore.Web --urls http://localhost:5080
```

Abrir `http://localhost:5080`.

## Rutas para la demostración

| Ruta | Resultado esperado |
| --- | --- |
| `/` | Tienda Nexa y archivos estáticos |
| `/estado` | Respuesta terminal: `Nexa está operativo` |
| `/admin` | `403 Forbidden` porque no incluye la clave |
| `/admin?clave=nexa-2026` | Panel administrativo protegido |
| `/ruta-inexistente` | `404 Not Found` |

## Componentes del caso

- `Middleware/RequestLogMiddleware.cs`: mide la duración con `Stopwatch` y registra método, ruta, estado HTTP y milisegundos mediante `ILogger`.
- `Extensions/RequestLogMiddlewareExtensions.cs`: expone el método `UseRequestLog()`.
- `Program.cs`: configura la canalización, el cortocircuito para `/admin`, la ruta terminal `/estado` y el orden respecto a los archivos estáticos.
- `wwwroot/`: contiene la tienda pública, el panel administrativo y los recursos visuales.

## Demostración del orden

La propiedad `RequestLogging:AfterStaticFiles` de `appsettings.json` controla la ubicación efectiva del registro sin tener que alterar otras partes del proyecto.

### Registro antes de los archivos estáticos

Mantener:

```json
"RequestLogging": {
  "AfterStaticFiles": false
}
```

Al recargar la tienda, la consola registra `/`, `/css/site.css`, `/js/site.js`, `/favicon.svg` y `/estado`. `UseRequestLog()` entra antes de `UseStaticFiles()` y observa todas esas solicitudes.

### Registro después de los archivos estáticos

Cambiar el valor a `true` y reiniciar la aplicación. Los recursos atendidos por `UseStaticFiles()` terminan la canalización antes de llegar al middleware de auditoría. La consola continúa registrando rutas dinámicas como `/estado`, `/admin` y las respuestas 404.

## Flujo principal

```text
Solicitud
   │
   ├─ RequestLogMiddleware
   │      inicia Stopwatch
   │
   ├─ Protección /admin
   │      sin clave: 403 y cortocircuito
   │      con clave: continúa
   │
   ├─ Archivos estáticos
   │
   ├─ /estado o /admin
   │
   └─ Respuesta 404
          │
          └─ RequestLogMiddleware registra estado y tiempo
```

## Nota de seguridad

La clave incluida en `appsettings.json` sirve únicamente para una demostración académica. En una aplicación real se usarían autenticación, autorización por roles y almacenamiento seguro de secretos.
