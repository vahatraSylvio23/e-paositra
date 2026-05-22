using System.ComponentModel.DataAnnotations;

namespace ViewModel;
public class ModifyViewModel
{
    [Required(ErrorMessage = "Le prénom est requis.")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage = "Le nom de famille est requis.")]
    public string? LastName { get; set; }
    [Required(ErrorMessage = "L'email est requis.")]

    public string? Email { get; set; }
    [Required(ErrorMessage = "Le numéro de téléphone est requis.")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "Le mot de passe est requis.")]
    public string? Password { get; set; }
    // [Required(ErrorMessage = "La confirmation du mot de passe est requise.")]
    // [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    // public string? ConfirmPassword {get; set; }
}