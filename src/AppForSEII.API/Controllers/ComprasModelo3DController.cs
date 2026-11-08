
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
}
