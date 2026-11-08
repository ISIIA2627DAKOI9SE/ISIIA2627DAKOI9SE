
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Controllers;

public class ComprasModelo3DController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ComprasModelo3DController> _logger;

    public ComprasModelo3DController(
        ApplicationDbContext context,
        ILogger<ComprasModelo3DController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET SELECT: Mostrar y filtrar los modelos disponibles.
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(
        typeof(IList<Modelo3DForPurchaseDTO>), 200)]
    public async Task<IActionResult> GetModelsForPurchase(
        string? nombre,
        string? categoria)
    {
        var modelos = _context.Modelo3Ds.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            modelos = modelos.Where(m =>
                m.Nombre.Contains(nombre));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            modelos = modelos.Where(m =>
                m.Categoria == categoria);
        }

        var resultado = await modelos
            .OrderBy(m => m.Id)
            .Select(m => new Modelo3DForPurchaseDTO(
                m.Id,
                m.Nombre,
                m.Categoria,
                m.Formato,
                m.Precio,
                m.LicenciasModelo3D
                    .OrderBy(l => l.Id)
                    .Select(l => l.Nombre)
                    .ToList()
            ))
            .ToListAsync();

        if (resultado.Count == 0)
        {
            _logger.LogInformation(
                "No se encontraron modelos 3D.");
        }

        return Ok(resultado);
    }

    // GET DETAILS: Consultar una compra existente.
    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(CompraModelo3DDetailDTO), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPurchase(int id)
    {
        var compra = await _context.ComprasModelo3Ds
            .AsNoTracking()
            .Include(c => c.LineasCompraModelo)
                .ThenInclude(l => l.Modelo3D)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (compra == null)
        {
            _logger.LogInformation(
                "No se encontró la compra con Id {Id}", id);

            return NotFound();
        }

        var lineas = compra.LineasCompraModelo
            .OrderBy(l => l.Modelo3D.Id)
            .Select(l => new LineaCompraModeloDetailDTO(
                l.Modelo3D.Id,
                l.Modelo3D.Nombre,
                l.Modelo3D.Categoria,
                l.Modelo3D.Formato,
                l.CantidadLicencias,
                l.PrecioUnidad,
                l.Subtotal
            ))
            .ToList();

        var resultado = new CompraModelo3DDetailDTO(
            compra.Id,
            compra.FechaCompra,
            compra.NombreCliente,
            compra.ApellidosCliente,
            compra.CorreoElectronico,
            compra.DireccionFacturacion,
            compra.Descripcion,
            compra.PrecioTotal,
            compra.MetodoPago,
            lineas
        );

        return Ok(resultado);
    }

    // POST CREATE: Confirmar y guardar una compra.
    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(CompraModelo3DDetailDTO), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreatePurchase(
        [FromBody] CompraModelo3DForCreateDTO? compraDTO)
    {
        // Validar el cuerpo de la petición.
        if (compraDTO == null)
        {
            return BadRequest(
                "Debe introducir los datos de la compra.");
        }

        // Validar los datos obligatorios.
        if (string.IsNullOrWhiteSpace(compraDTO.NombreCliente) ||
            string.IsNullOrWhiteSpace(compraDTO.ApellidosCliente) ||
            string.IsNullOrWhiteSpace(compraDTO.CorreoElectronico) ||
            string.IsNullOrWhiteSpace(compraDTO.DireccionFacturacion))
        {
            return BadRequest(
                "Debe completar los datos obligatorios.");
        }

        // Validar el método de pago permitido para modelos 3D.
        if (!Enum.IsDefined(compraDTO.MetodoPago) ||
            compraDTO.MetodoPago == MetodoPago.Bizum)
        {
            return BadRequest(
                "El método de pago debe ser TarjetaCredito o PayPal.");
        }

        // Comprobar que el carrito contiene modelos.
        if (compraDTO.Lineas == null ||
            compraDTO.Lineas.Count == 0)
        {
            return BadRequest(
                "Debe seleccionar al menos un modelo 3D.");
        }

        // Comprobar cantidades positivas.
        if (compraDTO.Lineas.Any(l =>
            l == null || l.CantidadLicencias <= 0))
        {
            return BadRequest(
                "Las cantidades de licencias deben ser positivas.");
        }

        // Evitar modelos repetidos.
        var ids = compraDTO.Lineas
            .Select(l => l.Modelo3DId)
            .ToList();

        if (ids.Distinct().Count() != ids.Count)
        {
            return BadRequest(
                "No puede incluir el mismo modelo dos veces.");
        }

        // Recuperar los modelos desde la base de datos.
        var modelos = await _context.Modelo3Ds
            .Where(m => ids.Contains(m.Id))
            .ToListAsync();

        if (modelos.Count != ids.Count)
        {
            return BadRequest(
                "Alguno de los modelos seleccionados no existe.");
        }

        // Calcular el total utilizando los precios reales.
        decimal precioTotal = 0m;

        foreach (var lineaDTO in compraDTO.Lineas)
        {
            var modelo = modelos.Single(m =>
                m.Id == lineaDTO.Modelo3DId);

            precioTotal +=
                modelo.Precio * lineaDTO.CantidadLicencias;
        }

        // Crear la compra mediante su constructor.
        var compra = new CompraModelo3D(
            0,
            DateTime.UtcNow,
            compraDTO.NombreCliente.Trim(),
            compraDTO.ApellidosCliente.Trim(),
            compraDTO.CorreoElectronico.Trim(),
            compraDTO.DireccionFacturacion.Trim(),
            compraDTO.Descripcion?.Trim() ?? "",
            precioTotal,
            compraDTO.MetodoPago
        );

        // Crear y asociar las líneas mediante sus constructores.
        foreach (var lineaDTO in compraDTO.Lineas)
        {
            var modelo = modelos.Single(m =>
                m.Id == lineaDTO.Modelo3DId);

            decimal precioUnidad = modelo.Precio;

            decimal subtotal =
                precioUnidad * lineaDTO.CantidadLicencias;

            var linea = new LineaCompraModelo(
                0,
                lineaDTO.CantidadLicencias,
                precioUnidad,
                subtotal
            );

            linea.Modelo3D = modelo;
            linea.CompraModelo3D = compra;

            compra.LineasCompraModelo.Add(linea);
        }

        // Persistir la compra y sus líneas.
        _context.ComprasModelo3Ds.Add(compra);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Compra de modelos 3D creada con Id {Id}",
            compra.Id);

        // Construir las líneas del DTO de respuesta.
        var lineasResultado = compra.LineasCompraModelo
            .OrderBy(l => l.Modelo3D.Id)
            .Select(l => new LineaCompraModeloDetailDTO(
                l.Modelo3D.Id,
                l.Modelo3D.Nombre,
                l.Modelo3D.Categoria,
                l.Modelo3D.Formato,
                l.CantidadLicencias,
                l.PrecioUnidad,
                l.Subtotal
            ))
            .ToList();

        // Construir el resultado de la compra.
        var resultado = new CompraModelo3DDetailDTO(
            compra.Id,
            compra.FechaCompra,
            compra.NombreCliente,
            compra.ApellidosCliente,
            compra.CorreoElectronico,
            compra.DireccionFacturacion,
            compra.Descripcion,
            compra.PrecioTotal,
            compra.MetodoPago,
            lineasResultado
        );

        // Devolver HTTP 201 con la ubicación de la compra.
        return CreatedAtAction(
            nameof(GetPurchase),
            new { id = compra.Id },
            resultado
        );
    }
}
