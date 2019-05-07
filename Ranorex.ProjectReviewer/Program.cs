using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Ranorex.ProjectReviewer
{
    partial class Program
    {
        public static string solutionFilePath;

        static void Main(string[] args)
        {
            //Try to set Console Width
            try
            {
                Console.WindowWidth = 150;
            }
            catch (Exception)
            {
                Console.WindowWidth = Console.LargestWindowWidth;
            }
                
            //Get Solution File Path
            Console.Write("Compressed Solution File Path: ");
            solutionFilePath = Console.ReadLine();
            if (string.IsNullOrEmpty(solutionFilePath)) //Used for dev/debug
                solutionFilePath = @"..\..\..\ProjectReviewTester\";

            //Set Filename & Delete if exists
            Writer.csvFilePath = $"ProjectAnalysis_{Path.GetFileName(solutionFilePath)}.csv";
            Utilities.DeleteExistingCSVFile();

            //Write header
            Writer.WriteHeader();

            //Inspect Files
            InspectTestSuites();
            InspectRecordingModules();
            InspectReposities();

            //Output Stats
            StatTracker.WriteStats();

            //Finished
            Console.WriteLine($"\nFinished, project analysis here: {Writer.csvFilePath} \n\n(press any key to exit)");
            Console.ReadKey();
        }
    }
}
