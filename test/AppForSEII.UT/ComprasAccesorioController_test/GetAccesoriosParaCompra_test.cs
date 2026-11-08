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

    [Fact]
    public async Task CreateCompraAccesorio_Success_CreatesPurchaseAndDecreasesStock()
    {
        // Arrange
        var lineas = new List<LineaCompraAccesorioDTO>();
        lineas.Add(new LineaCompraAccesorioDTO(1, 4));  // 4 * 5.00 = 20.00
        lineas.Add(new LineaCompraAccesorioDTO(2, 2));  // 2 * 25.00 = 50.00
        // Total = 70.00

        var dto = new CompraAccesorioParaCrearDTO(
            "cliente-1",
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            MetodoPago.TarjetaCredito,
            lineas
        );

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var actualDTO = Assert.IsType<CompraAccesorioDetailDTO>(createdAtResult.Value);

        var expectedLineas = new List<LineaCompraAccesorioDTO>();
        expectedLineas.Add(new LineaCompraAccesorioDTO(1, "Boquilla de Latón 0.4mm", CategoriaAccesorio.Boquillas, "Universal", 4, 5.00m, 20.00m));
        expectedLineas.Add(new LineaCompraAccesorioDTO(2, "Base PEI Magnética", CategoriaAccesorio.BasesImpresion, "Universal", 2, 25.00m, 50.00m));

        var expectedDTO = new CompraAccesorioDetailDTO(
            actualDTO.Id,
            actualDTO.FechaCompra,
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            70.00m,
            MetodoPago.TarjetaCredito,
            "cliente-1",
            expectedLineas
        );

        Assert.Equal(expectedDTO, actualDTO);

        // El stock se ha descontado en la base de datos
        var accesorio1BD = await _context.Accesorios.FindAsync(1);
        var accesorio2BD = await _context.Accesorios.FindAsync(2);
        Assert.NotNull(accesorio1BD);
        Assert.NotNull(accesorio2BD);
        Assert.Equal(16, accesorio1BD.CantidadDisponible); // 20 - 4
        Assert.Equal(8, accesorio2BD.CantidadDisponible);  // 10 - 2
    }

    [Fact]
    public async Task CreateCompraAccesorio_NullDTO_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.CreateCompraAccesorio(null!);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task CreateCompraAccesorio_ModelInvalid_ReturnsBadRequest()
    {
        // Arrange
        var lineas = new List<LineaCompraAccesorioDTO>();
        lineas.Add(new LineaCompraAccesorioDTO(1, 1));

        var dto = new CompraAccesorioParaCrearDTO(
            "cliente-1",
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            MetodoPago.TarjetaCredito,
            lineas
        );

        // Simula que falta un campo obligatorio (FA 7.1)
        _controller.ModelState.AddModelError("NombreCliente", "El nombre del cliente es obligatorio");

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(error.ContainsKey("NombreCliente"));
    }

    [Fact]
    public async Task CreateCompraAccesorio_EmptyLines_ReturnsBadRequest()
    {
        // Arrange: carrito vacío (FA 4)
        var lineas = new List<LineaCompraAccesorioDTO>();

        var dto = new CompraAccesorioParaCrearDTO(
            "cliente-1",
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            MetodoPago.PayPal,
            lineas
        );

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(error.ContainsKey("LineasCompraAccesorio"));
    }

    [Fact]
    public async Task CreateCompraAccesorio_NullLines_ReturnsBadRequest()
    {
        // Arrange: lista de líneas null
        var dto = new CompraAccesorioParaCrearDTO(
            "cliente-1",
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            MetodoPago.PayPal,
            null!
        );

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(error.ContainsKey("LineasCompraAccesorio"));
    }

    [Fact]
    public async Task CreateCompraAccesorio_DuplicatedAccesorio_ReturnsBadRequest()
    {
        // Arrange: el accesorio 1 aparece en dos líneas
        var lineas = new List<LineaCompraAccesorioDTO>();
        lineas.Add(new LineaCompraAccesorioDTO(1, 2));
        lineas.Add(new LineaCompraAccesorioDTO(1, 3));

        var dto = new CompraAccesorioParaCrearDTO(
            "cliente-1",
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            MetodoPago.TarjetaCredito,
            lineas
        );

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        var mensajes = Assert.IsType<string[]>(error["LineasCompraAccesorio"]);
        Assert.Equal("No se puede repetir el mismo accesorio en varias líneas de la compra.", mensajes[0]);
    }

    [Fact]
    public async Task CreateCompraAccesorio_UserNotFound_ReturnsBadRequest()
    {
        // Arrange: cliente que no existe
        var lineas = new List<LineaCompraAccesorioDTO>();
        lineas.Add(new LineaCompraAccesorioDTO(1, 1));

        var dto = new CompraAccesorioParaCrearDTO(
            "cliente_inexistente",
            "Desconocido",
            "Usuario",
            "Calle Desconocida",
            "600000000",
            MetodoPago.Bizum,
            lineas
        );

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(error.ContainsKey("ClienteId"));
    }

    [Fact]
    public async Task CreateCompraAccesorio_AccesorioNotFound_ReturnsBadRequest()
    {
        // Arrange: accesorio que no existe
        var lineas = new List<LineaCompraAccesorioDTO>();
        lineas.Add(new LineaCompraAccesorioDTO(9999, 1));

        var dto = new CompraAccesorioParaCrearDTO(
            "cliente-1",
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            MetodoPago.Bizum,
            lineas
        );

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(error.ContainsKey("AccesorioId"));
    }

    [Theory]
    [InlineData(21)]
    [InlineData(50)]
    public async Task CreateCompraAccesorio_InsufficientStock_ReturnsBadRequest(int cantidadExcesiva)
    {
        // Arrange: el accesorio 1 tiene stock 20 (FA 3.1 / 7.3)
        var lineas = new List<LineaCompraAccesorioDTO>();
        lineas.Add(new LineaCompraAccesorioDTO(1, cantidadExcesiva));

        var dto = new CompraAccesorioParaCrearDTO(
            "cliente-1",
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            MetodoPago.TarjetaCredito,
            lineas
        );

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(error.ContainsKey("Cantidad"));

        // El stock no ha cambiado
        var accesorio = await _context.Accesorios.FindAsync(1);
        Assert.NotNull(accesorio);
        Assert.Equal(20, accesorio.CantidadDisponible);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task CreateCompraAccesorio_NonPositiveQuantity_ReturnsBadRequest(int cantidadInvalida)
    {
        // Arrange
        var lineas = new List<LineaCompraAccesorioDTO>();
        lineas.Add(new LineaCompraAccesorioDTO(1, cantidadInvalida));

        var dto = new CompraAccesorioParaCrearDTO(
            "cliente-1",
            "Carlos",
            "Sánchez",
            "Calle Universidad 10",
            "600123456",
            MetodoPago.TarjetaCredito,
            lineas
        );

        // Act
        var result = await _controller.CreateCompraAccesorio(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var error = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(error.ContainsKey("Cantidad"));
    }
}