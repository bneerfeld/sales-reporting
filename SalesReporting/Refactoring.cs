// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Text.Json;
// using System.ComponentModel;
//
// public class ReportService
// {
//     public void Create(int year, int month)
//     {
//         var allSales = JsonSerializer.Deserialize<List<SaleObject>>(File.ReadAllText("sales.json"));
//
//         var filtered = allSales.Where(s => s.Date.Year == year && s.Date.Month == month).ToList();
//         double total;
//         foreach (var sale in filtered)
//         {
//             if(sale.Currency == "USD")
//             {
//                 total = total + sale.Amount;
//             }
//
//             if(sale.Currency == "EUR")
//             {
//                 total = total + sale.Amount * 1.1;
//             }
//
//             if(sale.Currency == "GBP")
//             {
//                 total = total + sale.Amount * 1.3;
//             }
//         }
//
//         var reportRows = new List<string>
//         {
//             $"Monatlicher Verkaufsbericht ({month}/{year}",
//             $"-------------------------------------------",
//             $"Gesamt Umsatz in USD: {total}"
//         };
//
//         File.WriteAllLines($"report_{year}_{month}.txt", reportRows);
//         Console.WriteLine("Report generated successfully");
//     }
// }
//
// public class SaleObject
// {
//     public DateTime Date { get; set; }
//     public double Amount { get; set; }
//     public string Currency { get; set; }
// }