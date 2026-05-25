namespace RescueSystem.Application.DTOs.Report
{
    public class ActivityChartItemDTO
    {
        public string Day { get; set; } = string.Empty;
        public int Requests { get; set; }
        public int Resolved { get; set; }
    }
}
