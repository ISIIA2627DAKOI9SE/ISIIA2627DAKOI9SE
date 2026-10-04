namespace AppForSEII.API.Models;
public class Impresora3D
{
    public int Id { get; set; }

    public string Nombre { get; set; }
    public string Modelo { get; set; }
    public TipoImpresora Tipo { get; set; }
    public string Descripcion { get; set; }
    public decimal PrecioKilovatioHora { get; set; }
    public decimal PrecioReserva { get; set; }
}