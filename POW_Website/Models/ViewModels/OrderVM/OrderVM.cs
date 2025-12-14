namespace POWStudio.Models.ViewModels;

public class OrderVM
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);
    public List<OrderListItemVM> OrderListItemVms { get; set; }
}