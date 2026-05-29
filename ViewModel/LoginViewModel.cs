using System.ComponentModel.DataAnnotations;

namespace ViewModel;

public class LoginViewModel
{
    [Required(ErrorMessage = "L'email est requis.")]
    [EmailAddress]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",ErrorMessage = "L'email doit contenir un domaine.")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Le mot de passe est requis.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}