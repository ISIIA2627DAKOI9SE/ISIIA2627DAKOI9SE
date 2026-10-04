using System;
using System.Collections.Generic;

namespace AppForSEII.API.Models;

    public enum MetodoPago
{
    TarjetaCredito,
    PayPal,
    Bizum
}
    public class EncargoImpresion
    {
    public EncargoImpresion()
    {
    }
    public EncargoImpresion(int id, DateTime fechaEncargo, string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, string? descripcion, MetodoPago metodoPago, IList<LineaEncargo> lineasEncargo)
    {
        Id = id;
        FechaEncargo = fechaEncargo;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        Descripcion = descripcion;
        MetodoPago = metodoPago;
        LineasEncargo = lineasEncargo;
    }
    [Key]
        public int Id { get; set; }

        public DateTime FechaEncargo { get; set; }
        [Required, StringLength(50)]
        public string NombreCliente { get; set; } 
        [Required, StringLength(50)]
        public string ApellidosCliente { get; set; } 
        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Dirección de envío")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Añade una dirección de envío.")]
        public string DireccionEnvio { get; set; } 
        [Required, StringLength(50)]
        public string NumeroTelefono { get; set; } 
        public string? Descripcion { get; set; }
        public decimal PrecioTotal { get; set; }
        [Required, StringLength(50)]
        public MetodoPago MetodoPago { get; set; }

        public IList<LineaEncargo> LineasEncargo { get; set; }
            = new List<LineaEncargo>();
            
    }