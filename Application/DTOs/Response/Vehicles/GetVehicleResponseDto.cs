using Application.DTOs.Request.Vehicles;

namespace Application.DTOs.Response.Vehicles;

public class GetVehicleResponseDto : VehicleBaseDto
{
    public int Id { get; set; }
    public virtual GetVehicleBrandResponseDto? VehicleBrand { get; set; } = null;
    public virtual GetVehicleOwnerResponseDto? VehicleOwner { get; set; } = null;
}