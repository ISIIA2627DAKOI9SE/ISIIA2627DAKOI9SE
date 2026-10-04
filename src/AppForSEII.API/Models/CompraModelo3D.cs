using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

public class CompraModelo3D
{
    public CompraModelo3D()
    {
    }

    public CompraModelo3D(int id,DateTime fechaCompra,string nombreCliente,string apellidosCliente,string correoElectronico,string direccionFacturacion,string descripcion,decimal precioTotal)
    {
        Id = id;
        FechaCompra = fechaCompra;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        CorreoElectronico = correoElectronico;
        DireccionFacturacion = direccionFacturacion;
        Descripcion = descripcion;
        PrecioTotal = precioTotal;
    }

    [Key]
    public int Id { get; set; }

    public DateTime FechaCompra { get; set; }

    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    public string NombreCliente { get; set; }

    [Required(ErrorMessage = "Los apellidos del cliente son obligatorios")]
    public string ApellidosCliente { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    public string CorreoElectronico { get; set; }

    [Required(ErrorMessage = "La dirección de facturación es obligatoria")]
    public string DireccionFacturacion { get; set; }

    public string? Descripcion { get; set; }

    [Precision(10, 2)]
    public decimal PrecioTotal { get; set; }
    public ICollection<LineaCompraModelo> LineasCompraModelo { get; set; }
    = new List<LineaCompraModelo>();
}