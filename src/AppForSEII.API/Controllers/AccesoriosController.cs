using AppForSEII.API.DTOs.AccesorioDTOs;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccesoriosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccesoriosController> _logger;

    public AccesoriosController(ApplicationDbContext context, ILogger<AccesoriosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]")]
    [Route("Select")]
    [ProducesResponseType(typeof(IList<AccesorioParaCompraDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IList<AccesorioParaCompraDTO>>> GetAccesoriosForPurchase(string? nombre, CategoriaAccesorio? categoria)
    {
        var query = _context.Accesorios.AsQueryable();

        // Flujo alternativo 2.1: Solo debe devolver accesorios donde CantidadDisponible > 0
        query = query.Where(a => a.CantidadDisponible > 0);

        // Filtro por nombre
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(a => a.Nombre.ToLower().Contains(nombre.ToLower()));
        }

        // Filtro por categoria
        if (categoria.HasValue)
        {
            query = query.Where(a => a.CategoriaAccesorio == categoria.Value);
        }

        var result = await query
            .OrderBy(a => a.Nombre)
            .Select(a => new AccesorioParaCompraDTO(
                a.Id,
                a.Nombre,
                a.CategoriaAccesorio,
                a.Compatibilidad,
                a.CantidadDisponible,
                a.Precio
            ))
            .ToListAsync();

        return Ok(result);
    }
}
