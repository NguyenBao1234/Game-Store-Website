
using System.ComponentModel.DataAnnotations;

namespace POWStudio.Models.ViewModels;

public class RedeemVM
{
    [Required(ErrorMessage = "Redemption code is required.")]
    public string RedeemCode { get; set; }
}