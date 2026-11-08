using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ReservaDTOs;

public class ReservaDetalleDTO
{
    public ReservaDetalleDTO()
    {
    }

    public ReservaDetalleDTO(
        int id,
        DateTime fechaReserva,
        string nombreCliente,
        string apellidosCliente,
        string direccionFacturacion,
        MetodoPago metodoPago,
        decimal precioTotal,
        IList<LineaReservaDetalleDTO> lineasReserva)
    {
        Id = id;
        FechaReserva = fechaReserva;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionFacturacion = direccionFacturacion;
        MetodoPago = metodoPago;
        PrecioTotal = precioTotal;
        LineasReserva = lineasReserva;
    }

    public int Id { get; set; }

    public DateTime FechaReserva { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public string ApellidosCliente { get; set; } = string.Empty;

    public string DireccionFacturacion { get; set; } = string.Empty;

    public MetodoPago MetodoPago { get; set; }

    public decimal PrecioTotal { get; set; }

    public IList<LineaReservaDetalleDTO> LineasReserva { get; set; }
        = new List<LineaReservaDetalleDTO>();

    public override bool Equals(object? obj)
    {
        return obj is ReservaDetalleDTO otra
            && Id == otra.Id
            && FechaReserva == otra.FechaReserva
            && NombreCliente == otra.NombreCliente
            && ApellidosCliente == otra.ApellidosCliente
            && DireccionFacturacion == otra.DireccionFacturacion
            && MetodoPago == otra.MetodoPago
            && PrecioTotal == otra.PrecioTotal
            && LineasReserva.SequenceEqual(otra.LineasReserva);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Id);
        hash.Add(FechaReserva);
        hash.Add(NombreCliente);
        hash.Add(ApellidosCliente);
        hash.Add(DireccionFacturacion);
        hash.Add(MetodoPago);
        hash.Add(PrecioTotal);

        foreach (var linea in LineasReserva)
        {
            hash.Add(linea);
        }

        return hash.ToHashCode();
    }
}