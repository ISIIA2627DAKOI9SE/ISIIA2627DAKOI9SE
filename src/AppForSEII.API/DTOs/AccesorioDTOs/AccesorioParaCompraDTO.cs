namespace AppForSEII.API.DTOs.AccesorioDTOs;

public class AccesorioParaCompraDTO
{
    public AccesorioParaCompraDTO()
    {
    }

    public AccesorioParaCompraDTO(int id, string nombre, CategoriaAccesorio categoria, string compatibilidad, int cantidadDisponible, decimal precio)
    {
        Id = id;
        Nombre = nombre;
        Categoria = categoria;
        Compatibilidad = compatibilidad;
        CantidadDisponible = cantidadDisponible;
        Precio = precio;
    }

    public AccesorioParaCompraDTO(int id, string nombre, string compatibilidad, int cantidadDisponible, decimal precio, CategoriaAccesorio categoria)
        : this(id, nombre, categoria, compatibilidad, cantidadDisponible, precio)
    {
    }

    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    public CategoriaAccesorio Categoria { get; set; }

    [Required(ErrorMessage = "La compatibilidad es obligatoria")]
    [StringLength(200, ErrorMessage = "La compatibilidad no puede tener más de 200 caracteres")]
    public string Compatibilidad { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad disponible no puede ser negativa")]
    public int CantidadDisponible { get; set; }

    [Range(0.01, 100000, ErrorMessage = "El precio debe ser mayor que 0")]
    [Precision(10, 2)]
    public decimal Precio { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is AccesorioParaCompraDTO dTO &&
               Id == dTO.Id &&
               Nombre == dTO.Nombre &&
               Categoria == dTO.Categoria &&
               Compatibilidad == dTO.Compatibilidad &&
               CantidadDisponible == dTO.CantidadDisponible &&
               Precio == dTO.Precio;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nombre, Categoria, Compatibilidad, CantidadDisponible, Precio);
    }
}
