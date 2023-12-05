using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class YachtClubContext : DbContext
    {
        public YachtClubContext()
            : base("DBConnection")
        { }
        public DbSet<YachtClub> YachtClubs { get; set; }
    }
}
