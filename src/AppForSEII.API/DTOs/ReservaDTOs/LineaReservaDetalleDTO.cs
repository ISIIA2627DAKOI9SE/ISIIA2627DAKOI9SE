using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ReservaDTOs;

public class LineaReservaDetalleDTO
{
    public LineaReservaDetalleDTO()
    {
    }

    public LineaReservaDetalleDTO(
        int impresora3DId,
        string nombre,
        string modelo,
        TipoImpresora tipo,
        string descripcion,
        decimal precioKilovatioHora,
        decimal precioReserva,
        TiempoReserva tiempoReserva,
        decimal precioSubtotal)
    {
        Impresora3DId = impresora3DId;
        Nombre = nombre;
        Modelo = modelo;
        Tipo = tipo;
        Descripcion = descripcion;
        PrecioKilovatioHora = precioKilovatioHora;
        PrecioReserva = precioReserva;
        TiempoReserva = tiempoReserva;
        PrecioSubtotal = precioSubtotal;
    }

    public int Impresora3DId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public TipoImpresora Tipo { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public decimal PrecioKilovatioHora { get; set; }

    public decimal PrecioReserva { get; set; }

    public TiempoReserva TiempoReserva { get; set; }

    public decimal PrecioSubtotal { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is LineaReservaDetalleDTO otra
            && Impresora3DId == otra.Impresora3DId
            && Nombre == otra.Nombre
            && Modelo == otra.Modelo
            && Tipo == otra.Tipo
            && Descripcion == otra.Descripcion
            && PrecioKilovatioHora == otra.PrecioKilovatioHora
            && PrecioReserva == otra.PrecioReserva
            && TiempoReserva == otra.TiempoReserva
            && PrecioSubtotal == otra.PrecioSubtotal;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Impresora3DId);
        hash.Add(Nombre);
        hash.Add(Modelo);
        hash.Add(Tipo);
        hash.Add(Descripcion);
        hash.Add(PrecioKilovatioHora);
        hash.Add(PrecioReserva);
        hash.Add(TiempoReserva);
        hash.Add(PrecioSubtotal);

        return hash.ToHashCode();
    }
}