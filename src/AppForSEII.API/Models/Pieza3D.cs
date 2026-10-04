using System.Collections.Generic;

namespace AppForSEII.API.Models;

    
    public enum CategoriaPieza
    {
    Decoracion,
    MiniaturasYMaquetas,
    Repuestos,
    HerramientasYAccesorios
    }
    public class Pieza3D
    {
    public Pieza3D()
    {
    }
    public Pieza3D(int id, string nombre, decimal peso, CategoriaPieza categoria, HashSet<Material> materialesvalidos)
    {
        Id = id;
        Nombre = nombre;
        Peso = peso;
        Categoria = categoria;
        MaterialesValidos = materialesvalidos;
    }
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public decimal Peso { get; set; }
    [Required]
    public CategoriaPieza Categoria { get; set; }
    [Required]
    public HashSet<Material> MaterialesValidos { get; set; }   
    }
