using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using System.IO;
using System.Data;

namespace DataSource
{
    public class MorningStar : DataSourceBase
    {
        protected override String BaseUrl { get { return "http://quicktake.morningstar.com/syndication/holdings.aspx?cn=GLG117&symbol="; } }

        public override IEnumerable<Holding> GetTopHoldings()
        {
            List<String> rows = ExtractTableRows(ExtractTable(ExtractHoldingDiv())).ToList<String>();
            List<Holding> holdings = new List<Holding>(10);
            for (int i = 1; i < rows.Count; i++)
            {
                Holding h = ExtractHolding(rows[i]);
                holdings.Add(h);
            }

            return holdings;
        }

        

        private Holding ExtractHolding(string row)
        {
            var data = ExtractTableData(row);

            //need to extract additional data as well as remove any href's
            Holding h = new Holding { Fund = Fund, Name = GetName(data.ToList()[0]), Percentage = Decimal.Parse(data.ToList()[1]), Symbol = GetSymbol(data.ToList()[0]), Sector=ParseSector(data.ToList()[5]), DateCreated = DateTime.Now, DateLastModified = DateTime.Now };
            return h;
        }

        private String ParseSector(string stringToParse)
        {
            Int32 endingPosition;
            String sectorAsText = ExtractSpecificPart(stringToParse, " alt=\"", "\" />", out endingPosition);
            switch (sectorAsText)
            {
                case "technology":
                    return "Technology";
                case "communication services":
                    return "Services";
                case "consumer defensive":
                    return "Consumer Spending Non-Cyclical";
                case "financial services":
                    return "Financial";
                case "energy":
                    return "Energy";
                case "real estate":
                    return "Real Estate";
                case "consumer cyclical":
                    return "Consumer Spending Cyclical";
                case "basic materials":
                    return "Basic Materials";
                default:
                    return "N/A";
            }
        }

        private string GetName(string stringToParse)    //<span id=\"CName0\" >Oppenheimer Institutional Money Market E</span>
        {
            Int32 endingPosition;
            return ExtractSpecificPart(stringToParse, "\" >", "</span>", out endingPosition);
        }

        private string GetSymbol(string morningStarUrl)
        {
            if (morningStarUrl.StartsWith("<a href=\""))
            {
                Int32 indexOfEquals = morningStarUrl.IndexOf('=');
                String temp = morningStarUrl.Substring(indexOfEquals+1);
                indexOfEquals = temp.IndexOf('=');
                temp = temp.Substring(indexOfEquals+1);

                Int32 indexOfQuotation = temp.IndexOf("\"");
                return temp.Substring(0, indexOfQuotation);
            }
            return String.Empty;
        }

        private string ExtractHoldingDiv()
        {
            var startingIndex = ExtractedContent.IndexOf("<div class=\"syn_section_b1\">");
            var stringToParse = ExtractedContent.Substring(startingIndex);
            var sr = new StringReader(stringToParse);


            var finishingIndex = ExtractedContent.IndexOf("<div class=\"syn_text_note\">");
            var size = finishingIndex - startingIndex;
            var buffer = new char[size];
            sr.Read(buffer, 0, size);

            return new String(buffer);
        }

        private string ExtractTable(string holdingDiv)
        {
            var startingIndex = holdingDiv.IndexOf("<table class=\"syn_table_b1\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">");
            var stringToParse = holdingDiv.Substring(startingIndex);
            var sr = new StringReader(stringToParse);

            var finishingIndex = holdingDiv.IndexOf("</table>");
            var size = finishingIndex - startingIndex;
            var buffer = new char[size];

            sr.Read(buffer, 0, size);

            return new String(buffer);

        }

        private IEnumerable<String> ExtractTableData(string tableRow)
        {
            List<String> rowData = new List<String>();
            var rowPart = tableRow;
            while (rowPart.Contains("<td>") || rowPart.Contains("<td align=\"right\">"))
            {
                Int32 endingPosition;
                rowData.Add(ExtractTableDataValue(rowPart, out endingPosition));
                rowPart = rowPart.Substring(endingPosition);
            }
            return rowData;
        }

        private string ExtractTableDataValue(string rowPart, out int endingPosition)
        {
            var returnValue = ExtractSpecificPart(rowPart, "<td align=\"right\">", "</td>", out endingPosition);
            returnValue = returnValue != String.Empty ? returnValue : ExtractSpecificPart(rowPart, "<td>", "</td>", out endingPosition);
            
            char[] newline = new char[2];
            newline[0] = '\r';
            newline[1] = '\n';
            
            while (returnValue.IndexOfAny(newline) >= 0)
                returnValue = returnValue.Remove(returnValue.IndexOfAny(newline), 2).Trim();

            return returnValue;
        }

        private IEnumerable<String> ExtractTableRows(string table)
        {
            List<String> tableRows = new List<String>();
            var tablePart = table;
            while (tablePart.Contains("<tr class=\"syn_table_row"))
            {
                Int32 endingPosition;
                tableRows.Add(ExtractTableRow(tablePart, out endingPosition));
                tablePart = tablePart.Substring(endingPosition);
            }
            return tableRows;
       }

        private String ExtractTableRow(string tablePart, out Int32 endingPosition)
        {
            return ExtractSpecificPart(tablePart, "<tr class=\"syn_table_row", "</tr>", out endingPosition);
        }

        private String ExtractSpecificPart(string stringToParse, string startingText, string endingText, out Int32 endingPosition)
        {
            if (stringToParse.Contains(startingText) && stringToParse.Contains(endingText))
            {
                var startingIndex = stringToParse.IndexOf(startingText);
                var subStringtoParse = stringToParse.Substring(startingIndex + startingText.Length);
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
            }
            else
                endingPosition = -1;
            return String.Empty;
        }
    }
}
