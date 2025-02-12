using DSPTest_WebService.Models;
using DSPTest_WebService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DSPTest_WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerUsageController : ControllerBase
    {
        private readonly PowerUsageService powerUsageService;

        public PowerUsageController(PowerUsageService powerUsageService)
        {
            this.powerUsageService = powerUsageService;
        }

        [HttpGet("{customerName}/regular-usage")]
        public async Task<IActionResult> GetCustomerUsage(string customerName)
        {
            try
            {
                var data = await powerUsageService.GetCustomerUsageData(customerName);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching data: {ex.Message}");
            }
        }

        [HttpGet("{customerName}/peaks-count")]
        public async Task<IActionResult> GetPeakCount(string customerName)
        {
            try
            {
                var data = await powerUsageService.GetCustomerUsageData(customerName);
                var peakCount = powerUsageService.CalculatePeaks(data.Select(x => x.Consumption).ToList());
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
                var data = await powerUsageService.GetCustomerUsageData(customerName);
                var highestPeak = powerUsageService.FindHighestPeak(data.Select(x => x.Consumption).ToList());
                return Ok(highestPeak);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error calculating highest peak: {ex.Message}");
            }
        }

        [HttpGet("{customerName}/hourly-consumption")]
        public async Task<IActionResult> GetHourlyPowerConsumption(string customerName)
        {
            try
            {
                var hourlyConsumption = await powerUsageService.GetHourlyPowerConsumption(customerName);
                return Ok(hourlyConsumption);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error calculating hourly power consumption: {ex.Message}");
            }
        }
    }
}
