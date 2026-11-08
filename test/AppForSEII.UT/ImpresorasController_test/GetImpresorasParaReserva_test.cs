using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ImpresoraDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppForSEII.UT.ImpresorasController_test
{
    public class GetImpresorasParaReserva_test : AppForSEII4SqliteUT
    {
        public GetImpresorasParaReserva_test()
        {
            var prusa = new Impresora3D(
                1,
                "Prusa MK4",
                "MK4",
                TipoImpresora.Filamento,
                "Impresora de filamento",
                0.25m,
                8.00m);

            var elegoo = new Impresora3D(
                2,
                "Elegoo Mars",
                "Mars 4",
                TipoImpresora.Resina,
                "Impresora de resina",
                0.30m,
                10.00m);

            var bambu = new Impresora3D(
                3,
                "Bambu Lab",
                "P1S",
                TipoImpresora.Filamento,
                "Impresora de filamento cerrada",
                0.35m,
                12.00m);

            _context.Impresora3Ds.AddRange(prusa, elegoo, bambu);
            _context.SaveChanges();
        }

        public static IEnumerable<object?[]> CasosSelect()
        {
            var prusa = new ImpresoraParaReservaDTO(
                1,
                "Prusa MK4",
                "MK4",
                TipoImpresora.Filamento,
                "Impresora de filamento",
                0.25m,
                8.00m);

            var elegoo = new ImpresoraParaReservaDTO(
                2,
                "Elegoo Mars",
                "Mars 4",
                TipoImpresora.Resina,
                "Impresora de resina",
                0.30m,
                10.00m);

            var bambu = new ImpresoraParaReservaDTO(
                3,
                "Bambu Lab",
                "P1S",
                TipoImpresora.Filamento,
                "Impresora de filamento cerrada",
                0.35m,
                12.00m);

            return new List<object?[]>
            {
                // Sin filtros: devuelve todas ordenadas por nombre.
                new object?[]
                {
                    null,
                    null,
                    new List<ImpresoraParaReservaDTO>
                    {
                        bambu, elegoo, prusa
                    }
                },

                // Nombre parcial.
                new object?[]
                {
                    "Prusa",
                    null,
                    new List<ImpresoraParaReservaDTO> { prusa }
                },

                // Tipo con varias coincidencias, ordenadas por nombre.
                new object?[]
                {
                    null,
                    TipoImpresora.Filamento,
                    new List<ImpresoraParaReservaDTO> { bambu, prusa }
                },

                // Tipo con una coincidencia.
                new object?[]
                {
                    null,
                    TipoImpresora.Resina,
                    new List<ImpresoraParaReservaDTO> { elegoo }
                },

                // Ambos filtros coinciden.
                new object?[]
                {
                    "Prusa",
                    TipoImpresora.Filamento,
                    new List<ImpresoraParaReservaDTO> { prusa }
                },

                // El nombre coincide, pero el tipo no.
                new object?[]
                {
                    "Prusa",
                    TipoImpresora.Resina,
                    new List<ImpresoraParaReservaDTO>()
                },

                // Nombre sin coincidencias.
                new object?[]
                {
                    "NoExiste",
                    null,
                    new List<ImpresoraParaReservaDTO>()
                },

                // Tipo válido sin impresoras de ese tipo.
                new object?[]
                {
                    null,
                    TipoImpresora.Polimero,
                    new List<ImpresoraParaReservaDTO>()
                },

                // Nombre vacío: permite consultar todas.
                new object?[]
                {
                    "",
                    null,
                    new List<ImpresoraParaReservaDTO>
                    {
                        bambu, elegoo, prusa
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(CasosSelect))]
        public async Task GetImpresorasParaReserva_DevuelveResultadoEsperado(
            string? nombre,
            TipoImpresora? tipo,
            IList<ImpresoraParaReservaDTO> esperadas)
        {
            // Arrange
            var controller = new ImpresorasController(_context);

            // Act
            var resultado =
                await controller.GetImpresorasParaReserva(nombre, tipo);

            // Assert
            var respuesta = Assert.IsType<OkObjectResult>(resultado);

            var obtenidas =
                Assert.IsType<List<ImpresoraParaReservaDTO>>(
                    respuesta.Value);

            Assert.Equal(esperadas.Count, obtenidas.Count);

            for (int i = 0; i < esperadas.Count; i++)
            {
                Assert.True(
                    esperadas[i].Equals(obtenidas[i]),
                    $"La impresora de la posición {i} no coincide.");
            }
        }

        [Fact]
        public async Task GetImpresorasParaReserva_SinImpresoras_DevuelveListaVacia()
        {
            // Arrange
            _context.Impresora3Ds.RemoveRange(_context.Impresora3Ds);
            _context.SaveChanges();

            var controller = new ImpresorasController(_context);

            // Act
            var resultado =
                await controller.GetImpresorasParaReserva();

            // Assert
            var respuesta = Assert.IsType<OkObjectResult>(resultado);

            var obtenidas =
                Assert.IsType<List<ImpresoraParaReservaDTO>>(
                    respuesta.Value);

            Assert.Empty(obtenidas);
        }
    }
}