using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class PortfolioDbContext : DbContext
    {
        public PortfolioDbContext()
            : base("PortfolioDb")
        {
            
        }

        public IDbSet<PortfolioApplication> Applications { get; set; }
    }
}