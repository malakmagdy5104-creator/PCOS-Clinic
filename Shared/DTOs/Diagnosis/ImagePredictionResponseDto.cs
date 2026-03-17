using System.Text.Json.Serialization;

public class ImagePredictionResponseDto
{
    [JsonPropertyName("prediction")]
    public string prediction { get; set; }

    [JsonPropertyName("confidence")]
    public string confidence { get; set; }

    [JsonPropertyName("gradcam_image")]
    public string gradcam_image { get; set; }

    [JsonPropertyName("details")]
    public PredictionDetails details { get; set; }
}

public class PredictionDetails
{
    [JsonPropertyName("effnet_prob")]
    public string effnet_prob { get; set; }

    [JsonPropertyName("densenet_prob")]
    public string densenet_prob { get; set; }
}