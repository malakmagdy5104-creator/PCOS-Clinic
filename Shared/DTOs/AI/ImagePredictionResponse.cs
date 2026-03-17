public class ImagePredictionResponse
{
    public string prediction { get; set; }
    public string confidence { get; set; }  
    public string gradcam_image { get; set; }
}