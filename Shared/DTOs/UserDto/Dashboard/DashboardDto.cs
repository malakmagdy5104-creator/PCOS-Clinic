public class DashboardDto
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public int TotalAnalyses { get; set; }

    public List<AnalysisHistoryDto> History { get; set; }
}