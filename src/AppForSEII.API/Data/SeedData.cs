using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity;

namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }

            // Caso de uso: Comprar accesorios para impresión 3D
            try {
                SeedAccesorios(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Accesorios in the Database.");
            }

            try {
                SeedComprasAccesorios(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Compras de Accesorios in the Database.");
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }


            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                Cliente user = new Cliente("3", "Peter", "Jackson", "peter@uclm.es", "Calle Mayor 12, Albacete");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.Result.Succeeded) {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
            }

        }

        public static void SeedAccesorios(ApplicationDbContext dbContext) {
            // Si ya hay accesorios, no se vuelven a insertar
            if (dbContext.Accesorios.Any()) return;

            dbContext.Accesorios.AddRange(
                new Accesorio {
                    Nombre = "Boquilla de Latón 0.4mm",
                    CategoriaAccesorio = CategoriaAccesorio.Boquillas,
                    Compatibilidad = "Creality Ender 3, Prusa i3",
                    CantidadDisponible = 150,
                    Precio = 4.50m
                },
                new Accesorio {
                    Nombre = "Boquilla de Acero Endurecido 0.6mm",
                    CategoriaAccesorio = CategoriaAccesorio.Boquillas,
                    Compatibilidad = "Bambu Lab P1S, Creality K1",
                    CantidadDisponible = 30,
                    Precio = 9.95m
                },
                new Accesorio {
                    Nombre = "Base de Impresión PEI Magnética",
                    CategoriaAccesorio = CategoriaAccesorio.BasesImpresion,
                    Compatibilidad = "Artillery Sidewinder X1, Artillery Genius",
                    CantidadDisponible = 20,
                    Precio = 25.99m
                },
                new Accesorio {
                    Nombre = "Espátula de Retirada de Piezas",
                    CategoriaAccesorio = CategoriaAccesorio.Herramientas,
                    Compatibilidad = "Universal",
                    CantidadDisponible = 40,
                    Precio = 5.90m
                },
                new Accesorio {
                    Nombre = "Alicate de Corte de Precisión",
                    CategoriaAccesorio = CategoriaAccesorio.Herramientas,
                    Compatibilidad = "Universal",
                    CantidadDisponible = 25,
                    Precio = 8.75m
                },
                new Accesorio {
                    Nombre = "Kit de Limpieza de Boquillas y Agujas",
                    CategoriaAccesorio = CategoriaAccesorio.KitsLimpieza,
                    Compatibilidad = "Universal",
                    CantidadDisponible = 45,
                    Precio = 9.99m
                },
                new Accesorio {
                    Nombre = "Termistor NTC 100K",
                    CategoriaAccesorio = CategoriaAccesorio.Repuestos,
                    Compatibilidad = "E3D V6 Hotend",
                    CantidadDisponible = 80,
                    Precio = 6.20m
                },
                new Accesorio {
                    Nombre = "Ventilador de Capa 4010",
                    CategoriaAccesorio = CategoriaAccesorio.Repuestos,
                    Compatibilidad = "Creality Ender 3 V2",
                    CantidadDisponible = 0, // sin stock: sirve para probar los flujos alternativos 2.1 / 3.1 / 7.3
                    Precio = 3.80m
                }
            );

            dbContext.SaveChanges();
        }

        public static void SeedComprasAccesorios(ApplicationDbContext dbContext) {
            if (dbContext.ComprasAccesorios.Any()) return;

            // El cliente se crea en SeedUsers
            var cliente = dbContext.Clientes.FirstOrDefault(c => c.Email == "peter@uclm.es");
            if (cliente == null) return;

            var boquilla = dbContext.Accesorios.First(a => a.Nombre == "Boquilla de Latón 0.4mm");
            var base_pei = dbContext.Accesorios.First(a => a.Nombre == "Base de Impresión PEI Magnética");

            var compra = new CompraAccesorio {
                FechaCompra = DateTime.UtcNow,
                NombreCliente = cliente.Name!,
                ApellidosCliente = cliente.Surname!,
                DireccionEnvio = cliente.DireccionFacturacion,
                NumeroTelefono = "600123456",
                MetodoPago = MetodoPago.TarjetaCredito,
                ClienteId = cliente.Id,
                LineaCompraAccesorios = new List<LineaCompraAccesorio> {
                    new LineaCompraAccesorio {
                        AccesorioId = boquilla.Id,
                        Cantidad = 2,
                        PrecioUnidad = boquilla.Precio,
                        Subtotal = 2 * boquilla.Precio
                    },
                    new LineaCompraAccesorio {
                        AccesorioId = base_pei.Id,
                        Cantidad = 1,
                        PrecioUnidad = base_pei.Precio,
                        Subtotal = base_pei.Precio
                    }
                }
            };
            compra.PrecioTotal = compra.LineaCompraAccesorios.Sum(l => l.Subtotal); // 2*4.50 + 25.99 = 34.99

            // Descontar el stock vendido
            boquilla.CantidadDisponible -= 2;
            base_pei.CantidadDisponible -= 1;

            dbContext.ComprasAccesorios.Add(compra);
            dbContext.SaveChanges();
        }
    }
}