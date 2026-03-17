public class DiagnosisResultDto
{
    public string ImagePrediction { get; set; }
    public double? Confidence { get; set; }
    public string Heatmap { get; set; }
    public string TabularPrediction { get; set; }
    public DateTime AnalysisDate { get; set; }
    public string FinalStatus { get; set; }
}