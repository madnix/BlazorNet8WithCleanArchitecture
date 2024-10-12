using Application.DTOs.Request.Vehicles;
using Application.DTOs.Response;
using Application.DTOs.Response.Vehicles;

namespace Application.Contracts;

public interface IVehicle
{
    Task<GeneralResponse> AddVehicle(CreateVehicleRequestDto model);
    Task<GeneralResponse> AddVehicleBrand(CreateVehicleBrandRequestDto model);
    Task<GeneralResponse> AddVehicleOwner(CreateVehicleOwnerRequestDto model);
    Task<GeneralResponse> DeleteVehicle(int id);
    Task<GeneralResponse> DeleteVehicleBrand(int id);
    Task<GeneralResponse> DeleteVehicleOwner(int id);
    Task<GeneralResponse> UpdateVehicle(UpdateVehicleRequestDto model);
    Task<GeneralResponse> UpdateVehicleBrand(UpdateVehicleBrandRequestDto model);
    Task<GeneralResponse> UpdateVehicleOwner(UpdateVehicleOwnerRequestDto model);
    Task<GetVehicleResponseDto> GetVehicle(int id);
    Task<GetVehicleBrandResponseDto> GetVehicleBrand(int id);
    Task<GetVehicleOwnerResponseDto> GetVehicleOwner(int id);
    Task<IEnumerable<GetVehicleResponseDto>> GetVehicles();
    Task<IEnumerable<GetVehicleBrandResponseDto>> GetVehicleBrands();
    Task<IEnumerable<GetVehicleOwnerResponseDto>> GetVehicleOwners();
}