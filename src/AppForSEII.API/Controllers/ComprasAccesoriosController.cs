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

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(CompraAccesorioDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CompraAccesorioDetailDTO>> CreateCompraAccesorio([FromBody] CompraAccesorioParaCrearDTO compraParaCrearDTO)
    {
        if (compraParaCrearDTO == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Flujo alternativo 4: el carrito no puede estar vacío
        if (compraParaCrearDTO.LineasCompraAccesorio == null || compraParaCrearDTO.LineasCompraAccesorio.Count == 0)
        {
            ModelState.AddModelError("LineasCompraAccesorio", "La lista de accesorios no puede estar vacía.");
            return BadRequest(ModelState);
        }

        // No se puede repetir el mismo accesorio en varias líneas
        if (compraParaCrearDTO.LineasCompraAccesorio.GroupBy(l => l.AccesorioId).Any(g => g.Count() > 1))
        {
            ModelState.AddModelError("LineasCompraAccesorio", "No se puede repetir el mismo accesorio en varias líneas de la compra.");
            return BadRequest(ModelState);
        }

        // El cliente tiene que existir en la base de datos
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == compraParaCrearDTO.ClienteId || c.UserName == compraParaCrearDTO.ClienteId);

        if (cliente == null)
        {
            ModelState.AddModelError("ClienteId", $"El usuario '{compraParaCrearDTO.ClienteId}' no existe en la base de datos.");
            return BadRequest(ModelState);
        }

        // Flujos alternativos 3.1 y 7.3: cantidad válida y stock suficiente
        var accesoriosParaComprar = new List<(Accesorio Accesorio, int Cantidad)>();

        foreach (var lineaDto in compraParaCrearDTO.LineasCompraAccesorio)
        {
            if (lineaDto.Cantidad <= 0)
            {
                ModelState.AddModelError("Cantidad", "La cantidad solicitada debe ser mayor que cero.");
                continue;
            }

            var accesorio = await _context.Accesorios.FindAsync(lineaDto.AccesorioId);
            if (accesorio == null)
            {
                ModelState.AddModelError("AccesorioId", $"El accesorio con ID {lineaDto.AccesorioId} no existe en la base de datos.");
                continue;
            }

            if (lineaDto.Cantidad > accesorio.CantidadDisponible)
            {
                ModelState.AddModelError("Cantidad", $"No hay suficiente stock para el accesorio '{accesorio.Nombre}'. Stock disponible: {accesorio.CantidadDisponible}, solicitado: {lineaDto.Cantidad}.");
            }
            else
            {
                accesoriosParaComprar.Add((accesorio, lineaDto.Cantidad));
            }
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Crear la compra con los constructores que ya tienen los modelos
        var compra = new CompraAccesorio(
            0,
            DateTime.UtcNow,
            compraParaCrearDTO.NombreCliente,
            compraParaCrearDTO.ApellidosCliente,
            compraParaCrearDTO.DireccionEnvio,
            compraParaCrearDTO.NumeroTelefono,
            0m,
            compraParaCrearDTO.MetodoPago
        );
        compra.ClienteId = cliente.Id;
        compra.LineaCompraAccesorios = new List<LineaCompraAccesorio>();

        decimal precioTotal = 0m;

        foreach (var item in accesoriosParaComprar)
        {
            // Restar stock
            item.Accesorio.CantidadDisponible -= item.Cantidad;

            decimal subtotal = item.Cantidad * item.Accesorio.Precio;
            precioTotal += subtotal;

            var lineaCompra = new LineaCompraAccesorio(item.Cantidad, item.Accesorio.Precio, subtotal);
            lineaCompra.Accesorio = item.Accesorio;
            compra.LineaCompraAccesorios.Add(lineaCompra);
        }

        compra.PrecioTotal = precioTotal;

        _context.CompraAccesorios.Add(compra);
        await _context.SaveChangesAsync();

        var lineasDTO = compra.LineaCompraAccesorios.Select(l => new LineaCompraAccesorioDTO(
            l.Accesorio.Id,
            l.Accesorio.Nombre,
            l.Accesorio.CategoriaAccesorio,
            l.Accesorio.Compatibilidad,
            l.Cantidad,
            l.PrecioUnidad,
            l.Subtotal
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

        return CreatedAtAction(nameof(GetCompraAccesorio), new { id = compra.Id }, compraDetailDTO);
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