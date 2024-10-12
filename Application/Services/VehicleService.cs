using System.Net.Http.Json;
using Application.DTOs.Request.Vehicles;
using Application.DTOs.Response;
using Application.DTOs.Response.Vehicles;
using Application.Extensions;

namespace Application.Services;

public class VehicleService (HttpClientService httpClientService) : IVehicleService
{
    #region Private Methods
    private async Task<HttpClient> PrivateClient()
        => await httpClientService.GetPrivateClient();

    private static string? CheckResponseStatus(HttpResponseMessage response)
    {
        return !response.IsSuccessStatusCode
            ? $"Sorry unknown error occured.{Environment.NewLine}" +
              $"Error Description:{Environment.NewLine}" +
              $"Status Code: {response.StatusCode}{Environment.NewLine}" +
              $"Reason Phrase: {response.ReasonPhrase}"
            : null;
    }
    
    private static GeneralResponse ErrorOperation(string? message)
        => new (false, message);
    #endregion
    
    #region Public Methods

    #region Add
    public async Task<GeneralResponse> AddVehicle(CreateVehicleRequestDto model)
    {
        var result = await (await PrivateClient()).PostAsJsonAsync(Constant.AddVehicleRoute, model);
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }

    public async Task<GeneralResponse> AddVehicleBrand(CreateVehicleBrandRequestDto model)
    {
        var result = await (await PrivateClient()).PostAsJsonAsync(Constant.AddVehicleBrandRoute, model);
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }

    public async Task<GeneralResponse> AddVehicleOwner(CreateVehicleOwnerRequestDto model)
    {
        var result = await (await PrivateClient()).PostAsJsonAsync(Constant.AddVehicleOwnerRoute, model);
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }
    #endregion

    #region Delete
    public async Task<GeneralResponse> DeleteVehicle(int id)
    {
        var result = await (await PrivateClient()).DeleteAsync($"{Constant.DeleteVehicleRoute}/{id}");
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }

    public async Task<GeneralResponse> DeleteVehicleBrand(int id)
    {
        var result = await (await PrivateClient()).DeleteAsync($"{Constant.DeleteVehicleBrandRoute}/{id}");
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }

    public async Task<GeneralResponse> DeleteVehicleOwner(int id)
    {
        var result = await (await PrivateClient()).DeleteAsync($"{Constant.DeleteVehicleOwnerRoute}/{id}");
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }
    #endregion

    #region Update
    public async Task<GeneralResponse> UpdateVehicle(UpdateVehicleRequestDto model)
    {
        var result = await (await PrivateClient()).PutAsJsonAsync(Constant.UpdateVehicleRoute, model);
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }

    public async Task<GeneralResponse> UpdateVehicleBrand(UpdateVehicleBrandRequestDto model)
    {
        var result = await (await PrivateClient()).PutAsJsonAsync(Constant.UpdateVehicleBrandRoute, model);
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }

    public async Task<GeneralResponse> UpdateVehicleOwner(UpdateVehicleOwnerRequestDto model)
    {
        var result = await (await PrivateClient()).PutAsJsonAsync(Constant.UpdateVehicleOwnerRoute, model);
        if (!string.IsNullOrEmpty(CheckResponseStatus(result))) return ErrorOperation(CheckResponseStatus(result));
        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }
    #endregion

    #region Get Single
    public async Task<GetVehicleResponseDto> GetVehicle(int id)
        => (await (await PrivateClient()).GetFromJsonAsync<GetVehicleResponseDto>($"{Constant.GetVehicleRoute}/{id}"))!;

    public async Task<GetVehicleBrandResponseDto> GetVehicleBrand(int id)
        => (await (await PrivateClient()).GetFromJsonAsync<GetVehicleBrandResponseDto>($"{Constant.GetVehicleBrandRoute}/{id}"))!;

    public async Task<GetVehicleOwnerResponseDto> GetVehicleOwner(int id)
        => (await (await PrivateClient()).GetFromJsonAsync<GetVehicleOwnerResponseDto>($"{Constant.GetVehicleOwnerRoute}/{id}"))!;
    #endregion

    #region Get List
    public async Task<IEnumerable<GetVehicleResponseDto>> GetVehicles()
        => (await (await PrivateClient()).GetFromJsonAsync<IEnumerable<GetVehicleResponseDto>>(Constant.GetVehiclesRoute))!;

    public async Task<IEnumerable<GetVehicleBrandResponseDto>> GetVehicleBrands()
        => (await (await PrivateClient()).GetFromJsonAsync<IEnumerable<GetVehicleBrandResponseDto>>(Constant.GetVehicleBrandsRoute))!;

    public async Task<IEnumerable<GetVehicleOwnerResponseDto>> GetVehicleOwners()
        => (await (await PrivateClient()).GetFromJsonAsync<IEnumerable<GetVehicleOwnerResponseDto>>(Constant.GetVehicleOwnersRoute))!;
    #endregion
    
    #endregion
}