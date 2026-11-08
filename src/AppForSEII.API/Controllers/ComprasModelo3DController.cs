
using AppForSEII.API.Data;
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

    // GET SELECT: Mostrar y filtrar los modelos 3D disponibles
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(
        typeof(IList<Modelo3DForPurchaseDTO>), 200)]
    public async Task<IActionResult> GetModelsForPurchase(
        string? nombre,
        string? categoria)
    {
        // Obtener los modelos disponibles
        var modelos = _context.Modelo3Ds.AsNoTracking();

        // Aplicar el filtro por nombre
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            modelos = modelos.Where(m =>
                m.Nombre.Contains(nombre));
        }

        // Aplicar el filtro por categoría
        if (!string.IsNullOrWhiteSpace(categoria))
        {
            modelos = modelos.Where(m =>
                m.Categoria == categoria);
        }

        // Construir los DTO de los modelos encontrados
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

        // Registrar si no existen modelos coincidentes
        if (resultado.Count == 0)
        {
            _logger.LogInformation(
                "No se encontraron modelos 3D.");
        }

        // Devolver los modelos encontrados
        return Ok(resultado);
    }

    // GET DETAILS: Consultar los detalles de una compra realizada
    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(CompraModelo3DDetailDTO), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPurchase(int id)
    {
        // Buscar la compra y sus modelos asociados
        var compra = await _context.ComprasModelo3Ds
            .AsNoTracking()
            .Include(c => c.LineasCompraModelo)
                .ThenInclude(l => l.Modelo3D)
            .FirstOrDefaultAsync(c => c.Id == id);

        // Comprobar si la compra existe
        if (compra == null)
        {
            _logger.LogInformation(
                "No se encontró la compra de modelos 3D con Id {Id}",
                id);

            return NotFound();
        }

        // Construir los detalles de los modelos comprados
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

        // Construir el DTO con los datos de la compra
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

        // Devolver los detalles de la compra
        return Ok(resultado);
    }
}
