using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs.ApplicationUserDTO;

namespace AppForSEII.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);


    }


    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Modelo3D> Modelo3Ds { get; set; }

     public DbSet<Cliente> Clientes { get; set; }

    public DbSet<CompraAccesorio> ComprasAccesorios { get; set; }
    public DbSet<LicenciaModelo3D> LicenciasModelo3Ds { get; set; }

    public DbSet<LineaCompraModelo> LineasCompraModelos { get; set; }
     
     public DbSet<CompraModelo3D> ComprasModelo3Ds { get; set; }

     
     public DbSet<Impresora3D> Impresora3Ds { get; set; }

    public DbSet<LineaReserva> LineasReserva { get; set; }

    public DbSet<ReservaImpresora> ReservaImpresoras { get; set; }




}