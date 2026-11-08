using System;
using System.Data.Common;
using AppForSEII.API.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.UT
{
    public class AppForSEII4SqliteUT : IDisposable
    {
        protected readonly DbConnection _connection;
        protected readonly ApplicationDbContext _context;
        protected readonly DbContextOptions<ApplicationDbContext> _contextOptions;

        protected ApplicationDbContext CreateContext()
        {
            return new ApplicationDbContext(_contextOptions);
        }

        public AppForSEII4SqliteUT()
        {
            // La conexión abierta mantiene la base de datos en memoria.
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(_connection)
                    .Options;

            // Crea las tablas a partir de los modelos del proyecto.
            _context = new ApplicationDbContext(_contextOptions);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}