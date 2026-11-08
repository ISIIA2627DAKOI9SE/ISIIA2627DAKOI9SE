
namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class LineaCompraModeloForCreateDTO
{
    public LineaCompraModeloForCreateDTO(
        int modelo3DId,
        int cantidadLicencias)
    {
        Modelo3DId = modelo3DId;
        CantidadLicencias = cantidadLicencias;
    }

    public int Modelo3DId { get; set; }

    public int CantidadLicencias { get; set; }
}
