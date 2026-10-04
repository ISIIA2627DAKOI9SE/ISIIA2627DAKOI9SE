namespace AppForSEII.API.Models;

public class LineaReserva
{ 
    public int Id { get; set; }

    public TiempoReserva TiempoReserva { get; set; }

    public decimal PrecioSubtotal { get; set; }
    
}