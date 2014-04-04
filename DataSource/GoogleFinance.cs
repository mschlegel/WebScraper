using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using Models;

namespace DataSource
{
    public class GoogleFinance : DataSourceBase
    {
        protected override String BaseUrl { get { return "http://www.google.com/finance?q="; } }

        public override IEnumerable<Holding> GetTopHoldings()
        {
            List<String> rows = ExtractTableRows(ExtractTable(ExtractTop10HoldingDiv())).ToList<String>();
            List<Holding> holdings = new List<Holding>(10);
            for( int i=1; i< rows.Count; i++)
            {
                Holding h = ExtractHolding(rows[i]);
                holdings.Add(h);
            }
            
            return holdings;
        }

        private Holding ExtractHolding(string row)
        {
            Int32 endingPosition;
            var symbol = ExtractEverythingBetween(row, "<a href=\"?q=", "\" >", out endingPosition);
            var name = row.Substring(endingPosition, row.IndexOf("</a>") - endingPosition);
            var percentage = ExtractEverythingBetween(row, "<td class=data nowrap>", "%\n", out endingPosition);

            Holding h = new Holding { Fund = Fund, Name = name, Percentage = Decimal.Parse(percentage), Symbol = symbol, DateCreated = DateTime.Now, DateLastModified = DateTime.Now };
            return h;
        }

        private string ExtractTop10HoldingDiv()
        {
            var startingIndex = ExtractedContent.IndexOf("<div class=hdg><h3>Top 10 holdings</h3></div>");
            var stringToParse = ExtractedContent.Substring(startingIndex);
            var sr = new StringReader(stringToParse);


            var finishingIndex = ExtractedContent.IndexOf("Holdings Performance");
            var size = finishingIndex - startingIndex;
            var buffer = new char[size];
            sr.Read(buffer, 0, size);

            return new String(buffer);
        }

        private string ExtractTable(string top10HoldingDiv)
        {
            var startingIndex = top10HoldingDiv.IndexOf("<table border=\"0\" cellpadding=\"3\" cellspacing=\"0\" width=\"100%\">");
            var stringToParse = top10HoldingDiv.Substring(startingIndex);
            var sr = new StringReader(stringToParse);

            var finishingIndex = top10HoldingDiv.IndexOf("</table>");
            var size = finishingIndex - startingIndex;
            var buffer = new char[size];

            sr.Read(buffer, 0, size);

            return new String(buffer);

        }

        private IEnumerable<String> ExtractTableRows(string table)
        {
            List<String> tableRows = new List<String>();
            var tablePart = table;
            while (tablePart.Contains("<tr>"))
            {
                Int32 endingPosition;
                tableRows.Add(ExtractTableRow(tablePart, out endingPosition));
                tablePart = tablePart.Substring(endingPosition);
            }
            return tableRows;
        }

        private String ExtractTableRow(string tablePart, out Int32 endingPosition)
        {
            return ExtractEverythingBetween(tablePart, "<tr>", "<td>&nbsp;", out endingPosition);
        }

        private String ExtractEverythingBetween(string stringToParse, string startingText, string endingText, out Int32 endingPosition)
        {
            var startingIndex = stringToParse.IndexOf(startingText);
            var subStringtoParse= stringToParse.Substring(startingIndex + startingText.Length);
            var sr = new StringReader(subStringtoParse);

            var finishingIndex = stringToParse.IndexOf(endingText);
            endingPosition = finishingIndex + endingText.Length;

            var size = finishingIndex - startingIndex - startingText.Length;
            if (size > 0)
            {
                var buffer = new char[size];

                sr.Read(buffer, 0, size);

                return new String(buffer);
            }
            return String.Empty;
        }
    }
}
