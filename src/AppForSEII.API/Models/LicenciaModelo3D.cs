using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

public class LicenciaModelo3D
{
    public LicenciaModelo3D()
    {
    }

    public LicenciaModelo3D(int id, string nombre, DateTime fechaExpiracion)
    {
        Id = id;
        Nombre = nombre;
        FechaExpiracion = fechaExpiracion;
    }

    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la licencia es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La fecha de expiración es obligatoria")]
    public DateTime FechaExpiracion { get; set; }
}