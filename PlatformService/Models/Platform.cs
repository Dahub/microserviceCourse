using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models;

public record Platform
{
    [Key]       
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Publisher { get; init; } = string.Empty;

    public string Cost { get; init; } = string.Empty;
}
    