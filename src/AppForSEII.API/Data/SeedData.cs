namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider,
            ILogger logger) {

            List<string> rolesNames = new List<string> {
                "Administrator",
                "Employee",
                "Customer"
            };

            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(
                    ex,
                    "An error occurred seeding the roles in the Database."
                );
            }

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(
                    ex,
                    "An error occurred seeding the Users in the Database."
                );
            }

            try {
                SeedImpresoras3D(dbContext);
                SeedReservasImpresora(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(
                    ex,
                    "An error occurred seeding reservation data in the Database."
                );
            }
        }

        public static void SeedRoles(
            RoleManager<IdentityRole> roleManager,
            List<string> roles) {

            foreach (string roleName in roles) {

                if (!roleManager.RoleExistsAsync(roleName).Result) {
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
            List<string> roles) {

            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {

                ApplicationUser user = new ApplicationUser(
                    "1",
                    "Elena",
                    "Navarro Martínez",
                    "elena@uclm.es"
                );

                user.EmailConfirmed = true;

                var result =
                    userManager.CreateAsync(user, "Password1234%");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager
                        .AddToRoleAsync(user, roles[0])
                        .Wait();
                }
            }

            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {

                ApplicationUser user = new ApplicationUser(
                    "3",
                    "Peter",
                    "Jackson",
                    "peter@uclm.es"
                );

                user.EmailConfirmed = true;

                var result =
                    userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager
                        .AddToRoleAsync(user, roles[2])
                        .Wait();
                }
            }
        }

        public static void SeedImpresoras3D(
            ApplicationDbContext dbContext) {

            if (!dbContext.Impresora3Ds.Any()) {

                Impresora3D impresora1 = new Impresora3D {
                    Nombre = "Prusa MK4",
                    Modelo = "MK4",
                    Tipo = TipoImpresora.Filamento,
                    Descripcion =
                        "Impresora 3D de filamento para reservas",
                    PrecioKilovatioHora = 0.25m,
                    PrecioReserva = 8.00m
                };

                Impresora3D impresora2 = new Impresora3D {
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
            ApplicationDbContext dbContext) {

            if (!dbContext.ReservaImpresoras.Any()) {

                Impresora3D? impresora =
                    dbContext.Impresora3Ds.FirstOrDefault();

                if (impresora != null) {

                    LineaReserva lineaReserva = new LineaReserva {
                        TiempoReserva = TiempoReserva.DosHoras,
                        PrecioSubtotal = 16.00m,
                        Impresora3D = impresora
                    };

                    ReservaImpresora reserva =
                        new ReservaImpresora {
                            FechaReserva = DateTime.Now,
                            NombreCliente = "Peter",
                            ApellidosCliente = "Jackson",
                            DireccionFacturacion =
                                "Calle Ejemplo 1, Albacete",
                            PrecioTotal = 16.00m,
                            MetodoPago = MetodoPago.Bizum,
                            LineasReserva =
                                new List<LineaReserva> {
                                    lineaReserva
                                }
                        };

                    dbContext.ReservaImpresoras.Add(reserva);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}