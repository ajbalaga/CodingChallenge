using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.core.Models
{
    public class JSONModel
    {
        public PersonalDetails data;
    }
    public class PersonalDetails {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Rank { get; set; }
        public string Tenure { get; set; }
        public string CityLocation { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Team { get; set; }

    }
}
