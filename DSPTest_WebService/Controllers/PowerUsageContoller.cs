using DSPTest_WebService.Models;
using DSPTest_WebService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DSPTest_WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerUsageController : ControllerBase
    {
        private readonly PowerUsageService _powerUsageService;

        public PowerUsageController(PowerUsageService powerUsageService)
        {
            _powerUsageService = powerUsageService;
        }

        [HttpGet("{customerName}/usage")]
        public async Task<IActionResult> GetCustomerUsage(string customerName)
        {
            try
            {
                var data = await _powerUsageService.GetCustomerUsageDataAsync(customerName);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching data: {ex.Message}");
            }
        }

        [HttpGet("{customerName}/peaks")]
        public async Task<IActionResult> GetPeakCount(string customerName)
        {
            try
            {
                var data = await _powerUsageService.GetCustomerUsageDataAsync(customerName);
                var peakCount = _powerUsageService.CalculatePeaks(data.Select(x => x.Consumption).ToList());
                return Ok(peakCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error calculating peaks: {ex.Message}");
            }
        }

        [HttpGet("{customerName}/highest-peak")]
        public async Task<IActionResult> GetHighestPeak(string customerName)
        {
            try
            {
                var data = await _powerUsageService.GetCustomerUsageDataAsync(customerName);
                var highestPeak = _powerUsageService.FindHighestPeak(data.Select(x => x.Consumption).ToList());
                return Ok(highestPeak);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error calculating highest peak: {ex.Message}");
            }
        }

        [HttpGet("{customerName}/analysis")]
        public async Task<IActionResult> GetCustomerAnalysis(string customerName)
        {
            try
            {
                var usageData = await _powerUsageService.GetCustomerUsageDataAsync(customerName);
                var consumptionData = usageData.Select(x => x.Consumption).ToList();

                var analysis = new CustomerPeakAnalysis
                {
                    CustomerName = customerName,
                    PeakCount = _powerUsageService.CalculatePeaks(consumptionData),
                    HighestPeak = _powerUsageService.FindHighestPeak(consumptionData),
                    UsageData = usageData
                };

                return Ok(analysis);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error performing analysis: {ex.Message}");
            }
        }
    }
}
