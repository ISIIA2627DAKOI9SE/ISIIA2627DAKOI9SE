using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

public class LineaCompraModelo
{
    public LineaCompraModelo()
    {
    }

    public LineaCompraModelo(int id,int cantidadLicencias,decimal precioUnidad,decimal subtotal)
    {
        Id = id;
        CantidadLicencias = cantidadLicencias;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
    }

    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "La cantidad de licencias es obligatoria")]
    public int CantidadLicencias { get; set; }

    [Precision(10, 2)]
    public decimal PrecioUnidad { get; set; }

    [Precision(10, 2)]
    public decimal Subtotal { get; set; }
}