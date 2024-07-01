using CodingChallenge.core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace CodingChallenge.core.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly string _validApiKey;
        private readonly ILogger<ApiKeyService> _logger;

        public ApiKeyService(IConfiguration configuration,ILogger<ApiKeyService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _validApiKey = configuration["API_KEY"]; //if from user secrets
            //from docker
            //Environment.GetEnvironmentVariable("API_KEY");
            ValidateApiKeyConfiguration();
        }

        private void ValidateApiKeyConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_validApiKey))
            {
                const string errorMessage = "API key is not configured properly in user secrets.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }

        public bool ValidateApiKey(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("Validation attempt with an empty API key.");
                return false;
            }

            var isValid = apiKey == _validApiKey;

            if (isValid)
            {
                _logger.LogInformation("API key validated successfully.");
            }
            else
            {
                _logger.LogWarning("Invalid API key provided.");
            }

            return isValid;
        }
    }
}
