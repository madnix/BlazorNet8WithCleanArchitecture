using Application.DTOs.Request.Vehicles;

namespace Application.DTOs.Response.Vehicles;

public class GetVehicleBrandResponseDto : CreateVehicleBrandRequestDto
{
    public int Id { get; set; }
    public virtual ICollection<GetVehicleResponseDto>? Vehicles { get; set; } = null;
}