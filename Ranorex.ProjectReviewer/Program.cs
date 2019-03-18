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
        static string writeCatagory;

        static void Main(string[] args)
        {
            //Get Solution File Path
            Console.Write("Compress Solution File Path: ");
            solutionFilePath = Console.ReadLine();
            if (solutionFilePath == "1")
                solutionFilePath = @"C:\Users\Ned\Documents\Ranorex\RanorexStudio Projects\SampleSolution\";
            if (solutionFilePath == "2")
                solutionFilePath = @"C:\Users\Sean Perrotta\Documents\Ranorex\RanorexStudio Projects\TestSolutionForProject\TestSolutionForProject";

            //Inspect Files
            InspectTestSuites();

            //Finished
            Console.WriteLine("Finished, press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Finds files with a specific extension. Ignores /bin folder.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>String array of files found</returns>
        static string[] FindFiles(string extension) => 
            Directory.GetFiles(solutionFilePath, $"*.{extension}", SearchOption.AllDirectories)
            .Where(file => !file.Contains("bin"))
            .ToArray();

        /// <summary>
        /// Outputs data to console (and eventually CSV file)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filename"></param>
        static void Write(string message, string filename = null)
        {
            Console.WriteLine($"{writeCatagory}\t{message}\t{filename}");
            //TODO: Write to CSV file
        }


        static void InspectTestSuites()
        {
            //Set catagory for output file
            writeCatagory = "Test Suite";
            Console.WriteLine("Starting inspection of test suite");

            //Get all TS files
            string[] testSuites = FindFiles("rxtst");
            if (testSuites.Length <= 0)
            {
                Write("No test suites found!");
                return;
            }
            Console.WriteLine($"Test Suites Found: {testSuites.Length.ToString()}");

            //Loop all TS files
            int totalTC = 0;
            foreach (string testSuiteFile in testSuites)
            {
                //Create XML Reader for TS file
                XDocument testSuite = XDocument.Load(testSuiteFile);

                //Check for Setup/Teardowns
                SetupCount(testSuite);
                TeardownCount(testSuite);

                //Loop all TC in TS
                IEnumerable<XElement> AllFlatTestCases = testSuite.Descendants("flatlistofchildren").Descendants("testcase");
                totalTC += AllFlatTestCases.Count<XElement>();
                foreach (XElement tc in AllFlatTestCases)
                {
                    //Check for TC descriptions
                    if (!TCContainsDescription(tc))
                        Write($"'{tc.Name.ToString()}' missing description");
                }

                //Check for empty test containers
                //TODO:SEAN

                IEnumerable<XElement> allChildTestCases = testSuite.Descendants("childhierarchy").Descendants("testcase");
                foreach(XElement testmodule in allChildTestCases)
                {
                    XAttribute enabledId = testmodule.Attribute("id");
                    
                        Write("we got a problem");
                }

                    //Loop all Modules
                    IEnumerable<XElement> allFlatModules = testSuite.Descendants("flatlistofchildren").Descendants("testmodule");
                foreach (XElement module in allFlatModules)
                {
                    //Check for disabled modules
                    XAttribute enabledAttribute = module.Attribute("enabled");
                    if (enabledAttribute == null)
                        continue;

                    if (enabledAttribute.Value == "False")
                        Write($"{module.Attribute("name").Value} disabled!");
                }
            }
        }

        /// <summary>
        /// Checks if a TC has a description
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns>True if found, else false</returns>
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

        /// <summary>
        /// Writes how many setup regions exists in a TS
        /// </summary>
        /// <param name="testSuite"></param>
        static void SetupCount(XDocument testSuite)
        {
            int count = testSuite.Descendants("flatlistofchildren").Descendants("setup").Count();
            Write("Total [SETUP] regions found: " + count);
        }

        /// <summary>
        /// Writes how many teardown regions exists in a TS
        /// </summary>
        /// <param name="testSuite"></param>
        static void TeardownCount(XDocument testSuite)
        {
            int count = testSuite.Descendants("flatlistofchildren").Descendants("teardown").Count();
            Write("Total [TEARDOWN] regions found: " + count);
        }

        //  TS
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
