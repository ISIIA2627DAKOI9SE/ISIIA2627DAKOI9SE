namespace AppForSEII.API.Models;
public class Impresora3D
{
    public Impresora3D()
    {
    }
    public Impresora3D(int id, string nombre, string modelo, TipoImpresora tipo, string descripcion, decimal precioKilovatioHora, decimal precioReserva)
    {
        Id = id;
        Nombre = nombre;
        Modelo = modelo;
        Tipo = tipo;
        Descripcion = descripcion;
        PrecioKilovatioHora = precioKilovatioHora;
        PrecioReserva = precioReserva;
    }
    public int Id { get; set; }

    public string Nombre { get; set; }
    public string Modelo { get; set; }
    public TipoImpresora Tipo { get; set; }
    public string Descripcion { get; set; }
    public decimal PrecioKilovatioHora { get; set; }
    public decimal PrecioReserva { get; set; }
}