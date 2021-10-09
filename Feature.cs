namespace FeatureExtraction
{
    public class Feature
    {
        public string Word { get; set; }
        public int DocumentIndex { get; set; }
        public double Weight { get; set; } = 0d;
    }

}
