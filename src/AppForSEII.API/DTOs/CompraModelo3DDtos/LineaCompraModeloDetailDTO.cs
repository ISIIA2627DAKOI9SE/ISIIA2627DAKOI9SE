
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class LineaCompraModeloDetailDTO
{
    public LineaCompraModeloDetailDTO(
        int modelo3DId,
        string nombre,
        string categoria,
        FormatoModelo3D formato,
        int cantidadLicencias,
        decimal precioUnidad,
        decimal subtotal)
    {
        Modelo3DId = modelo3DId;
        Nombre = nombre;
        Categoria = categoria;
        Formato = formato;
        CantidadLicencias = cantidadLicencias;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
    }

    public int Modelo3DId { get; set; }
    public string Nombre { get; set; }
    public string Categoria { get; set; }
    public FormatoModelo3D Formato { get; set; }
    public int CantidadLicencias { get; set; }
    public decimal PrecioUnidad { get; set; }
    public decimal Subtotal { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is LineaCompraModeloDetailDTO other
            && Modelo3DId == other.Modelo3DId
            && Nombre == other.Nombre
            && Categoria == other.Categoria
            && Formato == other.Formato
            && CantidadLicencias == other.CantidadLicencias
            && PrecioUnidad == other.PrecioUnidad
            && Subtotal == other.Subtotal;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Modelo3DId,
            Nombre,
            Categoria,
            Formato,
            CantidadLicencias,
            PrecioUnidad,
            Subtotal
        );
    }
}
