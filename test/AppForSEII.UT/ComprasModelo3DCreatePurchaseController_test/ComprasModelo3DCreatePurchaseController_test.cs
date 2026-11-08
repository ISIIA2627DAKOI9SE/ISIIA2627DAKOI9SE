
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT;

public class ComprasModelo3DCreatePurchaseController_test
    : AppForSEII4SqliteUT
{
    private readonly ComprasModelo3DController _controller;
    private readonly Mock<ILogger<ComprasModelo3DController>> _mockLogger;

    public ComprasModelo3DCreatePurchaseController_test()
    {
        _mockLogger =
            new Mock<ILogger<ComprasModelo3DController>>();

        _controller = new ComprasModelo3DController(
            _context,
            _mockLogger.Object
        );

        SeedDatabase();
    }

    // Preparar los modelos disponibles para las pruebas.
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

        _context.Modelo3Ds.AddRange(dragon, torre);
        _context.SaveChanges();
    }

    // Crear una compra válida para reutilizarla.
    private CompraModelo3DForCreateDTO CreateValidDTO(
        string nombre = "Carlos",
        string apellidos = "Sanchez",
        string correo = "carlos@uclm.es",
        string direccion = "Calle Universidad 10",
        MetodoPago metodoPago = MetodoPago.PayPal,
        IList<LineaCompraModeloForCreateDTO>? lineas = null)
    {
        lineas ??= new List<LineaCompraModeloForCreateDTO>
        {
            new LineaCompraModeloForCreateDTO(1, 2),
            new LineaCompraModeloForCreateDTO(2, 1)
        };

        return new CompraModelo3DForCreateDTO(
            nombre,
            apellidos,
            correo,
            direccion,
            "Compra de prueba",
            metodoPago,
            lineas
        );
    }

    // PRUEBA 1: Crear una compra correctamente.
    [Fact]
    public async Task CreatePurchase_CompraValida_DevuelveCreated()
    {
        // Arrange
        var compraDTO = CreateValidDTO();

        // Act
        var result = await _controller.CreatePurchase(compraDTO);

        // Assert
        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result);

        var actualDTO =
            Assert.IsType<CompraModelo3DDetailDTO>(
                createdResult.Value
            );

        Assert.Equal(201, createdResult.StatusCode);

        Assert.Equal(
            nameof(ComprasModelo3DController.GetPurchase),
            createdResult.ActionName
        );

        Assert.True(actualDTO.Id > 0);
        Assert.Equal("Carlos", actualDTO.NombreCliente);
        Assert.Equal("Sanchez", actualDTO.ApellidosCliente);
        Assert.Equal("carlos@uclm.es", actualDTO.CorreoElectronico);
        Assert.Equal(34.97m, actualDTO.PrecioTotal);
        Assert.Equal(MetodoPago.PayPal, actualDTO.MetodoPago);

        Assert.Equal(2, actualDTO.Lineas.Count);

        Assert.Equal(2, actualDTO.Lineas[0].CantidadLicencias);
        Assert.Equal(19.98m, actualDTO.Lineas[0].Subtotal);

        Assert.Equal(1, actualDTO.Lineas[1].CantidadLicencias);
        Assert.Equal(14.99m, actualDTO.Lineas[1].Subtotal);

        Assert.Equal(1, await _context.ComprasModelo3Ds.CountAsync());
    }

    // PRUEBA 2: El cuerpo de la petición es nulo.
    [Fact]
    public async Task CreatePurchase_CompraNula_DevuelveBadRequest()
    {
        // Arrange
        CompraModelo3DForCreateDTO? compraDTO = null;

        // Act
        var result = await _controller.CreatePurchase(compraDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(_context.ComprasModelo3Ds);
    }

    // PRUEBA 3: Faltan datos obligatorios del cliente.
    [Fact]
    public async Task CreatePurchase_DatosObligatoriosVacios_DevuelveBadRequest()
    {
        // Arrange
        var compraDTO = CreateValidDTO(nombre: "");

        // Act
        var result = await _controller.CreatePurchase(compraDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(_context.ComprasModelo3Ds);
    }

    // PRUEBA 4: El carrito está vacío.
    [Fact]
    public async Task CreatePurchase_CarritoVacio_DevuelveBadRequest()
    {
        // Arrange
        var compraDTO = CreateValidDTO(
            lineas: new List<LineaCompraModeloForCreateDTO>()
        );

        // Act
        var result = await _controller.CreatePurchase(compraDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(_context.ComprasModelo3Ds);
    }

    // PRUEBA 5: Las cantidades deben ser positivas.
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreatePurchase_CantidadInvalida_DevuelveBadRequest(
        int cantidad)
    {
        // Arrange
        var lineas = new List<LineaCompraModeloForCreateDTO>
        {
            new LineaCompraModeloForCreateDTO(1, cantidad)
        };

        var compraDTO = CreateValidDTO(lineas: lineas);

        // Act
        var result = await _controller.CreatePurchase(compraDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(_context.ComprasModelo3Ds);
    }

    // PRUEBA 6: El modelo solicitado no existe.
    [Fact]
    public async Task CreatePurchase_ModeloInexistente_DevuelveBadRequest()
    {
        // Arrange
        var lineas = new List<LineaCompraModeloForCreateDTO>
        {
            new LineaCompraModeloForCreateDTO(9999, 1)
        };

        var compraDTO = CreateValidDTO(lineas: lineas);

        // Act
        var result = await _controller.CreatePurchase(compraDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(_context.ComprasModelo3Ds);
    }

    // PRUEBA 7: Método de pago no permitido.
    [Theory]
    [InlineData(MetodoPago.Bizum)]
    [InlineData((MetodoPago)999)]
    public async Task CreatePurchase_MetodoPagoInvalido_DevuelveBadRequest(
        MetodoPago metodoPago)
    {
        // Arrange
        var compraDTO = CreateValidDTO(metodoPago: metodoPago);

        // Act
        var result = await _controller.CreatePurchase(compraDTO);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(_context.ComprasModelo3Ds);
    }
}
