namespace AppForSEII.API.DTOs.CompraAccesorioDTOs;

public class CompraAccesorioParaCrearDTO
{
    public CompraAccesorioParaCrearDTO()
    {
        LineasCompraAccesorio = new List<LineaCompraAccesorioDTO>();
    }

    public CompraAccesorioParaCrearDTO(string clienteId, string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, MetodoPago metodoPago, IList<LineaCompraAccesorioDTO> lineasCompraAccesorio)
    {
        ClienteId = clienteId;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        MetodoPago = metodoPago;
        LineasCompraAccesorio = lineasCompraAccesorio ?? new List<LineaCompraAccesorioDTO>();
    }

    [Required(ErrorMessage = "El identificador del cliente es obligatorio")]
    public string ClienteId { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
    public string NombreCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos del cliente son obligatorios")]
    [StringLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres")]
    public string ApellidosCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección de envío es obligatoria")]
    [StringLength(200, ErrorMessage = "La dirección no puede tener más de 200 caracteres")]
    public string DireccionEnvio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de teléfono es obligatorio")]
    [Phone(ErrorMessage = "El número de teléfono no tiene un formato válido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
    public string NumeroTelefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El método de pago es obligatorio")]
    public MetodoPago MetodoPago { get; set; }

    public IList<LineaCompraAccesorioDTO> LineasCompraAccesorio { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is CompraAccesorioParaCrearDTO other &&
               ClienteId == other.ClienteId &&
               NombreCliente == other.NombreCliente &&
               ApellidosCliente == other.ApellidosCliente &&
               DireccionEnvio == other.DireccionEnvio &&
               NumeroTelefono == other.NumeroTelefono &&
               MetodoPago == other.MetodoPago &&
               ((LineasCompraAccesorio == null && other.LineasCompraAccesorio == null) ||
                (LineasCompraAccesorio != null && other.LineasCompraAccesorio != null && LineasCompraAccesorio.SequenceEqual(other.LineasCompraAccesorio)));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClienteId, NombreCliente, ApellidosCliente, DireccionEnvio, NumeroTelefono, MetodoPago);
    }
}
