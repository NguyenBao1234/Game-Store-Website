using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace POWStudio.Models;
//Order aka Bill
public class Order
{
    public int Id { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    [Column(TypeName = "money")]
    public decimal? Price { get; set; }
    
    [Column(TypeName = "money")]
    public decimal? DiscountAmount {get; set;}
    
    [Column(TypeName = "money")]
    public decimal? FinalPrice {get; set;}

}