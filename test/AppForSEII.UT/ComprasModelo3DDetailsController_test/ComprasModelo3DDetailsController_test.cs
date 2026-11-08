
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT;

public class ComprasModelo3DDetailsController_test : AppForSEII4SqliteUT
{
    private readonly ComprasModelo3DController _controller;
    private readonly Mock<ILogger<ComprasModelo3DController>> _mockLogger;

    private readonly DateTime _fechaCompra =
        new DateTime(2026, 11, 8, 12, 0, 0, DateTimeKind.Utc);

    public ComprasModelo3DDetailsController_test()
    {
        _mockLogger =
            new Mock<ILogger<ComprasModelo3DController>>();

        _controller = new ComprasModelo3DController(
            _context,
            _mockLogger.Object
        );
    }

    // Preparar una compra con dos modelos 3D
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

        var compra = new CompraModelo3D(
            1,
            _fechaCompra,
            "Carlos",
            "Sanchez",
            "carlos@uclm.es",
            "Calle Universidad 10",
            "Compra de modelos 3D",
            34.97m,
            MetodoPago.PayPal
        );

        var lineaDragon = new LineaCompraModelo(
            1,
            2,
            9.99m,
            19.98m
        );

        lineaDragon.Modelo3D = dragon;
        lineaDragon.CompraModelo3D = compra;

        var lineaTorre = new LineaCompraModelo(
            2,
            1,
            14.99m,
            14.99m
        );

        lineaTorre.Modelo3D = torre;
        lineaTorre.CompraModelo3D = compra;

        compra.LineasCompraModelo.Add(lineaDragon);
        compra.LineasCompraModelo.Add(lineaTorre);

        _context.Modelo3Ds.AddRange(dragon, torre);
        _context.ComprasModelo3Ds.Add(compra);

        _context.SaveChanges();
    }

    // PRUEBA 1: Obtener los detalles de una compra existente
    [Fact]
    public async Task GetPurchase_CompraExistente_DevuelveDetailsDTO()
    {
        // Arrange
        SeedDatabase();

        var lineasEsperadas = new List<LineaCompraModeloDetailDTO>
        {
            new LineaCompraModeloDetailDTO(
                1,
                "Dragon Medieval",
                "Miniaturas",
                FormatoModelo3D.STL,
                2,
                9.99m,
                19.98m
            ),
            new LineaCompraModeloDetailDTO(
                2,
                "Torre Medieval",
                "Arquitectura",
                FormatoModelo3D.OBJ,
                1,
                14.99m,
                14.99m
            )
        };

        var esperado = new CompraModelo3DDetailDTO(
            1,
            _fechaCompra,
            "Carlos",
            "Sanchez",
            "carlos@uclm.es",
            "Calle Universidad 10",
            "Compra de modelos 3D",
            34.97m,
            MetodoPago.PayPal,
            lineasEsperadas
        );

        // Act
        var result = await _controller.GetPurchase(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var actual = Assert.IsType<CompraModelo3DDetailDTO>(
            okResult.Value
        );

        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(esperado, actual);
        Assert.Equal(2, actual.Lineas.Count);
    }

    // PRUEBA 2: La compra solicitada no existe
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(9999)]
    public async Task GetPurchase_CompraInexistente_DevuelveNotFound(
        int compraId
    )
    {
        // Arrange
        SeedDatabase();

        // Act
        var result = await _controller.GetPurchase(compraId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // PRUEBA 3: No hay compras en la base de datos
    [Fact]
    public async Task GetPurchase_BDVacia_DevuelveNotFound()
    {
        // Arrange
        // No se inserta ninguna compra.

        // Act
        var result = await _controller.GetPurchase(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // PRUEBA 4: Comprobar cantidades y precio total
    [Fact]
    public async Task GetPurchase_CompraExistente_CantidadesCorrectas()
    {
        // Arrange
        SeedDatabase();

        // Act
        var result = await _controller.GetPurchase(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var actual = Assert.IsType<CompraModelo3DDetailDTO>(
            okResult.Value
        );

        Assert.Equal(34.97m, actual.PrecioTotal);

        Assert.Equal(2, actual.Lineas.Count);

        Assert.Equal(2, actual.Lineas[0].CantidadLicencias);
        Assert.Equal(19.98m, actual.Lineas[0].Subtotal);

        Assert.Equal(1, actual.Lineas[1].CantidadLicencias);
        Assert.Equal(14.99m, actual.Lineas[1].Subtotal);
    }
}
