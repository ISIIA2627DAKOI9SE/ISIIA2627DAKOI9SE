using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ReservaDTOs;

public class ReservaCrearDTO
{
    public ReservaCrearDTO()
    {
    }

    public ReservaCrearDTO(
        string clienteUserName,
        string nombreCliente,
        string apellidosCliente,
        string direccionFacturacion,
        MetodoPago metodoPago,
        IList<LineaReservaCrearDTO> lineasReserva)
    {
        ClienteUserName = clienteUserName;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionFacturacion = direccionFacturacion;
        MetodoPago = metodoPago;
        LineasReserva = lineasReserva;
    }

    [Required(ErrorMessage = "El usuario del cliente es obligatorio")]
    public string ClienteUserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(50)]
    public string NombreCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos del cliente son obligatorios")]
    [StringLength(50)]
    public string ApellidosCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección de facturación es obligatoria")]
    [StringLength(200)]
    public string DireccionFacturacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El método de pago es obligatorio")]
    [EnumDataType(typeof(MetodoPago),
        ErrorMessage = "El método de pago no es válido")]
    public MetodoPago? MetodoPago { get; set; }

    [Required(ErrorMessage = "Las líneas de reserva son obligatorias")]
    [MinLength(1,
        ErrorMessage = "Debes seleccionar al menos una impresora")]
    public IList<LineaReservaCrearDTO> LineasReserva { get; set; }
        = new List<LineaReservaCrearDTO>();
}