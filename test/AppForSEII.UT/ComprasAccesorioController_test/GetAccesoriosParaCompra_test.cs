using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraAccesorioDTOs;

namespace AppForSEII.UT;

public class ComprasAccesoriosController_test : AppForSEII4SqliteUT
{
    private readonly ComprasAccesoriosController _controller;
    private readonly Mock<ILogger<ComprasAccesoriosController>> _mockLogger;

    public ComprasAccesoriosController_test()
    {
        _mockLogger = new Mock<ILogger<ComprasAccesoriosController>>();
        _controller = new ComprasAccesoriosController(_context, _mockLogger.Object);
        SeedDatabase();
    }

        private void SeedDatabase()
    {
        var cliente = new Cliente("cliente-1", "Carlos", "Sánchez", "carlos@uclm.es", "Calle Universidad 10");
        _context.Clientes.Add(cliente);

        var accesorio1 = new Accesorio(1, "Boquilla de Latón 0.4mm", CategoriaAccesorio.Boquillas, "Universal", 20, 5.00m);
        var accesorio2 = new Accesorio(2, "Base PEI Magnética", CategoriaAccesorio.BasesImpresion, "Universal", 10, 25.00m);
        var accesorio3 = new Accesorio(3, "Espátula Metálica", CategoriaAccesorio.Herramientas, "Universal", 5, 8.00m);

        _context.Accesorios.Add(accesorio1);
        _context.Accesorios.Add(accesorio2);
        _context.Accesorios.Add(accesorio3);

        // Sembrar una compra existente para las pruebas de Details
        var fechaExistente = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);

        var compraExistente = new CompraAccesorio(
            1,
            fechaExistente,
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            10.00m,
            MetodoPago.Bizum
        );
        compraExistente.ClienteId = "cliente-1";
        compraExistente.LineaCompraAccesorios = new List<LineaCompraAccesorio>();

        var lineaExistente = new LineaCompraAccesorio(2, 5.00m, 10.00m);
        lineaExistente.Accesorio = accesorio1;
        compraExistente.LineaCompraAccesorios.Add(lineaExistente);

        _context.CompraAccesorios.Add(compraExistente);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetCompraAccesorio_Success_ReturnsDetailsDTO()
    {
        // Act
        var result = await _controller.GetCompraAccesorio(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualDTO = Assert.IsType<CompraAccesorioDetailDTO>(okResult.Value);

        var expectedLineas = new List<LineaCompraAccesorioDTO>();
        expectedLineas.Add(new LineaCompraAccesorioDTO(
            1,
            "Boquilla de Latón 0.4mm",
            CategoriaAccesorio.Boquillas,
            "Universal",
            2,
            5.00m,
            10.00m
        ));

        var expectedDTO = new CompraAccesorioDetailDTO(
            1,
            new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc),
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            10.00m,
            MetodoPago.Bizum,
            "cliente-1",
            expectedLineas
        );

        Assert.Equal(expectedDTO, actualDTO);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(9999)]
    public async Task GetCompraAccesorio_NotFound_ReturnsNotFound(int compraIdInexistente)
    {
        // Act
        var result = await _controller.GetCompraAccesorio(compraIdInexistente);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}