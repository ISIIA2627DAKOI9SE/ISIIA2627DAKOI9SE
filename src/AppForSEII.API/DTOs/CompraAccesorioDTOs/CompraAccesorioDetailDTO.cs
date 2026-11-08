namespace AppForSEII.API.DTOs.CompraAccesorioDTOs;

public class CompraAccesorioDetailDTO
{
    public CompraAccesorioDetailDTO()
    {
        LineasCompraAccesorio = new List<LineaCompraAccesorioDTO>();
    }

    public CompraAccesorioDetailDTO(int id, DateTime fechaCompra, string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, decimal precioTotal, MetodoPago metodoPago, string clienteId, IList<LineaCompraAccesorioDTO> lineasCompraAccesorio)
    {
        Id = id;
        FechaCompra = fechaCompra;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        ClienteId = clienteId;
        LineasCompraAccesorio = lineasCompraAccesorio ?? new List<LineaCompraAccesorioDTO>();
    }

    public int Id { get; set; }

    public DateTime FechaCompra { get; set; }

    public string? ClienteId { get; set; }

    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(50)]
    public string NombreCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos del cliente son obligatorios")]
    [StringLength(100)]
    public string ApellidosCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección de envío es obligatoria")]
    [StringLength(200)]
    public string DireccionEnvio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de teléfono es obligatorio")]
    [Phone]
    [StringLength(20)]
    public string NumeroTelefono { get; set; } = string.Empty;

    [Precision(10, 2)]
    public decimal PrecioTotal { get; set; }

    public MetodoPago MetodoPago { get; set; }

    public IList<LineaCompraAccesorioDTO> LineasCompraAccesorio { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is CompraAccesorioDetailDTO other &&
               Id == other.Id &&
               FechaCompra == other.FechaCompra &&
               NombreCliente == other.NombreCliente &&
               ApellidosCliente == other.ApellidosCliente &&
               DireccionEnvio == other.DireccionEnvio &&
               NumeroTelefono == other.NumeroTelefono &&
               PrecioTotal == other.PrecioTotal &&
               MetodoPago == other.MetodoPago &&
               ClienteId == other.ClienteId &&
               ((LineasCompraAccesorio == null && other.LineasCompraAccesorio == null) ||
                (LineasCompraAccesorio != null && other.LineasCompraAccesorio != null && LineasCompraAccesorio.SequenceEqual(other.LineasCompraAccesorio)));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, FechaCompra, ClienteId, NombreCliente, ApellidosCliente, DireccionEnvio, NumeroTelefono, PrecioTotal);
    }
}
