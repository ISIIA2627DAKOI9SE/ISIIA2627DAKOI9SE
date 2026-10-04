using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models;

public class LineaCompraAccesorio
{
    public LineaCompraAccesorio()
    {
    }

    public LineaCompraAccesorio(int id, int cantidad, decimal precioUnidad, decimal subtotal)
    {
        Id = id;
        Cantidad = cantidad;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
    }
    [Key]
    public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int Cantidad { get; set; }
    [Precision(10, 2)]
    public decimal PrecioUnidad { get; set; }
    [Precision(10, 2)]
    public decimal Subtotal { get; set; }

    public int CompraAccesorioId { get; set; }
    public CompraAccesorio CompraAccesorio { get; set; }
    public int AccesorioId { get; set; }
    public Accesorio Accesorio { get; set; }
}