namespace POWStudio.Models.ViewModels;

public class OrderUListItemVM
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    
    public decimal OriginalPrice { get; set; }

    public decimal? DiscountAmount {get; set;}
    
    public decimal FinalPrice => OriginalPrice - DiscountAmount??0;

    public List<GameInOrderVM> Games { get; set; }
    public string MainName { get; set; }
    public string SubName { get; set; }
}