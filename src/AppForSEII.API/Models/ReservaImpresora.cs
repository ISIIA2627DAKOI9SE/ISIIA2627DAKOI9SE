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
    public int Id { get; set; }

    public DateTime FechaReserva { get; set; }

    public string NombreCliente { get; set; }

    public string ApellidosCliente { get; set; }

    public string DireccionFacturacion { get; set; }

    public decimal PrecioTotal { get; set; }

    public MetodoPago MetodoPago { get; set; }

    public IList<LineaReserva> LineasReserva { get; set; } = new List<LineaReserva>();
}