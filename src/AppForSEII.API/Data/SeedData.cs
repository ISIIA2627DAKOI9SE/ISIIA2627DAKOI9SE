using AppForSEII.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            // Si ya hay accesorios, la base de datos ya fue inicializada
            if (context.Accesorios.Any())
            {
                return;
            }

            // 1. Accesorios
            var accesorios = new List<Accesorio>
            {
                new Accesorio
                {
                    Nombre = "Boquilla de Latón 0.4mm",
                    CategoriaAccesorio = CategoriaAccesorio.Boquillas,
                    Compatibilidad = "Creality Ender 3, Prusa i3",
                    CantidadDisponible = 150,
                    Precio = 4.50m
                },
                new Accesorio
                {
                    Nombre = "Base de Impresión PEI Magnética",
                    CategoriaAccesorio = CategoriaAccesorio.BasesImpresion,
                    Compatibilidad = "Artillery Sidewinder X1, Artillery Genius",
                    CantidadDisponible = 20,
                    Precio = 25.99m
                },
                new Accesorio
                {
                    Nombre = "Espátula de Retirada de Piezas",
                    CategoriaAccesorio = CategoriaAccesorio.Herramientas,
                    Compatibilidad = "Universal",
                    CantidadDisponible = 40,
                    Precio = 5.90m
                },
                new Accesorio
                {
                    Nombre = "Kit de Limpieza de Boquillas y Agujas",
                    CategoriaAccesorio = CategoriaAccesorio.KitsLimpieza,
                    Compatibilidad = "Universal",
                    CantidadDisponible = 45,
                    Precio = 9.99m
                },
                new Accesorio
                {
                    Nombre = "Termistor NTC 100K",
                    CategoriaAccesorio = CategoriaAccesorio.Repuestos,
                    Compatibilidad = "E3D V6 Hotend",
                    CantidadDisponible = 80,
                    Precio = 6.20m
                }
            };

            context.Accesorios.AddRange(accesorios);
            context.SaveChanges();

            // 2. Cliente de prueba
            var cliente = new Cliente
            {
                UserName = "cliente_prueba@i3dmodelos.com",
                Email = "cliente_prueba@i3dmodelos.com",
                Name = "Carlos",
                Surname = "García",
                DireccionFacturacion = "Calle Falsa 123, Ciudad"
            };

            context.Clientes.Add(cliente);
            context.SaveChanges();

            // 3. Compra de accesorios con sus líneas
            var compra = new CompraAccesorio
            {
                FechaCompra = DateTime.UtcNow,
                NombreCliente = cliente.Name!,
                ApellidosCliente = cliente.Surname!,
                DireccionEnvio = "Calle Falsa 123, Ciudad",
                NumeroTelefono = "600123456",
                MetodoPago = MetodoPago.TarjetaCredito,
                PrecioTotal = 30.49m,
                ClienteId = cliente.Id, 
                LineaCompraAccesorios = new List<LineaCompraAccesorio>
                {
                    new LineaCompraAccesorio
                    {
                        Cantidad = 1,
                        PrecioUnidad = accesorios[0].Precio,
                        Subtotal = accesorios[0].Precio,
                        Accesorio = accesorios[0]
                    },
                    new LineaCompraAccesorio
                    {
                        Cantidad = 1,
                        PrecioUnidad = accesorios[1].Precio,
                        Subtotal = accesorios[1].Precio,
                        Accesorio = accesorios[1]
                    }
                }
            };

            context.ComprasAccesorios.Add(compra);
            context.SaveChanges();
        }
    }
}