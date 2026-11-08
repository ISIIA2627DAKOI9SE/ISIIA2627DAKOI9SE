using System.Globalization;
using AppForSEII.API.Controllers;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.ReservaDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AppForSEII.UT.ReservasController_test;

public class CreateReserva_test : AppForSEII4SqliteUT
{
    public CreateReserva_test()
    {
        var cliente = new Cliente(
            "cliente-prueba",
            "Peter",
            "Jackson",
            "peter@prueba.es",
            "Calle Mayor 12");

        var prusa = new Impresora3D(
            1,
            "Prusa MK4",
            "MK4",
            TipoImpresora.Filamento,
            "Impresora de filamento",
            0.25m,
            8m);

        var elegoo = new Impresora3D(
            2,
            "Elegoo Mars",
            "Mars 4",
            TipoImpresora.Resina,
            "Impresora de resina",
            0.30m,
            10m);

        _context.Clientes.Add(cliente);
        _context.Impresora3Ds.AddRange(prusa, elegoo);
        _context.SaveChanges();
    }

    private static ReservaCrearDTO CrearSolicitudValida()
    {
        var lineas = new List<LineaReservaCrearDTO>();

        lineas.Add(new LineaReservaCrearDTO(
            1, TiempoReserva.DosHoras));

        return new ReservaCrearDTO(
            "peter@prueba.es",
            "Peter",
            "Jackson",
            "Calle Mayor 12",
            MetodoPago.Bizum,
            lineas);
    }

    [Theory]
    [InlineData(TiempoReserva.UnaHora, "8.25", false)]
    [InlineData(TiempoReserva.DosHoras, "8.50", false)]
    [InlineData(TiempoReserva.TresHoras, "8.75", false)]
    [InlineData(TiempoReserva.CuatroHoras, "9.00", false)]
    [InlineData(TiempoReserva.CincoHoras, "9.25", false)]
    [InlineData(TiempoReserva.UnDia, "14.00", false)]
    [InlineData(TiempoReserva.DosDias, "20.00", false)]
    [InlineData(TiempoReserva.DosHoras, "8.50", true)]
    public async Task CreateReserva_Valida_DevuelveYGuardaDetalle(
        TiempoReserva tiempo,
        string subtotalEsperadoTexto,
        bool incluirSegundaImpresora)
    {
        // Arrange
        decimal subtotalEsperado = decimal.Parse(
            subtotalEsperadoTexto,
            CultureInfo.InvariantCulture);

        var solicitud = CrearSolicitudValida();
        solicitud.LineasReserva.Clear();

        // En el caso de varias impresoras enviamos primero la ID 2.
        if (incluirSegundaImpresora)
        {
            solicitud.LineasReserva.Add(new LineaReservaCrearDTO(
                2, TiempoReserva.UnDia));
        }

        solicitud.LineasReserva.Add(new LineaReservaCrearDTO(
            1, tiempo));

        var lineasEsperadas = new List<LineaReservaDetalleDTO>();

        lineasEsperadas.Add(new LineaReservaDetalleDTO(
            1,
            "Prusa MK4",
            "MK4",
            TipoImpresora.Filamento,
            "Impresora de filamento",
            0.25m,
            8m,
            tiempo,
            subtotalEsperado));

        decimal totalEsperado = subtotalEsperado;

        if (incluirSegundaImpresora)
        {
            lineasEsperadas.Add(new LineaReservaDetalleDTO(
                2,
                "Elegoo Mars",
                "Mars 4",
                TipoImpresora.Resina,
                "Impresora de resina",
                0.30m,
                10m,
                TiempoReserva.UnDia,
                17.20m));

            totalEsperado += 17.20m;
        }

        using var context = CreateContext();
        var controller = new ReservasController(context);
        var antes = DateTime.Now;

        // Act
        var resultado = await controller.CreateReserva(solicitud);

        var despues = DateTime.Now;

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(resultado);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(nameof(ReservasController.GetReserva), created.ActionName);

        var detalle = Assert.IsType<ReservaDetalleDTO>(created.Value);

        Assert.True(detalle.Id > 0);
        Assert.InRange(detalle.FechaReserva, antes, despues);
        Assert.NotNull(created.RouteValues);
        Assert.Equal(
            detalle.Id,
            Assert.IsType<int>(created.RouteValues!["id"]));

        // El ID y la fecha los genera el servidor.
        // Sus valores se han comprobado antes de usarlos aquí.
        var esperado = new ReservaDetalleDTO(
            detalle.Id,
            detalle.FechaReserva,
            "Peter",
            "Jackson",
            "Calle Mayor 12",
            MetodoPago.Bizum,
            totalEsperado,
            lineasEsperadas);

        Assert.True(esperado.Equals(detalle));

        // Un contexto nuevo comprueba lo guardado realmente.
        using var comprobacion = CreateContext();

        var guardada = await comprobacion.ReservaImpresoras
            .Include(reserva => reserva.LineasReserva)
            .SingleAsync();

        Assert.Equal(detalle.Id, guardada.Id);
        Assert.Equal("cliente-prueba", guardada.ClienteId);
        Assert.Equal(totalEsperado, guardada.PrecioTotal);
        Assert.Equal(
            lineasEsperadas.Count,
            guardada.LineasReserva.Count);

        foreach (var lineaEsperada in lineasEsperadas)
        {
            var lineaGuardada = Assert.Single(
                guardada.LineasReserva,
                linea => linea.Impresora3DId
                        == lineaEsperada.Impresora3DId);

            Assert.Equal(guardada.Id, lineaGuardada.ReservaImpresoraId);
            Assert.Equal(
                lineaEsperada.TiempoReserva,
                lineaGuardada.TiempoReserva);
            Assert.Equal(
                lineaEsperada.PrecioSubtotal,
                lineaGuardada.PrecioSubtotal);
        }

        var controllerLectura = new ReservasController(comprobacion);
        var lectura = await controllerLectura.GetReserva(detalle.Id);
        var ok = Assert.IsType<OkObjectResult>(lectura);
        var detalleGuardado = Assert.IsType<ReservaDetalleDTO>(ok.Value);

        Assert.True(esperado.Equals(detalleGuardado));
    }

