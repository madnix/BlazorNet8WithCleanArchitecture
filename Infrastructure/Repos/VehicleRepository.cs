using Application.Contracts;
using Application.DTOs.Request.Vehicles;
using Application.DTOs.Response;
using Application.DTOs.Response.Vehicles;
using Domain.Entity.VehicleEntity;
using Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class VehicleRepository(AppDbContext context) : IVehicle
{
    #region Private Methods
    
    private async Task<Vehicle> FindVehicleByName(string name)
        => (await context.Vehicles.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower()))!;
    
    private async Task<VehicleBrand?> FindVehicleBrandByName(string name)
        => await context.VehicleBrands.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower());
    
    private async Task<VehicleOwner?> FindVehicleOwnerByName(string name)
        => await context.VehicleOwners.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower());
    
    private async Task<Vehicle> FindVehicleById(int id)
        => (await context.Vehicles.Include(b => b.VehicleBrand).Include(o => o.VehicleOwner)
            .FirstOrDefaultAsync(x => x.Id == id))!;
    
    private async Task<VehicleBrand> FindVehicleBrandById(int id)
        => (await context.VehicleBrands.FirstOrDefaultAsync(x => x.Id == id))!;
    
    private async Task<VehicleOwner> FindVehicleOwnerById(int id)
        => (await context.VehicleOwners.FirstOrDefaultAsync(x => x.Id == id))!;
    
    private async Task SaveChangesAsync()
        => await context.SaveChangesAsync();

    private static GeneralResponse NullResponse(string message)
        => new(false, message);
    
    private static GeneralResponse AlreadyExistResponse(string message)
        => new(false, message);
    
    private static GeneralResponse OperationSuccessResponse(string message)
        => new(true, message);
    
    #endregion

    #region Public Methods

    #region Add
    public async Task<GeneralResponse> AddVehicle(CreateVehicleRequestDto model)
    {
        if (await FindVehicleByName(model.Name) is not null) return AlreadyExistResponse("Vehicle already exists");
        context.Vehicles.Add(model.Adapt(new Vehicle()));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle data saved");
    }
    
    public async Task<GeneralResponse> AddVehicleBrand(CreateVehicleBrandRequestDto model)
    {
        if (await FindVehicleBrandByName(model.Name) is not null) return AlreadyExistResponse("Vehicle Brand already exists");
        context.VehicleBrands.Add(model.Adapt(new VehicleBrand()));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle Brand data saved");
    }
    
    public async Task<GeneralResponse> AddVehicleOwner(CreateVehicleOwnerRequestDto model)
    {
        if (await FindVehicleOwnerByName(model.Name) is not null) return AlreadyExistResponse("Vehicle Owner already exists");
        context.VehicleOwners.Add(model.Adapt(new VehicleOwner()));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle Owner data saved");
    }
    #endregion

    #region Delete
    public async Task<GeneralResponse> DeleteVehicle(int id)
    {
        if (await FindVehicleById(id) is null) return NullResponse("Vehicle not found");
        context.Vehicles.Remove(await FindVehicleById(id));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle deleted");
    }
    
    public async Task<GeneralResponse> DeleteVehicleBrand(int id)
    {
        if (await FindVehicleBrandById(id) is null) return NullResponse("Vehicle Brand not found");
        context.VehicleBrands.Remove(await FindVehicleBrandById(id));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle Brand deleted");
    }
    
    public async Task<GeneralResponse> DeleteVehicleOwner(int id)
    {
        if (await FindVehicleOwnerById(id) is null) return NullResponse("Vehicle Owner not found");
        context.VehicleOwners.Remove(await FindVehicleOwnerById(id));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle Owner deleted");
    }
    #endregion

    #region Update
    public async Task<GeneralResponse> UpdateVehicle(UpdateVehicleRequestDto model)
    {
        if (await FindVehicleById(model.Id) is null) return NullResponse("Vehicle not found");
        context.Entry(await FindVehicleById(model.Id)).State = EntityState.Detached;
        context.Vehicles.Update(model.Adapt(new Vehicle()));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle data updated");
    }
    
    public async Task<GeneralResponse> UpdateVehicleBrand(UpdateVehicleBrandRequestDto model)
    {
        if (await FindVehicleBrandById(model.Id) is null) return NullResponse("Vehicle Brand not found");
        context.Entry(await FindVehicleBrandById(model.Id)).State = EntityState.Detached;
        context.VehicleBrands.Update(model.Adapt(new VehicleBrand()));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle Brand data updated");
    }
    
    public async Task<GeneralResponse> UpdateVehicleOwner(UpdateVehicleOwnerRequestDto model)
    {
        if (await FindVehicleOwnerById(model.Id) is null) return NullResponse("Vehicle Owner not found");
        context.Entry(await FindVehicleOwnerById(model.Id)).State = EntityState.Detached;
        context.VehicleOwners.Update(model.Adapt(new VehicleOwner()));
        await SaveChangesAsync();
        return OperationSuccessResponse("Vehicle Owner data updated");
    }
    #endregion

    #region Get Single
    public async Task<GetVehicleResponseDto> GetVehicle(int id)
        => (await FindVehicleById(id)).Adapt(new GetVehicleResponseDto());

    public async Task<GetVehicleBrandResponseDto> GetVehicleBrand(int id)
        => (await FindVehicleBrandById(id)).Adapt(new GetVehicleBrandResponseDto());
    
    public async Task<GetVehicleOwnerResponseDto> GetVehicleOwner(int id)
        => (await FindVehicleOwnerById(id)).Adapt(new GetVehicleOwnerResponseDto());
    #endregion

    #region Get List
    public async Task<IEnumerable<GetVehicleResponseDto>> GetVehicles()
    {
        var data = await context
            .Vehicles
            .Include(b => b.VehicleBrand)
            .Include(o => o.VehicleOwner)
            .ToListAsync();

        return data.Select(vehicle => new GetVehicleResponseDto
        {
            Id = vehicle.Id,
            Name = vehicle.Name,
            Description = vehicle.Description,
            VehicleOwnerId = vehicle.VehicleOwnerId,
            VehicleBrandId = vehicle.VehicleBrandId,
            VehicleBrand = new GetVehicleBrandResponseDto
            {
                Id = vehicle.VehicleBrand!.Id,
                Name = vehicle.VehicleBrand.Name,
                Location = vehicle.VehicleBrand.Location
            },
            VehicleOwner = new GetVehicleOwnerResponseDto
            {
                Id = vehicle.VehicleOwner!.Id,
                Name = vehicle.VehicleOwner.Name,
                Address = vehicle.VehicleOwner.Address
            }
        }).ToList();
    }

    public async Task<IEnumerable<GetVehicleBrandResponseDto>> GetVehicleBrands()
        => (await context.VehicleBrands.ToListAsync()).Adapt<List<GetVehicleBrandResponseDto>>();

    public async Task<IEnumerable<GetVehicleOwnerResponseDto>> GetVehicleOwners()
        => (await context.VehicleOwners.ToListAsync()).Adapt<List<GetVehicleOwnerResponseDto>>();
    
    #endregion
    
    #endregion
}