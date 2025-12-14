namespace POWStudio.Models.ViewModels;

public class ReportSaleVM
{
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly Month { get; set; } =  DateOnly.FromDateTime(DateTime.Now);
    public int Year { get; set; }
    public decimal DateRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public decimal YearRevenue { get; set; }
    public decimal AllTimeRevenue { get; set; }
}