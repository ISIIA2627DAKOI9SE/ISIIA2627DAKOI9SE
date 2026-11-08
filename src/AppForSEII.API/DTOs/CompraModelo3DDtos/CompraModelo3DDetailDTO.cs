
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class CompraModelo3DDetailDTO
{
    public CompraModelo3DDetailDTO(
        int id,
        DateTime fechaCompra,
        string nombreCliente,
        string apellidosCliente,
        string correoElectronico,
        string direccionFacturacion,
        string? descripcion,
        decimal precioTotal,
        MetodoPago metodoPago,
        IList<LineaCompraModeloDetailDTO> lineas)
    {
        Id = id;
        FechaCompra = fechaCompra;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        CorreoElectronico = correoElectronico;
        DireccionFacturacion = direccionFacturacion;
        Descripcion = descripcion;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        Lineas = lineas;
    }

    public int Id { get; set; }
    public DateTime FechaCompra { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidosCliente { get; set; }
    public string CorreoElectronico { get; set; }
    public string DireccionFacturacion { get; set; }
    public string? Descripcion { get; set; }
    public decimal PrecioTotal { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public IList<LineaCompraModeloDetailDTO> Lineas { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is CompraModelo3DDetailDTO other
            && Id == other.Id
            && FechaCompra == other.FechaCompra
            && NombreCliente == other.NombreCliente
            && ApellidosCliente == other.ApellidosCliente
            && CorreoElectronico == other.CorreoElectronico
            && DireccionFacturacion == other.DireccionFacturacion
            && Descripcion == other.Descripcion
            && PrecioTotal == other.PrecioTotal
            && MetodoPago == other.MetodoPago
            && Lineas.SequenceEqual(other.Lineas);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Id);
        hash.Add(FechaCompra);
        hash.Add(NombreCliente);
        hash.Add(ApellidosCliente);
        hash.Add(CorreoElectronico);
        hash.Add(DireccionFacturacion);
        hash.Add(Descripcion);
        hash.Add(PrecioTotal);
        hash.Add(MetodoPago);

        foreach (var linea in Lineas)
        {
            hash.Add(linea);
        }

        return hash.ToHashCode();
    }
}
