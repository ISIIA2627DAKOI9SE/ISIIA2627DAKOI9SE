using AppForSEII.API.Data;
using AppForSEII.API.DTOs.ReservaDTOs;
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
}