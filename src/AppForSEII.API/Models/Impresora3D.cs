using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace AppForSEII.API.Models;
public class Impresora3D
{
    public Impresora3D()
    {
    }
    public Impresora3D(int id, string nombre, string modelo, TipoImpresora tipo, string descripcion, decimal precioKilovatioHora, decimal precioReserva)
    {
        Id = id;
        Nombre = nombre;
        Modelo = modelo;
        Tipo = tipo;
        Descripcion = descripcion;
        PrecioKilovatioHora = precioKilovatioHora;
        PrecioReserva = precioReserva;
    }
 [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la impresora es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo de la impresora es obligatorio")]
    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [EnumDataType(typeof(TipoImpresora))]
    public TipoImpresora Tipo { get; set; }

    [Required(ErrorMessage = "La descripción de la impresora es obligatoria")]
    [StringLength(1000)]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0, 99999999.99,
        ErrorMessage = "El precio por kilovatio/hora no puede ser negativo")]
    [Precision(10, 2)]
    public decimal PrecioKilovatioHora { get; set; }

    [Range(0, 99999999.99,
        ErrorMessage = "El precio de reserva no puede ser negativo")]
    [Precision(10, 2)]
    public decimal PrecioReserva { get; set; }

    public IList<LineaReserva> LineasReserva { get; set; }
}