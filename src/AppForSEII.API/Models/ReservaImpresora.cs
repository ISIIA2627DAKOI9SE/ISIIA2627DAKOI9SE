namespace AppForSEII.API.Models;

public class ReservaImpresora
{
    public ReservaImpresora()
    {
    }
    public ReservaImpresora(int id, DateTime fechaReserva, string nombreCliente, string apellidosCliente, string direccionFacturacion, decimal precioTotal, MetodoPago metodoPago, IList<LineaReserva> lineasReserva)
    {
        Id = id;
        FechaReserva = fechaReserva;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionFacturacion = direccionFacturacion;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        LineasReserva = lineasReserva;
    }
[Key]
    public int Id { get; set; }

    public DateTime FechaReserva { get; set; }

    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(50)]
    public string NombreCliente { get; set; } 

    [Required(ErrorMessage = "Los apellidos del cliente son obligatorios")]
    [StringLength(50)]
    public string ApellidosCliente { get; set; } 

    [Required(ErrorMessage = "La dirección de facturación es obligatoria")]
    [StringLength(200)]
    public string DireccionFacturacion { get; set; } 

    [Range(0, 99999999.99,
        ErrorMessage = "El precio total no puede ser negativo")]
    [Precision(10, 2)]
    public decimal PrecioTotal { get; set; }

    
    public MetodoPago MetodoPago { get; set; }

    public string ClienteId { get; set; } 
    public Cliente Cliente { get; set; }

    public IList<LineaReserva> LineasReserva { get; set; }
}