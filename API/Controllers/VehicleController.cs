using Application.Contracts;
using Application.DTOs.Request.Vehicles;
using Application.DTOs.Response;
using Application.DTOs.Response.Vehicles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehicleController(IVehicle vehicle) : ControllerBase
{
    [HttpPost("add-vehicle")]
    public async Task<ActionResult<GeneralResponse>> Create(CreateVehicleRequestDto model)
        => await vehicle.AddVehicle(model);
    
    [HttpPost("add-vehicle-brand")]
    public async Task<ActionResult<GeneralResponse>> Create(CreateVehicleBrandRequestDto model)
        => await vehicle.AddVehicleBrand(model);
    
    [HttpPost("add-vehicle-owner")]
    public async Task<ActionResult<GeneralResponse>> Create(CreateVehicleOwnerRequestDto model)
        => await vehicle.AddVehicleOwner(model);
    
    [HttpGet("get-vehicle/{id}")]
    public async Task<ActionResult<GetVehicleResponseDto>> Get(int id)
        => await vehicle.GetVehicle(id);
    
    [HttpGet("get-vehicle-brand/{id}")]
    public async Task<ActionResult<GetVehicleBrandResponseDto>> GetBrand(int id)
        => await vehicle.GetVehicleBrand(id);
    
    [HttpGet("get-vehicle-owner/{id}")]
    public async Task<ActionResult<GetVehicleOwnerResponseDto>> GetOwner(int id)
        => await vehicle.GetVehicleOwner(id);
    
    [HttpGet("get-vehicles")]
    public async Task<ActionResult<IEnumerable<GetVehicleResponseDto>>> GetList()
        => Ok(await vehicle.GetVehicles());
    
    [HttpGet("get-vehicle-brands")]
    public async Task<ActionResult<IEnumerable<GetVehicleBrandResponseDto>>> GetBrandList()
        => Ok(await vehicle.GetVehicleBrands());
    
    [HttpGet("get-vehicle-owners")]
    public async Task<ActionResult<IEnumerable<GetVehicleOwnerResponseDto>>> GetOwnerList()
        => Ok(await vehicle.GetVehicleOwners());

    [HttpPut("update-vehicle")]
    public async Task<ActionResult<GeneralResponse>> Update(UpdateVehicleRequestDto model)
        => Ok(await vehicle.UpdateVehicle(model));
    
    [HttpPut("update-vehicle-brand")]
    public async Task<ActionResult<GeneralResponse>> UpdateBrand(UpdateVehicleBrandRequestDto model)
        => Ok(await vehicle.UpdateVehicleBrand(model));
    
    [HttpPut("update-vehicle-owner")]
    public async Task<ActionResult<GeneralResponse>> UpdateOwner(UpdateVehicleOwnerRequestDto model)
        => Ok(await vehicle.UpdateVehicleOwner(model));
    
    [HttpDelete("delete-vehicle/{id}")]
    public async Task<ActionResult<GeneralResponse>> Delete(int id)
        => Ok(await vehicle.DeleteVehicle(id));
    
    [HttpDelete("delete-vehicle-brand/{id}")]
    public async Task<ActionResult<GeneralResponse>> DeleteBrand(int id)
        => Ok(await vehicle.DeleteVehicleBrand(id));
    
    [HttpDelete("delete-vehicle-owner/{id}")]
    public async Task<ActionResult<GeneralResponse>> DeleteOwner(int id)
        => Ok(await vehicle.DeleteVehicleOwner(id));
}