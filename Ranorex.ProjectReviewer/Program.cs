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
            //Set Filename
            Writer.csvFilePath = "ProjectAnalysis.csv";

            //Delete existing csv file
            if (File.Exists(Writer.csvFilePath))
                File.Delete(Writer.csvFilePath);

            //Set Console Width
            try
            {
                Console.WindowWidth = 150;
            }
            catch (Exception)
            {
                Console.WindowWidth = Console.LargestWindowWidth;
            }
                
            //Get Solution File Path
            Console.Write("Compress Solution File Path: ");
            solutionFilePath = Console.ReadLine();
            if (string.IsNullOrEmpty(solutionFilePath)) //Used for dev/debug
                solutionFilePath = @"..\..\..\ProjectReviewTester\";

            //Write header
            Console.WriteLine(string.Empty);
            Writer.catagory = "__Category__";
            Writer.Write("__Item__", "__Item2__", "__Issue Description___");

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
