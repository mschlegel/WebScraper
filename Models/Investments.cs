using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Models
{
    public class Investments : DbContext
    {
        public Investments() : base("Investments") { }
        public DbSet<Holding> Holdings { get; set; }
    }
}
