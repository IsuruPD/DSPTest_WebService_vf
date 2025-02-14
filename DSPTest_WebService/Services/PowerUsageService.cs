using DSPTest_WebService.Models;
using Microsoft.Data.SqlClient;

namespace DSPTest_WebService.Services
{
    public class PowerUsageService
    {
        private string connectionString;

        public PowerUsageService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<EnergyUsage>> GetCustomerUsageData(string customerName)
        {
            var usageData = new List<EnergyUsage>();
            string query = $"SELECT Time, [{customerName}] FROM tbl_customer_usage ORDER BY Time";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(1))
                {
                    usageData.Add(new EnergyUsage
                    {
                        Time = Convert.ToDateTime(reader[0]),
                        Consumption = Convert.ToDouble(reader[1])
                    });
                }
            }

            return usageData;
        }

        public int CalculatePeaks(List<double> data)
        {
            int peakCount = 0;
            for (int i = 1; i < data.Count - 1; i++)
            {
                if (data[i] > data[i - 1] && data[i] > data[i + 1])
                {
                    peakCount++;
                }
            }
            return peakCount;
        }

        public double FindHighestPeak(List<double> data)
        {
            double maxPeak = double.MinValue;
            for (int i = 1; i < data.Count - 1; i++)
            {
                if (data[i] > data[i - 1] && data[i] > data[i + 1])
                {
                    maxPeak = Math.Max(maxPeak, data[i]);
                }
            }
            return maxPeak == double.MinValue ? 0 : maxPeak;
        }

        public async Task<List<HourlyPowerConsumption>> GetHourlyPowerConsumption(string customerName)
        {
            var usageData = await GetCustomerUsageData(customerName);
            var hourlyPower = new Dictionary<int, double>();

            for (int i = 0; i < usageData.Count - 1; i++)
            {
                DateTime t1 = usageData[i].Time;
                DateTime t2 = usageData[i + 1].Time;
                double p1 = usageData[i].Consumption;
                double p2 = usageData[i + 1].Consumption;

                double timeDiff = (t2 - t1).TotalMinutes / 60.0;

                double hourlypower = ((p1 + p2) / 2) * timeDiff;

                int hour = t1.Hour;
                if (!hourlyPower.ContainsKey(hour))
                {
                    hourlyPower[hour] = 0;
                }
                hourlyPower[hour] += hourlypower;
            }

            return hourlyPower
                .Select(hp => new HourlyPowerConsumption
                {
                    Hour = hp.Key,
                    PowerConsumption = Math.Round(hp.Value, 2)
                })
                .OrderBy(x => x.Hour)
                .ToList();
        }
    }
}
