using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity;

namespace AppForSEII.API.Data
{
    public class SeedData
    {
        public static void Initialize(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider,
            ILogger logger)
        {
            List<string> rolesNames = new List<string>
            {
                "Administrator",
                "Employee",
                "Customer"
            };

            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred seeding the roles in the Database."
                );
            }

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            try
            {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred seeding the Users in the Database."
                );
            }

            // Caso de uso: Reservar impresoras 3D
            try
            {
                SeedImpresoras3D(dbContext);
                SeedReservasImpresora(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred seeding reservation data in the Database."
                );
            }

            // Caso de uso: Comprar accesorios para impresión 3D
            try
            {
                SeedAccesorios(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred seeding the Accesorios in the Database."
                );
            }

            try
            {
                SeedComprasAccesorios(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred seeding the Compras de Accesorios in the Database."
                );
            }

            // Caso de uso: Comprar modelos 3D
            try
            {
                SeedComprarModelos3D(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred seeding ComprarModelos3D data in the Database."
                );
            }
        }

        public static void SeedRoles(
            RoleManager<IdentityRole> roleManager,
            List<string> roles)
        {
            foreach (string roleName in roles)
            {
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole();

                    role.Name = roleName;
                    role.NormalizedName = roleName;

                    IdentityResult roleResult =
                        roleManager.CreateAsync(role).Result;
                }
            }
        }

        public static void SeedUsers(
            UserManager<ApplicationUser> userManager,
            List<string> roles)
        {
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null)
            {
                ApplicationUser user =
                    new ApplicationUser(
                        "1",
                        "Elena",
                        "Navarro Martínez",
                        "elena@uclm.es"
                    );

                user.EmailConfirmed = true;

                var result =
                    userManager.CreateAsync(
                        user,
                        "Password1234%"
                    );

                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
                    userManager
                        .AddToRoleAsync(user, roles[0])
                        .Wait();
                }
            }

            if (userManager.FindByNameAsync("peter@uclm.es").Result == null)
            {
                Cliente user =
                    new Cliente(
                        "3",
                        "Peter",
                        "Jackson",
                        "peter@uclm.es",
                        "Calle Mayor 12, Albacete"
                    );

                user.EmailConfirmed = true;

                var result =
                    userManager.CreateAsync(
                        user,
                        "OtherPass12$"
                    );

                result.Wait();

                if (result.Result.Succeeded)
                {
                    userManager
                        .AddToRoleAsync(user, roles[2])
                        .Wait();
                }
            }
        }

        // ==========================================================
        // UC RESERVAR IMPRESORAS 3D
        // ==========================================================

        public static void SeedImpresoras3D(
            ApplicationDbContext dbContext)
        {
            if (!dbContext.Impresora3Ds.Any())
            {
                Impresora3D impresora1 = new Impresora3D
                {
                    Nombre = "Prusa MK4",
                    Modelo = "MK4",
                    Tipo = TipoImpresora.Filamento,
                    Descripcion =
                        "Impresora 3D de filamento para reservas",
                    PrecioKilovatioHora = 0.25m,
                    PrecioReserva = 8.00m
                };

                Impresora3D impresora2 = new Impresora3D
                {
                    Nombre = "Elegoo Mars",
                    Modelo = "Mars 4",
                    Tipo = TipoImpresora.Resina,
                    Descripcion =
                        "Impresora 3D de resina para piezas de alta precisión",
                    PrecioKilovatioHora = 0.30m,
                    PrecioReserva = 10.00m
                };

                dbContext.Impresora3Ds.AddRange(
                    impresora1,
                    impresora2
                );

                dbContext.SaveChanges();
            }
        }

        public static void SeedReservasImpresora(
            ApplicationDbContext dbContext)
        {
            if (!dbContext.ReservaImpresoras.Any())
            {
                Impresora3D? impresora =
                    dbContext.Impresora3Ds.FirstOrDefault();

                if (impresora != null)
                {
                    LineaReserva lineaReserva = new LineaReserva
                    {
                        TiempoReserva = TiempoReserva.DosHoras,
                        PrecioSubtotal = 16.00m,
                        Impresora3D = impresora
                    };

                    ReservaImpresora reserva =
                        new ReservaImpresora
                        {
                            FechaReserva = DateTime.Now,
                            NombreCliente = "Peter",
                            ApellidosCliente = "Jackson",
                            DireccionFacturacion =
                                "Calle Ejemplo 1, Albacete",
                            PrecioTotal = 16.00m,
                            MetodoPago = MetodoPago.Bizum,
                            LineasReserva =
                                new List<LineaReserva>
                                {
                                    lineaReserva
                                }
                        };

                    dbContext.ReservaImpresoras.Add(reserva);
                    dbContext.SaveChanges();
                }
            }
        }

        // ==========================================================
        // UC COMPRAR ACCESORIOS PARA IMPRESIÓN 3D
        // ==========================================================

        public static void SeedAccesorios(ApplicationDbContext dbContext)
        {
            if (dbContext.Accesorios.Any())
            {
                return;
            }

            dbContext.Accesorios.AddRange(
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
                    Nombre = "Boquilla de Acero Endurecido 0.6mm",
                    CategoriaAccesorio = CategoriaAccesorio.Boquillas,
                    Compatibilidad = "Bambu Lab P1S, Creality K1",
                    CantidadDisponible = 30,
                    Precio = 9.95m
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
                    Nombre = "Alicate de Corte de Precisión",
                    CategoriaAccesorio = CategoriaAccesorio.Herramientas,
                    Compatibilidad = "Universal",
                    CantidadDisponible = 25,
                    Precio = 8.75m
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
                },
                new Accesorio
                {
                    Nombre = "Ventilador de Capa 4010",
                    CategoriaAccesorio = CategoriaAccesorio.Repuestos,
                    Compatibilidad = "Creality Ender 3 V2",
                    CantidadDisponible = 0,
                    Precio = 3.80m
                }
            );

            dbContext.SaveChanges();
        }

        public static void SeedComprasAccesorios(ApplicationDbContext dbContext)
        {
            if (dbContext.ComprasAccesorios.Any())
            {
                return;
            }

            var cliente =
                dbContext.Clientes.FirstOrDefault(
                    c => c.Email == "peter@uclm.es"
                );

            if (cliente == null)
            {
                return;
            }

            var boquilla =
                dbContext.Accesorios.First(
                    a => a.Nombre == "Boquilla de Latón 0.4mm"
                );

            var basePei =
                dbContext.Accesorios.First(
                    a => a.Nombre == "Base de Impresión PEI Magnética"
                );

            var compra = new CompraAccesorio
            {
                FechaCompra = DateTime.UtcNow,
                NombreCliente = cliente.Name!,
                ApellidosCliente = cliente.Surname!,
                DireccionEnvio = cliente.DireccionFacturacion,
                NumeroTelefono = "600123456",
                MetodoPago = MetodoPago.TarjetaCredito,
                ClienteId = cliente.Id,
                LineaCompraAccesorios =
                    new List<LineaCompraAccesorio>
                    {
                        new LineaCompraAccesorio
                        {
                            AccesorioId = boquilla.Id,
                            Cantidad = 2,
                            PrecioUnidad = boquilla.Precio,
                            Subtotal = 2 * boquilla.Precio
                        },
                        new LineaCompraAccesorio
                        {
                            AccesorioId = basePei.Id,
                            Cantidad = 1,
                            PrecioUnidad = basePei.Precio,
                            Subtotal = basePei.Precio
                        }
                    }
            };

            compra.PrecioTotal =
                compra.LineaCompraAccesorios.Sum(l => l.Subtotal);

            boquilla.CantidadDisponible -= 2;
            basePei.CantidadDisponible -= 1;

            dbContext.ComprasAccesorios.Add(compra);
            dbContext.SaveChanges();
        }

        // ==========================================================
        // UC COMPRAR MODELOS 3D
        // ==========================================================

        public static void SeedComprarModelos3D(
            ApplicationDbContext dbContext)
        {
            if (dbContext.Modelo3Ds.Any() ||
                dbContext.LicenciasModelo3Ds.Any() ||
                dbContext.LineasCompraModelos.Any() ||
                dbContext.ComprasModelo3Ds.Any())
            {
                return;
            }

            LicenciaModelo3D licenciaPersonal =
                new LicenciaModelo3D(
                    0,
                    "Licencia Personal",
                    new DateTime(2027, 6, 30)
                );

            LicenciaModelo3D licenciaComercial =
                new LicenciaModelo3D(
                    0,
                    "Licencia Comercial",
                    new DateTime(2028, 6, 30)
                );

            LicenciaModelo3D licenciaProfesional =
                new LicenciaModelo3D(
                    0,
                    "Licencia Profesional",
                    new DateTime(2029, 6, 30)
                );

            Modelo3D dragonMedieval =
                new Modelo3D(
                    0,
                    "Dragon Medieval",
                    "Miniaturas",
                    FormatoModelo3D.STL,
                    9.99m
                );

            Modelo3D torreMedieval =
                new Modelo3D(
                    0,
                    "Torre Medieval",
                    "Arquitectura",
                    FormatoModelo3D.OBJ,
                    14.99m
                );

            Modelo3D soporteAuriculares =
                new Modelo3D(
                    0,
                    "Soporte Auriculares",
                    "Accesorios",
                    FormatoModelo3D.TresMF,
                    4.99m
                );

            dragonMedieval.LicenciasModelo3D.Add(
                licenciaPersonal
            );

            dragonMedieval.LicenciasModelo3D.Add(
                licenciaComercial
            );

            torreMedieval.LicenciasModelo3D.Add(
                licenciaComercial
            );

            soporteAuriculares.LicenciasModelo3D.Add(
                licenciaProfesional
            );

            LineaCompraModelo linea1 =
                new LineaCompraModelo(
                    0,
                    2,
                    9.99m,
                    19.98m
                );

            linea1.Modelo3D = dragonMedieval;

            LineaCompraModelo linea2 =
                new LineaCompraModelo(
                    0,
                    1,
                    14.99m,
                    14.99m
                );

            linea2.Modelo3D = torreMedieval;

            LineaCompraModelo linea3 =
                new LineaCompraModelo(
                    0,
                    1,
                    4.99m,
                    4.99m
                );

            linea3.Modelo3D = soporteAuriculares;

            CompraModelo3D compra1 =
                new CompraModelo3D(
                    0,
                    new DateTime(2026, 10, 1),
                    "Peter",
                    "Jackson",
                    "peter@uclm.es",
                    "Calle Mayor 10",
                    "Compra de modelos para uso personal",
                    34.97m,
                    MetodoPago.PayPal
                );

            compra1.LineasCompraModelo.Add(
                linea1
            );

            compra1.LineasCompraModelo.Add(
                linea2
            );

            CompraModelo3D compra2 =
                new CompraModelo3D(
                    0,
                    new DateTime(2026, 10, 2),
                    "Ana",
                    "Martinez",
                    "ana@example.com",
                    "Avenida España 25",
                    null,
                    4.99m,
                    MetodoPago.TarjetaCredito
                );

            compra2.LineasCompraModelo.Add(
                linea3
            );

            dbContext.ComprasModelo3Ds.AddRange(
                compra1,
                compra2
            );

            dbContext.SaveChanges();
        }
    }
}