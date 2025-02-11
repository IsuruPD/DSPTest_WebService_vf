using DSPTest_WebService.Models;
using Microsoft.Data.SqlClient;

namespace DSPTest_WebService.Services
{
    public class PowerUsageService
    {
        private readonly string _connectionString;

        public PowerUsageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<PowerUsage>> GetCustomerUsageDataAsync(string customerName)
        {
            var usageData = new List<PowerUsage>();
            string query = $"SELECT Time, [{customerName}] FROM tbl_customer_usage ORDER BY Time";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(1))
                {
                    usageData.Add(new PowerUsage
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
    }
}
