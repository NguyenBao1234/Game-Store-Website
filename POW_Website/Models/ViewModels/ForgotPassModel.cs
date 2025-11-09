using System.ComponentModel.DataAnnotations;

namespace POWStudio.Models.ViewModels;

public class ForgotPassModel
{
    [Required(ErrorMessage = "Email là bắt buộc."), EmailAddress]
    public string Email { get; set; }
    
    public bool bSuccess { get; set; } = false;
}