using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

public class LineaReserva
{
    public LineaReserva()
    {
    }

    public LineaReserva(int id, TiempoReserva tiempoReserva,
        decimal precioSubtotal)
    {
        Id = id;
        TiempoReserva = tiempoReserva;
        PrecioSubtotal = precioSubtotal;
    }

    [Key]
    public int Id { get; set; }

    [EnumDataType(typeof(TiempoReserva))]
    public TiempoReserva TiempoReserva { get; set; }
        = TiempoReserva.UnaHora;

    [Range(0, 99999999.99,
        ErrorMessage = "El precio subtotal no puede ser negativo")]
    [Precision(10, 2)]
    public decimal PrecioSubtotal { get; set; }

    public int ReservaImpresoraId { get; set; }
    public ReservaImpresora ReservaImpresora { get; set; } 

    public int Impresora3DId { get; set; }
    public Impresora3D Impresora3D { get; set; } 
}