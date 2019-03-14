using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ranorex.ProjectReviewer
{
    class Program
    {
        static string solutionFilePath;
        static string cat;

        static void Main(string[] args)
        {
            Console.Write("Compress Solution File Path: ");
            solutionFilePath = Console.ReadLine();
            if (solutionFilePath == "1")
                solutionFilePath = @"C:\Users\Ned\Documents\Ranorex\RanorexStudio Projects\SampleSolution\";
            if (solutionFilePath == "2")
                solutionFilePath = @"C:\Users\Sean Perrotta\Documents\Ranorex\RanorexStudio Projects\Expedia\";

            InspectTestSuites();

            //Finished
            Console.WriteLine("Finished, press any key to exit.");
            Console.ReadKey();
        }

        static string[] FindFiles(string extension) => Directory.GetFiles(solutionFilePath, $"*.{extension}", SearchOption.AllDirectories);

        static void Write(string message, string filename = null)
        {
            Console.WriteLine($"{cat}\t{message}\t{filename}");
        }

        static void InspectTestSuites()
        {
            cat = "Test Suite";
            Write("Starting inspection of test suite");

            string[] testSuites = FindFiles("rxtst");
            if (testSuites.Length <= 0)
            {
                Write("No test suites found!");
                return;
            }

            Write($"Test Suites Found: {testSuites.Length.ToString()}");

            int totalTC = 0;

            foreach (string testSuiteFile in testSuites)
            {
                XDocument testSuite = XDocument.Load(testSuiteFile);

                //Check for Setup/Teardowns
                SetupCount(testSuite);
                TeardownCount(testSuite);

                IEnumerable<XElement> tcFlatList = testSuite.Descendants("flatlistofchildren").Descendants("testcase");
                totalTC += tcFlatList.Count<XElement>();
                foreach (XElement tc in tcFlatList)
                {
                    //Check for TC descriptions
                    if (!TCContainsDescription(tc))
                        Write($"'{tc.Name.ToString()}' missing description");
                }
            }
        }

        static bool TCContainsDescription(XElement testCase)
        {
            if (testCase.HasElements)
            {
                IEnumerable<XElement> allChildElements = testCase.Elements();
                foreach (XElement element in allChildElements)
                    if (element.Name == "description" && !string.IsNullOrEmpty(element.Value))
                        return true;
            }

            return false;
        }

        static void SetupCount(XDocument testSuite)
        {
            int count = testSuite.Descendants("flatlistofchildren").Descendants("setup").Count();
            Write("Total [SETUP] regions found: " + count);
        }

        static void TeardownCount(XDocument testSuite)
        {
            int count = testSuite.Descendants("flatlistofchildren").Descendants("teardown").Count();
            Write("Total [TEARDOWN] regions found: " + count);
        }

        //Disabled Modules
        //Empty tc
        //Unused modules

        //  MODUELS
        //Long action count
        //Headers = means can split?
        //Action comments
        //non-merged keyboard actions
        //Static delays
        //Disabled Steps
        //Options items?
        //Fixed pixel mouse click lcoation
        //Empty module
        //Using {back} or shitty key presses
    }
}
