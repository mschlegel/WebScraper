using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace DataSource
{
    public class YahooFinance : DataSourceBase
    {
        //public String ExtractedContent { get; set; }
        protected override String BaseUrl { get { return "http://finance.yahoo.com/q/hl?s="; } }
        public override IEnumerable<Holding> GetTopHoldings() { return new List<Holding>(); }
    }
}
