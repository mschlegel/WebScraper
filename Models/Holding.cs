using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Models
{

    public class Holding
    {
        public static readonly string SectorNotAvailable = "N/A";
        public static readonly string SymbolNotAvailable = "N/A";

        public Int32 Id { get; set; }
        public String Fund { get; set; }
        public String FlagReason { get; set; }
        public String Symbol { get; set; }
        public String Name { get; set; }
        public Decimal Percentage { get; set; }
        public Boolean Flag { get; set; }
        public String Sector { get; set; }
        public DateTime DateLastModified { get; set; }
        public DateTime DateCreated { get; set; }

        public override string ToString()
        {
            return String.Format("Name: {0}\nSymbol: {1}\nPercentage: {2}\nSector: {3}", 
                    Name, 
                    Symbol != String.Empty ? Symbol : SymbolNotAvailable, 
                    Percentage,
                    Sector);
        }

        public Holding()
        {
            DateCreated = DateTime.Now;
            DateLastModified = DateTime.Now;
        }

        /// <summary>
        /// Merge Holdings
        /// </summary>
        /// <param name="holding1">Holding 1</param>
        /// <param name="holding2">Holding 2</param>
        /// <returns>Merged Holding</returns>
        public static Holding operator +(Holding holding1, Holding holding2)
        {
            Holding h = new Holding();
            if (!String.IsNullOrEmpty(holding1.Fund))
                h.Fund = holding1.Fund;
            else if (!String.IsNullOrEmpty(holding2.Fund))
                h.Fund = holding2.Fund;
            else
            {
                h.Flag = true;
                h.FlagReason = "Merge: Fund options are null or empty.";
            }

            if (!String.IsNullOrEmpty(holding1.Name))
                h.Name = holding1.Name;
            else if (!String.IsNullOrEmpty(holding2.Name))
                h.Name = holding2.Name;
            else
            {
                h.Flag = true;
                h.FlagReason = "Merge: Name options are null or empty.";
            }

            if (holding1.Percentage != holding2.Percentage || holding1.Percentage == 0.0m)
            {
                h.Flag = true;
                h.FlagReason = "Merge: Percentages are different or 0.";
            }
            h.Percentage = holding1.Percentage;

            if (!String.IsNullOrEmpty(holding1.Symbol))
                h.Symbol = holding1.Symbol;
            else if (!String.IsNullOrEmpty(holding2.Symbol))
                h.Symbol = holding2.Symbol;
            else
            {
                h.Flag = true;
                h.FlagReason = "Merge: Symbol options are null or empty.";
            }
            if (!String.IsNullOrEmpty(holding1.Sector) && holding1.Sector != "N/A")
            { 
                h.Sector = holding1.Sector;
                if (!String.IsNullOrEmpty(holding2.Sector) && holding2.Sector != "N/A" && holding1.Sector != holding2.Sector)
                {
                    h.Flag = true;
                    h.FlagReason = "Merge: Sectors are different or not available.";
                }
            }
            else if (!String.IsNullOrEmpty(holding2.Sector) && holding2.Sector != "N/A")
            {
                h.Sector = holding2.Sector;
            }
            else
            {
                h.Sector = SectorNotAvailable;
            }

            return h;
        }
    }
}
