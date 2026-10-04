namespace AppForSEII.API.Models;
    public class LineaEncargo
    {
    public LineaEncargo()
    {
    }
     public LineaEncargo(int id, int cantidad, decimal precioUnidad, decimal subtotal, Pieza3D pieza, Material material)
    {
        Id = id;
        Cantidad = cantidad;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
        Pieza = pieza;
        Material = material;
    }

    [Key]
        public int Id { get; set; }
    [Required]
        public int Cantidad { get; set; }
[Required]
        public decimal PrecioUnidad { get; set; }
[Required]
        public decimal Subtotal { get; set; }
[Required]
        public Pieza3D Pieza { get; set; }
[Required]
        public Material Material { get; set; }
        
    }
