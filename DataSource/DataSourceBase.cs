using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Models;
using System.Data;

namespace DataSource
{
    public abstract class DataSourceBase : IDataSource
    {
        protected abstract String BaseUrl { get; }
        public String ExtractedContent { get; set; }
        public String Fund { get; set; }

        public void ScrapeSite(String fund)
        {
            if (String.IsNullOrEmpty(fund))
                throw new ArgumentNullException("Symbol cannot be null or empty.");

            WebClient client = new WebClient();
            client.Headers.Add("HTTP_USER_AGENT", "Custom-Web-Scraper-Agent");
            Fund = fund;

            try
            {
                // Download the web page content from the URL
                ExtractedContent = client.DownloadString(BaseUrl + fund);

                //Remove CSS styles, if any found
                //ExtractedContent = Regex.Replace(ExtractedContent, "<style(.| )*?>*</style>", "");
                //Remove script blocks
                //ExtractedContent = Regex.Replace(ExtractedContent, "<script(.| )*?>*</script>", "");
                // Remove all images
                //ExtractedContent = Regex.Replace(ExtractedContent, "<img(.| )*?/>", "");
                // Remove all HTML tags, leaving on the text inside.
                //ExtractedContent = Regex.Replace(ExtractedContent, "<(.| )*?>", "");
                // Remove all extra spaces, tabs and repeated line-breaks
                //ExtractedContent = Regex.Replace(ExtractedContent, "(x09)?", "");
                //ExtractedContent = Regex.Replace(ExtractedContent, "(x20){2,}", " ");
                //ExtractedContent = Regex.Replace(ExtractedContent, "(x0Dx0A)+", " ");
            }
            catch (Exception e)
            {
                ExtractedContent = "Error on downloading: " + e.Message;
            }
        }

        public abstract IEnumerable<Holding> GetTopHoldings();

        //QuickTake - morning star is another site we can scrape from:  http://quicktake.morningstar.com

        public DataSet GetDataSet()
        {
            // Create sample Customers table.
            DataTable table = new DataTable();
            table.TableName = "Holdings";

            // Create two columns, ID and Name.
            DataColumn idColumn = table.Columns.Add("Fund", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Symbol", typeof(string));
            table.Columns.Add("Percentage", typeof(decimal));

            // Set the ID column as the primary key column.
           // table.PrimaryKey = new DataColumn[] { idColumn };

            List<Holding> holdings = GetTopHoldings().ToList();

            foreach (Holding holding in holdings)
                table.Rows.Add(new object[] { Fund, holding.Name, holding.Symbol, holding.Percentage });

            table.AcceptChanges();

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(table);
            return dataSet;
        }
    }
}
