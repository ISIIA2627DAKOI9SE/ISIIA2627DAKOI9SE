using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ReservaDTOs;

public class LineaReservaCrearDTO
{
    public LineaReservaCrearDTO()
    {
    }

    public LineaReservaCrearDTO(
        int impresora3DId,
        TiempoReserva tiempoReserva)
    {
        Impresora3DId = impresora3DId;
        TiempoReserva = tiempoReserva;
    }

    [Range(1, int.MaxValue,
        ErrorMessage = "El identificador de la impresora debe ser válido")]
    public int Impresora3DId { get; set; }

    [EnumDataType(typeof(TiempoReserva),
        ErrorMessage = "El tiempo de reserva no es válido")]
    public TiempoReserva TiempoReserva { get; set; }
        = TiempoReserva.UnaHora;
}