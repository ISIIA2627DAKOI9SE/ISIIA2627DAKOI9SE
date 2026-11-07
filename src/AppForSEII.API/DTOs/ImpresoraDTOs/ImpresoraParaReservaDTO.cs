using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ImpresoraDTOs;

public class ImpresoraParaReservaDTO
{
    public ImpresoraParaReservaDTO()
    {
    }

    public ImpresoraParaReservaDTO(
        int id,
        string nombre,
        string modelo,
        TipoImpresora tipo,
        string descripcion,
        decimal precioKilovatioHora,
        decimal precioReserva)
    {
        Id = id;
        Nombre = nombre;
        Modelo = modelo;
        Tipo = tipo;
        Descripcion = descripcion;
        PrecioKilovatioHora = precioKilovatioHora;
        PrecioReserva = precioReserva;
    }

    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public TipoImpresora Tipo { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public decimal PrecioKilovatioHora { get; set; }

    public decimal PrecioReserva { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ImpresoraParaReservaDTO otra
            && Id == otra.Id
            && Nombre == otra.Nombre
            && Modelo == otra.Modelo
            && Tipo == otra.Tipo
            && Descripcion == otra.Descripcion
            && PrecioKilovatioHora == otra.PrecioKilovatioHora
            && PrecioReserva == otra.PrecioReserva;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Id,
            Nombre,
            Modelo,
            Tipo,
            Descripcion,
            PrecioKilovatioHora,
            PrecioReserva);
    }
}