namespace DSPTest_WebService.Models
{
    public class CustomerPeakAnalysis
    {
        public string CustomerName { get; set; }
        public int PeakCount { get; set; }
        public double HighestPeak { get; set; }
        public List<PowerUsage> UsageData { get; set; }
    }
}
