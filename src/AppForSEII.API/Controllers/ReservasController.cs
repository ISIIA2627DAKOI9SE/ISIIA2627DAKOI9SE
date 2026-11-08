using AppForSEII.API.Data;
using AppForSEII.API.DTOs.ReservaDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReservasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(
        typeof(ReservaDetalleDTO),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReserva(int id)
    {
        var reserva = await _context.ReservaImpresoras
            .AsNoTracking()
            .Include(reserva => reserva.LineasReserva)
                .ThenInclude(linea => linea.Impresora3D)
            .FirstOrDefaultAsync(reserva => reserva.Id == id);

        if (reserva == null)
        {
            return NotFound();
        }

        var lineas = reserva.LineasReserva
            .OrderBy(linea => linea.Impresora3DId)
            .Select(linea => new LineaReservaDetalleDTO(
                linea.Impresora3DId,
                linea.Impresora3D.Nombre,
                linea.Impresora3D.Modelo,
                linea.Impresora3D.Tipo,
                linea.Impresora3D.Descripcion,
                linea.Impresora3D.PrecioKilovatioHora,
                linea.Impresora3D.PrecioReserva,
                linea.TiempoReserva,
                linea.PrecioSubtotal))
            .ToList();

        var detalle = new ReservaDetalleDTO(
            reserva.Id,
            reserva.FechaReserva,
            reserva.NombreCliente,
            reserva.ApellidosCliente,
            reserva.DireccionFacturacion,
            reserva.MetodoPago,
            reserva.PrecioTotal,
            lineas);

        return Ok(detalle);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(
        typeof(ReservaDetalleDTO),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(string),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateReserva(
        ReservaCrearDTO reservaCrear)
    {
        // Las validaciones de los atributos del DTO se ejecutan
        // automáticamente en las peticiones HTTP por [ApiController].
        if (!ModelState.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        if (reservaCrear.LineasReserva == null
            || reservaCrear.LineasReserva.Count == 0)
        {
            ModelState.AddModelError(
                "LineasReserva",
                "Debes seleccionar al menos una impresora.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        if (!reservaCrear.MetodoPago.HasValue
            || !Enum.IsDefined(
                typeof(MetodoPago),
                reservaCrear.MetodoPago.Value))
        {
            ModelState.AddModelError(
                "MetodoPago",
                "Debes seleccionar un método de pago válido.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        // Evitamos líneas nulas, identificadores inválidos
        // y duraciones que no pertenezcan al enum.
        foreach (var linea in reservaCrear.LineasReserva)
        {
            if (linea == null)
            {
                ModelState.AddModelError(
                    "LineasReserva",
                    "Las líneas de reserva no pueden ser nulas.");

                continue;
            }

            if (linea.Impresora3DId <= 0)
            {
                ModelState.AddModelError(
                    "LineasReserva",
                    "El identificador de la impresora debe ser válido.");
            }

            if (!Enum.IsDefined(
                typeof(TiempoReserva),
                linea.TiempoReserva))
            {
                ModelState.AddModelError(
                    "LineasReserva",
                    "El tiempo de reserva no es válido.");
            }
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var idsImpresoras = reservaCrear.LineasReserva
            .Select(linea => linea.Impresora3DId)
            .ToList();

        if (idsImpresoras.Distinct().Count() != idsImpresoras.Count)
        {
            ModelState.AddModelError(
                "LineasReserva",
                "No puedes incluir la misma impresora más de una vez.");
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(
                cliente => cliente.UserName == reservaCrear.ClienteUserName);

        if (cliente == null)
        {
            ModelState.AddModelError(
                "ClienteUserName",
                "El usuario indicado no está registrado como cliente.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var impresoras = await _context.Impresora3Ds
            .Where(impresora => idsImpresoras.Contains(impresora.Id))
            .ToListAsync();

        foreach (var id in idsImpresoras)
        {
            if (!impresoras.Any(impresora => impresora.Id == id))
            {
                ModelState.AddModelError(
                    "LineasReserva",
                    $"La impresora con identificador {id} ya no existe.");
            }
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var reserva = new ReservaImpresora(
            0,
            DateTime.Now,
            reservaCrear.NombreCliente,
            reservaCrear.ApellidosCliente,
            reservaCrear.DireccionFacturacion,
            0m,
            reservaCrear.MetodoPago!.Value);

        reserva.ClienteId = cliente!.Id;
        reserva.Cliente = cliente;

        var lineasDetalle = new List<LineaReservaDetalleDTO>();

        foreach (var item in reservaCrear.LineasReserva
            .OrderBy(linea => linea.Impresora3DId))
        {
            var impresora = impresoras.First(
                impresora => impresora.Id == item.Impresora3DId);

            int horas = item.TiempoReserva switch
            {
                TiempoReserva.UnaHora => 1,
                TiempoReserva.DosHoras => 2,
                TiempoReserva.TresHoras => 3,
                TiempoReserva.CuatroHoras => 4,
                TiempoReserva.CincoHoras => 5,
                TiempoReserva.UnDia => 24,
                TiempoReserva.DosDias => 48,
                _ => throw new InvalidOperationException(
                    "El tiempo de reserva debería haberse validado.")
            };

            // Regla acordada para el ejercicio:
            // precio fijo + coste eléctrico estimado por hora.
            // PrecioKilovatioHora se interpreta como coste por hora,
            // pues el modelo no incluye el consumo eléctrico.
            decimal subtotal = decimal.Round(
                impresora.PrecioReserva
                    + impresora.PrecioKilovatioHora * horas,
                2,
                MidpointRounding.AwayFromZero);

            var linea = new LineaReserva(
                0,
                item.TiempoReserva,
                subtotal);

            linea.Impresora3DId = impresora.Id;
            linea.Impresora3D = impresora;
            linea.ReservaImpresora = reserva;

            reserva.LineasReserva.Add(linea);

            lineasDetalle.Add(new LineaReservaDetalleDTO(
                impresora.Id,
                impresora.Nombre,
                impresora.Modelo,
                impresora.Tipo,
                impresora.Descripcion,
                impresora.PrecioKilovatioHora,
                impresora.PrecioReserva,
                item.TiempoReserva,
                subtotal));
        }

        reserva.PrecioTotal = reserva.LineasReserva
            .Sum(linea => linea.PrecioSubtotal);

        // Coincide con el límite definido en el modelo.
        if (reserva.PrecioTotal > 99999999.99m)
        {
            ModelState.AddModelError(
                "PrecioTotal",
                "El importe de la reserva supera el máximo permitido.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        _context.ReservaImpresoras.Add(reserva);

        try
        {
            // EF guarda la reserva y sus líneas en una transacción.
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(
                "No se ha podido guardar la reserva. Inténtalo de nuevo.");
        }

        var detalle = new ReservaDetalleDTO(
            reserva.Id,
            reserva.FechaReserva,
            reserva.NombreCliente,
            reserva.ApellidosCliente,
            reserva.DireccionFacturacion,
            reserva.MetodoPago,
            reserva.PrecioTotal,
            lineasDetalle);

        return CreatedAtAction(
            nameof(GetReserva),
            new { id = reserva.Id },
            detalle);
    }
}