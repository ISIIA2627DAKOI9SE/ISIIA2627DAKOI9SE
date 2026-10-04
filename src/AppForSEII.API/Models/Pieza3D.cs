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

    public int Id { get; set; }

    public string Nombre { get; set; }

    public decimal Peso { get; set; }

    public CategoriaPieza Categoria { get; set; }

    public HashSet<Material> MaterialesValidos { get; set; }   
    }
