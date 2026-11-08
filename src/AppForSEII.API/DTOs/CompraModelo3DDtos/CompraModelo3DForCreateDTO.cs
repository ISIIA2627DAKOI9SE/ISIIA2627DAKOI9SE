
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class CompraModelo3DForCreateDTO
{
    public CompraModelo3DForCreateDTO(
        string nombreCliente,
        string apellidosCliente,
        string correoElectronico,
        string direccionFacturacion,
        string? descripcion,
        MetodoPago metodoPago,
        IList<LineaCompraModeloForCreateDTO> lineas)
    {
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        CorreoElectronico = correoElectronico;
        DireccionFacturacion = direccionFacturacion;
        Descripcion = descripcion;
        MetodoPago = metodoPago;
        Lineas = lineas;
    }

    public string NombreCliente { get; set; }

    public string ApellidosCliente { get; set; }

    public string CorreoElectronico { get; set; }

    public string DireccionFacturacion { get; set; }

    public string? Descripcion { get; set; }

    public MetodoPago MetodoPago { get; set; }

    public IList<LineaCompraModeloForCreateDTO> Lineas { get; set; }
}
