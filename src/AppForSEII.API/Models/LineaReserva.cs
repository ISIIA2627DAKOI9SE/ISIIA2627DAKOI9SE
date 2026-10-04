namespace AppForSEII.API.Models;

public class LineaReserva
{ 
    public int Id { get; set; }

    public TiempoReserva TiempoReserva { get; set; }

    public decimal PrecioSubtotal { get; set; }
    public Impresora3D Impresora3D { get; set; }
    
}