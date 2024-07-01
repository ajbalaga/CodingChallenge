using CodingChallenge.core.Interfaces;
using CodingChallenge.core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;

namespace CodingChallenge.core.Services
{
    public class CodingChallengeService : ICodingChallengeService
    {
        private readonly ILogger<CodingChallengeService> _logger;

        public CodingChallengeService(ILogger<CodingChallengeService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<double> AggregateCSV(StreamReader reader, int columnIndex, string filename)
        {
            if (reader == null)
            {
                _logger.LogError("StreamReader is null.");
                throw new ArgumentNullException(nameof(reader));
            }

            var startTime = DateTime.Now;
            _logger.LogInformation("Starting CSV aggregation for file '{FileName}' at {StartTime}.", filename, startTime);

            try
            {
                var values = new List<double>();
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var columns = line.Split(',');

                    if (columns.Length <= columnIndex)
                    {
                        _logger.LogWarning("Line does not have enough columns: {Line}", line);
                        continue;
                    }

                    if (double.TryParse(columns[columnIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        values.Add(value);
                    }
                    else
                    {
                        _logger.LogWarning("Could not parse value to double: {Value}", columns[columnIndex]);
                    }
                }

                if (values.Count == 0)
                {
                    _logger.LogWarning("No valid values found in the specified column index.");
                    throw new InvalidOperationException("No valid values found in the specified column index.");
                }

                var average = values.Average();
                var endTime = DateTime.Now;
                _logger.LogInformation("CSV aggregation completed successfully for file '{FileName}' at {EndTime}. Processing time: {ProcessingTime} ms. Average: {Average}.",
                    filename, endTime, (endTime - startTime).TotalMilliseconds, average);

                return average;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aggregating CSV data for file '{FileName}'.", filename);
                throw new Exception($"Error aggregating CSV data for file '{filename}': {ex.Message}", ex);
            }
        }

        public async Task<string> TransformJSON(StreamReader reader, string filename)
        {
            if (reader == null)
            {
                _logger.LogError("StreamReader is null.");
                throw new ArgumentNullException(nameof(reader));
            }

            var startTime = DateTime.UtcNow;
            _logger.LogInformation("Reading JSON content for file '{FileName}' at {StartTime}.", filename, startTime);

            try
            {
                var jsonContent = await reader.ReadToEndAsync();
                var jsonData = JsonConvert.DeserializeObject<List<JSONModel>>(jsonContent);

                if (jsonData == null)
                {
                    _logger.LogWarning("Deserialized JSON data is null for file '{FileName}'.", filename);
                    throw new Exception("Deserialized JSON data is null.");
                }

                // Predefined filter condition: Name contains "John" and Age >= 30
                _logger.LogInformation("Filtering JSON data based on predefined conditions for file '{FileName}'.", filename);
                var filteredData = new List<JSONModel>();

                foreach (var item in jsonData)
                {
                    if (item.data.Name.Contains("John", StringComparison.OrdinalIgnoreCase) && item.data.Age >= 30)
                    {
                        filteredData.Add(item);
                    }
                }

                var endTime = DateTime.UtcNow;
                _logger.LogInformation("JSON data transformation completed successfully for file '{FileName}' at {EndTime}. Processing time: {ProcessingTime} ms.",
                    filename, endTime, (endTime - startTime).TotalMilliseconds);

                return JsonConvert.SerializeObject(filteredData);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Error parsing JSON data for file '{FileName}'.", filename);
                throw new Exception($"Error parsing JSON data for file '{filename}': {jsonEx.Message}", jsonEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transforming JSON data for file '{FileName}'.", filename);
                throw new Exception($"Error transforming JSON data for file '{filename}': {ex.Message}", ex);
            }
        }
    }
}
