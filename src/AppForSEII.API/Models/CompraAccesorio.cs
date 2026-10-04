using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models;

public class CompraAccesorio
{
    public CompraAccesorio()
    {
    }

    public CompraAccesorio(int id, DateTime fechaCompra, string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, decimal precioTotal, MetodoPago metodoPago)
    {
        Id = id;
        FechaCompra = fechaCompra;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
    }
    [Key]
    public int Id { get; set; }
    public DateTime FechaCompra { get; set; }
    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    public string NombreCliente { get; set; }
    [Required(ErrorMessage = "Los apellidos del cliente son obligatorios")]
    public string ApellidosCliente { get; set; }
    [Required(ErrorMessage = "La dirección de envío es obligatoria")]
    public string DireccionEnvio { get; set; }
    [Required(ErrorMessage = "El número de teléfono es obligatorio")]
    public string NumeroTelefono { get; set; }
    [Precision(10, 2)]
    public decimal PrecioTotal { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public string ClienteId { get; set; }
    public Cliente Cliente { get; set; }
    public IList<LineaCompraAccesorio> LineaCompraAccesorios { get; set; }
}