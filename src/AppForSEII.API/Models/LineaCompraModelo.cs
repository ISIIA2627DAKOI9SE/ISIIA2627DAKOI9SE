using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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

    [Required]
    public int CompraModelo3DId { get; set; }

    [ForeignKey(nameof(CompraModelo3DId))]
    public CompraModelo3D CompraModelo3D { get; set; } = null!;

    [Required]
    public int Modelo3DId { get; set; }
    [ForeignKey(nameof(Modelo3DId))]
    public Modelo3D Modelo3D { get; set; } = null!;

}