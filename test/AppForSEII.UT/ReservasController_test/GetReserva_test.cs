using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ReservaDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppForSEII.UT.ReservasController_test;

public class GetReserva_test : AppForSEII4SqliteUT
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetReserva_Existente_DevuelveDetalle(
        int numeroImpresoras)
    {
        // Arrange
        var fecha = new DateTime(2026, 11, 8, 10, 0, 0);

        var cliente = new Cliente(
            "cliente-prueba",
            "Alberto",
            "Garcia",
            "alberto@prueba.es",
            "Calle Mayor 10");

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

        decimal total = numeroImpresoras == 1 ? 16m : 26m;

        var reserva = new ReservaImpresora(
            10,
            fecha,
            "Alberto",
            "Garcia",
            "Calle Mayor 10",
            total,
            MetodoPago.Bizum);

        reserva.ClienteId = cliente.Id;
        reserva.Cliente = cliente;

        var lineaPrusa = new LineaReserva(
            1, TiempoReserva.DosHoras, 16m);

        lineaPrusa.Impresora3DId = prusa.Id;
        lineaPrusa.Impresora3D = prusa;
        lineaPrusa.ReservaImpresoraId = reserva.Id;
        lineaPrusa.ReservaImpresora = reserva;

        var lineasEsperadas = new List<LineaReservaDetalleDTO>();

        lineasEsperadas.Add(new LineaReservaDetalleDTO(
            1,
            "Prusa MK4",
            "MK4",
            TipoImpresora.Filamento,
            "Impresora de filamento",
            0.25m,
            8m,
            TiempoReserva.DosHoras,
            16m));

        if (numeroImpresoras == 2)
        {
            var lineaElegoo = new LineaReserva(
                2, TiempoReserva.UnaHora, 10m);

            lineaElegoo.Impresora3DId = elegoo.Id;
            lineaElegoo.Impresora3D = elegoo;
            lineaElegoo.ReservaImpresoraId = reserva.Id;
            lineaElegoo.ReservaImpresora = reserva;

            // Añadimos primero la de ID 2 para comprobar la ordenación.
            reserva.LineasReserva.Add(lineaElegoo);

            lineasEsperadas.Add(new LineaReservaDetalleDTO(
                2,
                "Elegoo Mars",
                "Mars 4",
                TipoImpresora.Resina,
                "Impresora de resina",
                0.30m,
                10m,
                TiempoReserva.UnaHora,
                10m));
        }

        reserva.LineasReserva.Add(lineaPrusa);

        _context.ReservaImpresoras.Add(reserva);
        await _context.SaveChangesAsync();

        var esperado = new ReservaDetalleDTO(
            10,
            fecha,
            "Alberto",
            "Garcia",
            "Calle Mayor 10",
            MetodoPago.Bizum,
            total,
            lineasEsperadas);

        // Contexto nuevo: el controlador debe cargar las relaciones.
        using var context = CreateContext();
        var controller = new ReservasController(context);

        // Act
        var resultado = await controller.GetReserva(10);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(resultado);
        var detalle = Assert.IsType<ReservaDetalleDTO>(ok.Value);

        Assert.Equal(numeroImpresoras, detalle.LineasReserva.Count);
        Assert.True(esperado.Equals(detalle));
    }

    [Fact]
    public async Task GetReserva_Inexistente_DevuelveNotFound()
    {
        // Arrange
        using var context = CreateContext();
        var controller = new ReservasController(context);

        // Act
        var resultado = await controller.GetReserva(999);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }
}