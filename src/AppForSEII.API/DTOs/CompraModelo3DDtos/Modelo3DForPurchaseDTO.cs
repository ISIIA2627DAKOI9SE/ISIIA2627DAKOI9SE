
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class Modelo3DForPurchaseDTO
{
    public Modelo3DForPurchaseDTO(
        int id,
        string nombre,
        string categoria,
        FormatoModelo3D formato,
        decimal precio,
        IList<string> licencias)
    {
        Id = id;
        Nombre = nombre;
        Categoria = categoria;
        Formato = formato;
        Precio = precio;
        Licencias = licencias;
    }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Categoria { get; set; }
    public FormatoModelo3D Formato { get; set; }
    public decimal Precio { get; set; }
    public IList<string> Licencias { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Modelo3DForPurchaseDTO dto
            && Id == dto.Id
            && Nombre == dto.Nombre
            && Categoria == dto.Categoria
            && Formato == dto.Formato
            && Precio == dto.Precio
            && Licencias.SequenceEqual(dto.Licencias);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Id, Nombre, Categoria, Formato, Precio);
    }
}
