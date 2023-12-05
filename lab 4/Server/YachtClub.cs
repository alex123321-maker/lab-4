using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [Serializable]
    internal class YachtClub
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public int NumberOfYachts { get; set; }
        public int NumberOfPlaces { get; set; }
        public bool HasPool { get; set; }
        
    }
}
