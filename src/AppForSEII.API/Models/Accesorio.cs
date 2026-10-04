using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

public class Accesorio
{
    public Accesorio()
    {
    }

    public Accesorio(int id, string nombre, string compatibilidad,
        int cantidadDisponible, decimal precio)
    {
        Id = id;
        Nombre = nombre;
        Compatibilidad = compatibilidad;
        CantidadDisponible = cantidadDisponible;
        Precio = precio;
    }

    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;


    [Required(ErrorMessage = "La compatibilidad es obligatoria")]
    [StringLength(200)]
    public string Compatibilidad { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad disponible no puede ser negativa")]
    public int CantidadDisponible { get; set; }

    [Range(0.01, 100000, ErrorMessage = "El precio debe ser mayor que 0")]
    [Precision(10, 2)]
    public decimal Precio { get; set; }
}