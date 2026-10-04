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

        public int Cantidad { get; set; }

        public decimal PrecioUnidad { get; set; }

        public decimal Subtotal { get; set; }

        public Pieza3D Pieza { get; set; }

        public Material Material { get; set; }
        
    }
