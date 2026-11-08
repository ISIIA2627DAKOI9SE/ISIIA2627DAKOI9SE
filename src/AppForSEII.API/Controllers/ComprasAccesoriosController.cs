using AppForSEII.API.DTOs.CompraAccesorioDTOs;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComprasAccesoriosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ComprasAccesoriosController> _logger;

    public ComprasAccesoriosController(ApplicationDbContext context, ILogger<ComprasAccesoriosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(CompraAccesorioDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<CompraAccesorioDetailDTO>> GetCompraAccesorio(int id)
    {
        var compra = await _context.CompraAccesorios
            .Include(c => c.LineaCompraAccesorios)
                .ThenInclude(l => l.Accesorio)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (compra == null)
        {
            _logger.LogError($"Error: CompraAccesorio with id {id} does not exist");
            return NotFound();
        }

        var lineasDTO = compra.LineaCompraAccesorios.Select(linea => new LineaCompraAccesorioDTO(
            linea.AccesorioId,
            linea.Accesorio.Nombre,
            linea.Accesorio.CategoriaAccesorio,
            linea.Accesorio.Compatibilidad,
            linea.Cantidad,
            linea.PrecioUnidad,
            linea.Subtotal
        )).ToList();

        var compraDetailDTO = new CompraAccesorioDetailDTO(
            compra.Id,
            compra.FechaCompra,
            compra.NombreCliente,
            compra.ApellidosCliente,
            compra.DireccionEnvio,
            compra.NumeroTelefono,
            compra.PrecioTotal,
            compra.MetodoPago,
            compra.ClienteId,
            lineasDTO
        );

        return Ok(compraDetailDTO);
    }
}