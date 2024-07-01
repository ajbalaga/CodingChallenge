using CodingChallenge.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.core.Interfaces
{
    public interface ICodingChallengeService
    {
        Task<double> AggregateCSV(StreamReader reader, int columnIndex, string filename);
        Task<string> TransformJSON(StreamReader reader, string filename);
    }
}
