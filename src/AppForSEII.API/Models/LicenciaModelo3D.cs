using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models;

public class LicenciaModelo3D
{
    public LicenciaModelo3D()
    {
    }

    public LicenciaModelo3D(
        int id,
        string nombre,
        DateTime fechaExpiracion)
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

    [Required]
    public int Modelo3DId { get; set; }

    [ForeignKey(nameof(Modelo3DId))]
    public Modelo3D Modelo3D { get; set; } = null!;
}