using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIChefRemsey.Shared
{
    public class ReceptParameters
    {
        public string? Maaltijdmoment { get; set; }
        public List<Ingredient> Ingredients { get; set;} = new List<Ingredient>();

        public string? GeselecteerdeIdee { get; set; }

    }
}
