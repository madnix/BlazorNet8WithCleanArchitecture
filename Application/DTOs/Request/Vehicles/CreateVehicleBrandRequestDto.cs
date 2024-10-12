using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Vehicles;

public class CreateVehicleBrandRequestDto
{
    [Required] public string? Name { get; set; }

    [Required] public string? Location { get; set; }
}