    [Theory]
    [InlineData("ListaNula", "LineasReserva")]
    [InlineData("ListaVacia", "LineasReserva")]
    [InlineData("LineaNula", "LineasReserva")]
    [InlineData("IdCero", "LineasReserva")]
    [InlineData("IdNegativo", "LineasReserva")]
    [InlineData("TiempoInvalido", "LineasReserva")]
    [InlineData("PagoNulo", "MetodoPago")]
    [InlineData("PagoInvalido", "MetodoPago")]
    [InlineData("Duplicada", "LineasReserva")]
    [InlineData("ClienteInexistente", "ClienteUserName")]
    [InlineData("ImpresoraInexistente", "LineasReserva")]
    [InlineData("UnaValidaYOtraInexistente", "LineasReserva")]
    public async Task CreateReserva_Invalida_NoGuardaReserva(
        string caso,
        string campoEsperado)
    {
        // Arrange
        var solicitud = CrearSolicitudValida();

        switch (caso)
        {
            case "ListaNula":
                // null! permite introducir el dato incorrecto a propósito.
                solicitud.LineasReserva = null!;
                break;

            case "ListaVacia":
                solicitud.LineasReserva.Clear();
                break;

            case "LineaNula":
                solicitud.LineasReserva.Clear();
                solicitud.LineasReserva.Add(null!);
                break;

            case "IdCero":
                solicitud.LineasReserva[0].Impresora3DId = 0;
                break;

            case "IdNegativo":
                solicitud.LineasReserva[0].Impresora3DId = -1;
                break;

            case "TiempoInvalido":
                solicitud.LineasReserva[0].TiempoReserva =
                    (TiempoReserva)999;
                break;

            case "PagoNulo":
                solicitud.MetodoPago = null;
                break;

            case "PagoInvalido":
                solicitud.MetodoPago = (MetodoPago)999;
                break;

            case "Duplicada":
                solicitud.LineasReserva.Add(new LineaReservaCrearDTO(
                    1, TiempoReserva.UnaHora));
                break;

            case "ClienteInexistente":
                solicitud.ClienteUserName = "noexiste@prueba.es";
                break;

            case "ImpresoraInexistente":
                solicitud.LineasReserva[0].Impresora3DId = 999;
                break;

            case "UnaValidaYOtraInexistente":
                solicitud.LineasReserva.Add(new LineaReservaCrearDTO(
                    999, TiempoReserva.UnaHora));
                break;
        }

        using var context = CreateContext();
        var controller = new ReservasController(context);

        // Act
        var resultado = await controller.CreateReserva(solicitud);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        var errores = Assert.IsType<ValidationProblemDetails>(
            badRequest.Value);

        Assert.Equal(400, badRequest.StatusCode);
        Assert.True(errores.Errors.ContainsKey(campoEsperado));
        Assert.NotEmpty(errores.Errors[campoEsperado]);

        using var comprobacion = CreateContext();

        Assert.False(await comprobacion.ReservaImpresoras.AnyAsync());
        Assert.False(await comprobacion.LineasReserva.AnyAsync());
    }

