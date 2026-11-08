using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.AccesorioDTOs;

namespace AppForSEII.UT;

public class AccesoriosController_test : AppForSEII4SqliteUT
{
    private readonly AccesoriosController _controller;
    private readonly Mock<ILogger<AccesoriosController>> _mockLogger;

    public AccesoriosController_test()
    {
        _mockLogger = new Mock<ILogger<AccesoriosController>>();
        _controller = new AccesoriosController(_context, _mockLogger.Object);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var accesorio1 = new Accesorio(1, "Boquilla de Latón 0.4mm", CategoriaAccesorio.Boquillas, "Creality Ender 3", 10, 4.50m);
        var accesorio2 = new Accesorio(2, "Boquilla de Acero 0.6mm", CategoriaAccesorio.Boquillas, "Prusa MK4", 5, 9.95m);
        var accesorio3 = new Accesorio(3, "Base PEI Magnética", CategoriaAccesorio.BasesImpresion, "Universal", 8, 25.00m);
        var accesorio4 = new Accesorio(4, "Alicate de Corte", CategoriaAccesorio.Herramientas, "Universal", 15, 8.50m);
        var accesorio5 = new Accesorio(5, "Espátula Metálica", CategoriaAccesorio.Herramientas, "Universal", 12, 6.00m);
        var accesorioSinStock = new Accesorio(6, "Ventilador 4010", CategoriaAccesorio.Repuestos, "Creality", 0, 3.50m);

        _context.Accesorios.Add(accesorio1);
        _context.Accesorios.Add(accesorio2);
        _context.Accesorios.Add(accesorio3);
        _context.Accesorios.Add(accesorio4);
        _context.Accesorios.Add(accesorio5);
        _context.Accesorios.Add(accesorioSinStock);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAccesoriosParaCompra_ReturnsOnlyWithStockGreaterThanZero()
    {
        // Act
        var result = await _controller.GetAccesoriosParaCompra(null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualList = Assert.IsAssignableFrom<IList<AccesorioParaCompraDTO>>(okResult.Value);

        // El accesorio con stock 0 (Id 6) no debe aparecer
        var expectedList = new List<AccesorioParaCompraDTO>();
        expectedList.Add(new AccesorioParaCompraDTO(4, "Alicate de Corte", CategoriaAccesorio.Herramientas, "Universal", 15, 8.50m));
        expectedList.Add(new AccesorioParaCompraDTO(3, "Base PEI Magnética", CategoriaAccesorio.BasesImpresion, "Universal", 8, 25.00m));
        expectedList.Add(new AccesorioParaCompraDTO(2, "Boquilla de Acero 0.6mm", CategoriaAccesorio.Boquillas, "Prusa MK4", 5, 9.95m));
        expectedList.Add(new AccesorioParaCompraDTO(1, "Boquilla de Latón 0.4mm", CategoriaAccesorio.Boquillas, "Creality Ender 3", 10, 4.50m));
        expectedList.Add(new AccesorioParaCompraDTO(5, "Espátula Metálica", CategoriaAccesorio.Herramientas, "Universal", 12, 6.00m));

        Assert.Equal(expectedList, actualList);
    }

    public static IEnumerable<object?[]> TestCasesFiltros()
    {
        // Filtro por nombre "Boquilla"
        var accesoriosBoquilla = new List<AccesorioParaCompraDTO>();
        accesoriosBoquilla.Add(new AccesorioParaCompraDTO(2, "Boquilla de Acero 0.6mm", CategoriaAccesorio.Boquillas, "Prusa MK4", 5, 9.95m));
        accesoriosBoquilla.Add(new AccesorioParaCompraDTO(1, "Boquilla de Latón 0.4mm", CategoriaAccesorio.Boquillas, "Creality Ender 3", 10, 4.50m));

        // Filtro por categoría Herramientas
        var accesoriosHerramientas = new List<AccesorioParaCompraDTO>();
        accesoriosHerramientas.Add(new AccesorioParaCompraDTO(4, "Alicate de Corte", CategoriaAccesorio.Herramientas, "Universal", 15, 8.50m));
        accesoriosHerramientas.Add(new AccesorioParaCompraDTO(5, "Espátula Metálica", CategoriaAccesorio.Herramientas, "Universal", 12, 6.00m));

        // Filtro por nombre "Latón" y categoría Boquillas
        var accesoriosLatonBoquillas = new List<AccesorioParaCompraDTO>();
        accesoriosLatonBoquillas.Add(new AccesorioParaCompraDTO(1, "Boquilla de Latón 0.4mm", CategoriaAccesorio.Boquillas, "Creality Ender 3", 10, 4.50m));

        // Filtro por nombre inexistente
        var accesoriosInexistente = new List<AccesorioParaCompraDTO>();

        var allTests = new List<object?[]>();
        allTests.Add(new object?[] { "Boquilla", null, accesoriosBoquilla });
        allTests.Add(new object?[] { null, CategoriaAccesorio.Herramientas, accesoriosHerramientas });
        allTests.Add(new object?[] { "Latón", CategoriaAccesorio.Boquillas, accesoriosLatonBoquillas });
        allTests.Add(new object?[] { "Inexistente", null, accesoriosInexistente });
        return allTests;
    }

    [Theory]
    [MemberData(nameof(TestCasesFiltros))]
    public async Task GetAccesoriosParaCompra_FilterByNombreAndCategoria(string? nombreFiltro, CategoriaAccesorio? categoriaFiltro, IList<AccesorioParaCompraDTO> expectedList)
    {
        // Act
        var result = await _controller.GetAccesoriosParaCompra(nombreFiltro, categoriaFiltro);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualList = Assert.IsAssignableFrom<IList<AccesorioParaCompraDTO>>(okResult.Value);

        Assert.Equal(expectedList, actualList);
    }

    [Fact]
    public async Task GetAccesoriosParaCompra_NoStockAvailable_ReturnsEmptyList()
    {
        // Arrange: poner el stock de todos a 0
        var allAccesorios = _context.Accesorios.ToList();
        foreach (var accesorio in allAccesorios)
        {
            accesorio.CantidadDisponible = 0;
        }
        _context.SaveChanges();

        // Act
        var result = await _controller.GetAccesoriosParaCompra(null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualList = Assert.IsAssignableFrom<IList<AccesorioParaCompraDTO>>(okResult.Value);
        Assert.Empty(actualList);
    }
}
