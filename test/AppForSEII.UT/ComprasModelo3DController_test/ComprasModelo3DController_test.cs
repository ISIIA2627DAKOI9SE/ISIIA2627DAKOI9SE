
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;

namespace AppForSEII.UT;

public class ComprasModelo3DController_test : AppForSEII4SqliteUT
{
    private readonly ComprasModelo3DController _controller;
    private readonly Mock<ILogger<ComprasModelo3DController>> _mockLogger;

    public ComprasModelo3DController_test()
    {
        _mockLogger = new Mock<ILogger<ComprasModelo3DController>>();

        _controller = new ComprasModelo3DController(
            _context,
            _mockLogger.Object
        );
    }

    // Preparar modelos y licencias para las pruebas
    private void SeedDatabase()
    {
        var dragon = new Modelo3D(
            1,
            "Dragon Medieval",
            "Miniaturas",
            FormatoModelo3D.STL,
            9.99m
        );

        var torre = new Modelo3D(
            2,
            "Torre Medieval",
            "Arquitectura",
            FormatoModelo3D.OBJ,
            14.99m
        );

        var soporte = new Modelo3D(
            3,
            "Soporte Auriculares",
            "Accesorios",
            FormatoModelo3D.TresMF,
            4.99m
        );

        var fechaExpiracion = new DateTime(2027, 12, 31);

        dragon.LicenciasModelo3D.Add(
            new LicenciaModelo3D(
                1, "Licencia Personal", fechaExpiracion
            )
        );

        dragon.LicenciasModelo3D.Add(
            new LicenciaModelo3D(
                2, "Licencia Comercial", fechaExpiracion
            )
        );

        torre.LicenciasModelo3D.Add(
            new LicenciaModelo3D(
                3, "Licencia Comercial", fechaExpiracion
            )
        );

        soporte.LicenciasModelo3D.Add(
            new LicenciaModelo3D(
                4, "Licencia Profesional", fechaExpiracion
            )
        );

        _context.Modelo3Ds.AddRange(dragon, torre, soporte);
        _context.SaveChanges();
    }

    // PRUEBA 1: Obtener todos los modelos
    [Fact]
    public async Task GetModelsForPurchase_SinFiltros_DevuelveTodos()
    {
        // Arrange
        SeedDatabase();

        var esperado = new List<Modelo3DForPurchaseDTO>
        {
            new Modelo3DForPurchaseDTO(
                1, "Dragon Medieval", "Miniaturas",
                FormatoModelo3D.STL, 9.99m,
                new List<string>
                {
                    "Licencia Personal",
                    "Licencia Comercial"
                }
            ),
            new Modelo3DForPurchaseDTO(
                2, "Torre Medieval", "Arquitectura",
                FormatoModelo3D.OBJ, 14.99m,
                new List<string> { "Licencia Comercial" }
            ),
            new Modelo3DForPurchaseDTO(
                3, "Soporte Auriculares", "Accesorios",
                FormatoModelo3D.TresMF, 4.99m,
                new List<string> { "Licencia Profesional" }
            )
        };

        // Act
        var result = await _controller.GetModelsForPurchase(
            null, null
        );

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var actual = Assert.IsAssignableFrom<
            IList<Modelo3DForPurchaseDTO>>(okResult.Value);

        Assert.Equal(3, actual.Count);
        Assert.Equal(esperado, actual);
    }

    // PRUEBA 2: Filtrar por nombre
    [Fact]
    public async Task GetModelsForPurchase_FiltroNombre_DevuelveDragon()
    {
        // Arrange
        SeedDatabase();

        var esperado = new Modelo3DForPurchaseDTO(
            1, "Dragon Medieval", "Miniaturas",
            FormatoModelo3D.STL, 9.99m,
            new List<string>
            {
                "Licencia Personal",
                "Licencia Comercial"
            }
        );

        // Act
        var result = await _controller.GetModelsForPurchase(
            "Dragon", null
        );

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var modelos = Assert.IsAssignableFrom<
            IList<Modelo3DForPurchaseDTO>>(okResult.Value);

        var actual = Assert.Single(modelos);
        Assert.Equal(esperado, actual);
    }

    // PRUEBA 3: Filtrar por categoría
    [Fact]
    public async Task GetModelsForPurchase_FiltroCategoria_DevuelveTorre()
    {
        // Arrange
        SeedDatabase();

        var esperado = new Modelo3DForPurchaseDTO(
            2, "Torre Medieval", "Arquitectura",
            FormatoModelo3D.OBJ, 14.99m,
            new List<string> { "Licencia Comercial" }
        );

        // Act
        var result = await _controller.GetModelsForPurchase(
            null, "Arquitectura"
        );

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var modelos = Assert.IsAssignableFrom<
            IList<Modelo3DForPurchaseDTO>>(okResult.Value);

        var actual = Assert.Single(modelos);
        Assert.Equal(esperado, actual);
    }

    // PRUEBA 4: Combinar filtros
    [Fact]
    public async Task GetModelsForPurchase_DosFiltros_DevuelveTorre()
    {
        // Arrange
        SeedDatabase();

        var esperado = new Modelo3DForPurchaseDTO(
            2, "Torre Medieval", "Arquitectura",
            FormatoModelo3D.OBJ, 14.99m,
            new List<string> { "Licencia Comercial" }
        );

        // Act
        var result = await _controller.GetModelsForPurchase(
            "Torre", "Arquitectura"
        );

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var modelos = Assert.IsAssignableFrom<
            IList<Modelo3DForPurchaseDTO>>(okResult.Value);

        var actual = Assert.Single(modelos);
        Assert.Equal(esperado, actual);
    }

    // PRUEBA 5: Filtros sin coincidencias
    [Theory]
    [InlineData("Inexistente", null)]
    [InlineData(null, "Inexistente")]
    [InlineData("Dragon", "Arquitectura")]
    public async Task GetModelsForPurchase_SinCoincidencias_DevuelveVacio(
        string? nombre,
        string? categoria
    )
    {
        // Arrange
        SeedDatabase();

        var esperado = new List<Modelo3DForPurchaseDTO>();

        // Act
        var result = await _controller.GetModelsForPurchase(
            nombre, categoria
        );

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var actual = Assert.IsAssignableFrom<
            IList<Modelo3DForPurchaseDTO>>(okResult.Value);

        Assert.Equal(esperado, actual);
        Assert.Empty(actual);
    }

    // PRUEBA 6: Base de datos vacía
    [Fact]
    public async Task GetModelsForPurchase_BDVacía_DevuelveListaVacia()
    {
        // Arrange
        // No insertamos ningún modelo.

        var esperado = new List<Modelo3DForPurchaseDTO>();

        // Act
        var result = await _controller.GetModelsForPurchase(
            null, null
        );

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var actual = Assert.IsAssignableFrom<
            IList<Modelo3DForPurchaseDTO>>(okResult.Value);

        Assert.Equal(esperado, actual);
        Assert.Empty(actual);
    }
}
