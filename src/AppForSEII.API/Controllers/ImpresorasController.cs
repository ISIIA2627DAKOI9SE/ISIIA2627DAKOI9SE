using AppForSEII.API.Data;
using AppForSEII.API.DTOs.ImpresoraDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImpresorasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ImpresorasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(
        typeof(IList<ImpresoraParaReservaDTO>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult> GetImpresorasParaReserva(
        string? nombre = null,
        TipoImpresora? tipo = null)
    {
        IList<ImpresoraParaReservaDTO> impresoras =
            await _context.Impresora3Ds
                .Where(impresora =>
                    (nombre == null || impresora.Nombre.Contains(nombre))
                    && (tipo == null || impresora.Tipo == tipo))
                .OrderBy(impresora => impresora.Nombre)
                .Select(impresora => new ImpresoraParaReservaDTO(
                    impresora.Id,
                    impresora.Nombre,
                    impresora.Modelo,
                    impresora.Tipo,
                    impresora.Descripcion,
                    impresora.PrecioKilovatioHora,
                    impresora.PrecioReserva))
                .ToListAsync();

        return Ok(impresoras);
    }
}