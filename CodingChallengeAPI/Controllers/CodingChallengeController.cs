using CodingChallenge.core.Interfaces;
using CodingChallengeAPI.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodingChallengeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiKeyAuth]
    public class CodingChallengeController : ControllerBase
    {
        private readonly ICodingChallengeService _codingChallengeService;
        private readonly IApiKeyService _apiKeyService;
        private readonly ILogger<CodingChallengeController> _logger;
        private static int _filesProcessedCount = 0;

        public CodingChallengeController(ICodingChallengeService codingChallengeService, IApiKeyService apiKeyService, ILogger<CodingChallengeController> logger)
        {
            _codingChallengeService = codingChallengeService ?? throw new ArgumentNullException(nameof(codingChallengeService));
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("processFile")]
        [ApiKeyAuth]
        public async Task<IActionResult> ProcessFile(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var results = new List<string>();

            try
            {
                foreach (var file in files)
                {
                    if (IsCsvFile(file))
                    {
                        // Process CSV file
                        using (var reader = new StreamReader(file.OpenReadStream()))
                        {
                            var average = await _codingChallengeService.AggregateCSV(reader, columnIndex: 1, file.FileName);
                            results.Add($"CSV File '{file.FileName}': Average Employee Count Per Department: {average}");
                        }
                    }
                    else if (IsJsonFile(file))
                    {
                        // Process JSON file
                        using (var reader = new StreamReader(file.OpenReadStream()))
                        {
                            var transformedData = await _codingChallengeService.TransformJSON(reader, file.FileName);
                            results.Add($"JSON File '{file.FileName}': {transformedData}");
                        }
                    }
                    else
                    {
                        results.Add($"Unsupported file format for '{file.FileName}'. Only CSV or JSON files are allowed.");
                    }

                    Interlocked.Increment(ref _filesProcessedCount);
                }

                _logger.LogInformation("Total files processed so far: {FilesProcessedCount}", _filesProcessedCount);
                return Ok(string.Join("\n", results));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing files.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        private bool IsCsvFile(IFormFile file)
        {
            return file.ContentType.ToLower() == "text/csv" ||
                   file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsJsonFile(IFormFile file)
        {
            return file.ContentType.ToLower() == "application/json" ||
                   file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
