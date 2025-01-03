using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos;

public class Register : AccountBase
{
    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    public string? Name { get; set; }
    
    [Required]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}