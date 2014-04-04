using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSource;
using Models;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----Google Finance-----");
            IDataSource ds = new GoogleFinance();
            ds.ScrapeSite("ODMAX");
            foreach (var holding in ds.GetTopHoldings())
            {
                Console.WriteLine(holding);
                Console.WriteLine("---------");
            }

            Console.WriteLine("------------------------");
            Console.WriteLine("-----Morning Star-----");
            ds = new MorningStar();
            ds.ScrapeSite("ODMAX");
            foreach (var holding in ds.GetTopHoldings())
            {
                Console.WriteLine(holding);
                Console.WriteLine("---------");
            }

            Console.WriteLine("------------------------");
            Console.WriteLine("-----Yahoo Finance---Not Yet Implemented--");
            ds = new YahooFinance();
            ds.ScrapeSite("ODMAX");
            foreach (var holding in ds.GetTopHoldings())
            {
                Console.WriteLine(holding);
                Console.WriteLine("---------");
            }
            Console.WriteLine("------------------------");
        }
    }
}
