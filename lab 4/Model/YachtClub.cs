using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class YachtClub
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int NumberOfYachts { get; set; }
        public int NumberOfPlaces { get; set; }
        public bool HasPool { get; set; }

        public YachtClub() {
            Id = 0;
            Name = "Название";
            Address = "Улица";
            NumberOfYachts = 0;
            NumberOfPlaces = 0;
            HasPool = false;
        }
        
    }
}