    [Fact]
    public async Task CreateReserva_ModelStateInvalido_DevuelveBadRequest()
    {
        // Arrange
        var solicitud = CrearSolicitudValida();
        solicitud.NombreCliente = "";

        using var context = CreateContext();
        var controller = new ReservasController(context);

        // Una llamada directa al controlador no ejecuta [ApiController].
        // Simulamos el error que añade la validación HTTP.
        controller.ModelState.AddModelError(
            "NombreCliente",
            "El nombre del cliente es obligatorio");

        // Act
        var resultado = await controller.CreateReserva(solicitud);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        var errores = Assert.IsType<ValidationProblemDetails>(
            badRequest.Value);

        Assert.True(errores.Errors.ContainsKey("NombreCliente"));
        Assert.Contains(
            "El nombre del cliente es obligatorio",
            errores.Errors["NombreCliente"]);

        using var comprobacion = CreateContext();

        Assert.False(await comprobacion.ReservaImpresoras.AnyAsync());
        Assert.False(await comprobacion.LineasReserva.AnyAsync());
    }

    [Fact]
    public async Task CreateReserva_TotalExcesivo_DevuelveBadRequest()
    {
        // Arrange
        var impresora = await _context.Impresora3Ds.SingleAsync(
            impresora => impresora.Id == 1);

        impresora.PrecioReserva = 99999999.99m;
        await _context.SaveChangesAsync();

        var solicitud = CrearSolicitudValida();

        using var context = CreateContext();
        var controller = new ReservasController(context);

        // Act
        var resultado = await controller.CreateReserva(solicitud);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        var errores = Assert.IsType<ValidationProblemDetails>(
            badRequest.Value);

        Assert.True(errores.Errors.ContainsKey("PrecioTotal"));

        using var comprobacion = CreateContext();

        Assert.False(await comprobacion.ReservaImpresoras.AnyAsync());
        Assert.False(await comprobacion.LineasReserva.AnyAsync());
    }

    [Fact]
    public async Task CreateReserva_ErrorAlGuardar_DevuelveConflict()
    {
        // Arrange
        var solicitud = CrearSolicitudValida();

        var contextSimulado = new Mock<ApplicationDbContext>(
            _contextOptions);

        contextSimulado.CallBase = true;

        // Las consultas utilizan SQLite, pero el guardado falla.
        contextSimulado
            .Setup(context => context.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException(
                "Fallo de guardado simulado"));

        using var context = contextSimulado.Object;
        var controller = new ReservasController(context);

        // Act
        var resultado = await controller.CreateReserva(solicitud);

        // Assert
        var conflict = Assert.IsType<ConflictObjectResult>(resultado);

        Assert.Equal(409, conflict.StatusCode);
        Assert.Equal(
            "No se ha podido guardar la reserva. Inténtalo de nuevo.",
            Assert.IsType<string>(conflict.Value));

        contextSimulado.Verify(
            context => context.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        using var comprobacion = CreateContext();

        Assert.False(await comprobacion.ReservaImpresoras.AnyAsync());
        Assert.False(await comprobacion.LineasReserva.AnyAsync());
    }
}