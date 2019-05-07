using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranorex.ProjectReviewer
{
    class Writer
    {
        public static string catagory { get; set; }
        public static string csvFilePath { get; set; }

        /// <summary>
        /// Writes message to console and a csv file
        /// </summary>
        /// <param name="itemName">File name</param>
        /// <param name="itemName2">Second File Name</param>
        /// <param name="message">Description of issue</param>
        /// <param name="severity">1-3 (3 needs immediate attention)</param>
        public static void Write(string itemName, string itemName2, string message, int severity = 0)
        {
            //Write to console
            switch (severity)
            {
                case 1:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    StatTracker.totalMinorFindings++;
                    break;
                case 2:
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    StatTracker.totalMajorFindings++;
                    break;
                case 3:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    StatTracker.totalSevereFindings++;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.WriteLine(
                    $"{severity,-1} | " +
                    $"{catagory,-15} | " +
                    $"{itemName,-30} | " +
                    $"{itemName2,-30} | " +
                    $"{message,5}");
            Console.ResetColor();

            //Write to CSV file
            using (StreamWriter writer = new StreamWriter(new FileStream(csvFilePath, FileMode.Append, FileAccess.Write)))
                writer.WriteLine($"{severity},{catagory},{itemName}, {itemName2},{message},");
        }

        //Alternative Overloads
        public static void Write(string itemName, string message, int severity = 0) => Write(itemName, string.Empty, message, severity);
        public static void Write(string itemName, string message) => Write(itemName, string.Empty, message, 0);
        public static void WriteError(string errorMessage, Exception ex) => Write("ERROR", string.Empty, $"{errorMessage} - {ex}", 3);
        public static void WriteStat(string type, int number) => Write(type, string.Empty, number.ToString(), 0);

        /// <summary>
        /// Write header for console & csv file
        /// </summary>
        public static void WriteHeader()
        {
            Console.WriteLine(string.Empty);
            catagory = "__Category__";
            Write("__Item__", "__Item2__", "__Issue Description___");
        }
    }
}
