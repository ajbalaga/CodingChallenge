using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.core.Interfaces
{
    public interface IApiKeyService
    {
        bool ValidateApiKey(string apiKey);
    }
}
