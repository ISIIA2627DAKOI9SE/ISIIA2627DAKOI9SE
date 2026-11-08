namespace AppForSEII.API.DTOs.CompraAccesorioDTOs;

public class LineaCompraAccesorioDTO
{
    public LineaCompraAccesorioDTO()
    {
    }

    public LineaCompraAccesorioDTO(int accesorioId, int cantidad)
    {
        AccesorioId = accesorioId;
        Cantidad = cantidad;
    }

    public LineaCompraAccesorioDTO(int accesorioId, string nombre, CategoriaAccesorio categoria, string compatibilidad, int cantidad, decimal precioUnidad, decimal subtotal)
    {
        AccesorioId = accesorioId;
        Nombre = nombre;
        Categoria = categoria;
        Compatibilidad = compatibilidad;
        Cantidad = cantidad;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
    }

    public int AccesorioId { get; set; }

    [StringLength(100)]
    public string? Nombre { get; set; }

    public CategoriaAccesorio Categoria { get; set; }

    [StringLength(200)]
    public string? Compatibilidad { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int Cantidad { get; set; }

    [Precision(10, 2)]
    public decimal PrecioUnidad { get; set; }

    [Precision(10, 2)]
    public decimal Subtotal { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is LineaCompraAccesorioDTO other &&
               AccesorioId == other.AccesorioId &&
               Nombre == other.Nombre &&
               Categoria == other.Categoria &&
               Compatibilidad == other.Compatibilidad &&
               Cantidad == other.Cantidad &&
               PrecioUnidad == other.PrecioUnidad &&
               Subtotal == other.Subtotal;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AccesorioId, Nombre, Categoria, Compatibilidad, Cantidad, PrecioUnidad, Subtotal);
    }
}
