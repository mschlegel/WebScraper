using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using System.Data;

namespace DataSource
{
    public interface IDataSource
    {
        String ExtractedContent { get; set;}
        void ScrapeSite(String symbol);

        IEnumerable<Holding> GetTopHoldings();  //refactor to some sort of list

        DataSet GetDataSet();
    }
}
