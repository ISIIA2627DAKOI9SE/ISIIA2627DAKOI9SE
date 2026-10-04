using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

public class Modelo3D
{
    public Modelo3D()
    {
    }

    public Modelo3D(int id, string nombre, string categoria, FormatoModelo3D formato, decimal precio)
    {
        Id = id;
        Nombre = nombre;
        Categoria = categoria;
        Formato = formato;
        Precio = precio;
    }

    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del modelo 3D es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La categoría del modelo 3D es obligatoria")]
    public string Categoria { get; set; }

    [Required(ErrorMessage = "El formato del modelo 3D es obligatorio")]
    public FormatoModelo3D Formato { get; set; }

    [Precision(10, 2)]
    public decimal Precio { get; set; }
    public IList<LicenciaModelo3D> LicenciasModelo3D { get; set; } = new List<LicenciaModelo3D>();
    
}