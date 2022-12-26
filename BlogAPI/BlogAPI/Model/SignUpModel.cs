
using System.ComponentModel.DataAnnotations;


public class SignUpModel
{

    [Required(ErrorMessage = "Please provide all fields.")]
    public String Firstname { get; set; }
    [Required(ErrorMessage = "Please provide all fields.")]
    public String Lastname { get; set; }
    [Required(ErrorMessage = "Please provide all fields.")]
    public String Username { get; set; }
    [Required(ErrorMessage = "Please provide all fields.")]
    public String Email { get; set; }
    [Required(ErrorMessage = "Please provide all fields.")]
    [Compare("ConfirmPassword")]
    public String Password { get; set; }
    [Required(ErrorMessage = "Please provide all fields.")]
    public String ConfirmPassword { get; set; }
}
