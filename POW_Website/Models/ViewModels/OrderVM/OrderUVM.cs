namespace POWStudio.Models.ViewModels;

public class OrderUVM
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);
    public List<OrderUListItemVM> OrderListItemVms { get; set; }
}