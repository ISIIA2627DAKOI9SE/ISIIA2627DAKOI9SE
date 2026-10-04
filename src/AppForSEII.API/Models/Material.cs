using System;
using System.Collections.Generic;

namespace AppForSEII.API.Models;

public class Material
{
    public Material()
    {
    }
    public Material(int id, string nombre, decimal precioPorGramo, decimal stockGramos)
    {
        Id = id;
        Nombre = nombre;
        PrecioPorGramo = precioPorGramo;
        StockGramos = stockGramos;
    }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal PrecioPorGramo { get; set; }
    public decimal StockGramos { get; set; }
    
}