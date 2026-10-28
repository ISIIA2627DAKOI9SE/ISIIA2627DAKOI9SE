using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(CompraAccesorioId), nameof(AccesorioId))]
public class LineaCompraAccesorio
{
    public LineaCompraAccesorio()
    {
    }

    public LineaCompraAccesorio(int cantidad, decimal precioUnidad, decimal subtotal)
    {
        Cantidad = cantidad;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
    }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int Cantidad { get; set; }
    [Precision(10, 2)]
    public decimal PrecioUnidad { get; set; }
    [Precision(10, 2)]
    public decimal Subtotal { get; set; }

    [ForeignKey(nameof(CompraAccesorioId))]
    public int CompraAccesorioId { get; set; }
    public CompraAccesorio CompraAccesorio { get; set; }
    [ForeignKey(nameof(AccesorioId))]
    public int AccesorioId { get; set; }
    public Accesorio Accesorio { get; set; }
}