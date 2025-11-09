using System.ComponentModel.DataAnnotations;

namespace POWStudio.Models.ViewModels;

public class RegisterModel
{
    [Required(ErrorMessage = "Name là bắt buộc.")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "{0} phải có ít nhất {2} ký tự và có chỉ có thể dài tối đa {1} ký tự.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Email là bắt buộc."), EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password là bắt buộc."), DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} phải có ít nhất {2} ký tự và có chỉ có thể dài tối đa {1} ký tự.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,100}$",
        ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ thường, 1 chữ hoa, 1 chữ số, 1 ký tự đặc biệt và dài từ 6 đến 100 ký tự.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Nhập lại Mật khẩu là bắt buộc."), Compare("Password",ErrorMessage = "Mật khẩu xác nhận không khớp."), DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}