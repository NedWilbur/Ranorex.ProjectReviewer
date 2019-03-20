﻿using System;
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
            if (string.IsNullOrEmpty(solutionFilePath))
                solutionFilePath = @"..\..\..\ProjectReviewTester\";

            //Write output header
            Console.WriteLine(string.Empty);
            writeCatagory = "__Category__";
            Write("__Item__", "__Issue Description___");

            //Inspect Files
            InspectTestSuites();
            InspectRecordingModulesXML();
            InspectRecordingModulesCS();
            InspectRecordingModulesUsercodeCS();

            //Finished
            Console.WriteLine("\nFinished, press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Finds files with a specific extension. Ignores /bin folder.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>String array of files found</returns>
        static string[] FindFiles(string extension)
        {
            string[] foundFiles = Directory.GetFiles(solutionFilePath, $"*.{extension}", SearchOption.AllDirectories)
                .Where(file => !file.Contains("bin"))
                .ToArray();

            if (foundFiles.Length <= 0)
            {
                Write("ERROR", $"No {extension} files found!");
                return null;
            }

            return foundFiles;
        }
            

        /// <summary>
        /// Outputs data to console (and eventually CSV file)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="itemName"></param>
        static void Write(string itemName, string message)
        {
            Console.WriteLine(
                    $"{writeCatagory,-15} | " +
                    $"{itemName,-25} | " +
                    $"{message,5}"
                  );

            //TODO: Write to CSV file
        }

        static void InspectTestSuites()
        {
            //Set catagory for output file
            writeCatagory = "Test Suite";

            //Get all TS files
            string[] testSuites = FindFiles("rxtst");

            //Check if any TS files
            if (testSuites == null)
                return;

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
                        Write(tc.Attribute("name").Value, "Test case is missing a description");
                }

                // TO DO add empty test container name to write
                //Check for empty test containers
                IEnumerable<XElement> allChildTestCases = testSuite.Descendants("childhierarchy").Descendants("testcase");
                foreach(XElement testCase in allChildTestCases)
                {
                    //TODO - Add TS name to below message
                    if (!testCase.Elements().Any())
                        Write(testCase.Attribute("name").Value, "Testcase is empty");
                }

                //Loop all Modules
                IEnumerable<XElement> allFlatModules = testSuite.Descendants("flatlistofchildren").Descendants("testmodule");
                foreach (XElement module in allFlatModules)
                {
                    //Check for disabled modules 
                    XAttribute enabledAttribute = module.Attribute("enabled");
                    if (enabledAttribute == null)
                        continue;

                    //TODO - Add TS name to below message
                    if (enabledAttribute.Value == "False")
                        Write(module.Attribute("name").Value, "Disabled module in test suite");
                    
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
            Write("TS NAME TODO", "Total [SETUP] regions found: " + count);
        }

        /// <summary>
        /// Writes how many teardown regions exists in a TS
        /// </summary>
        /// <param name="testSuite"></param>
        static void TeardownCount(XDocument testSuite)
        {
            int count = testSuite.Descendants("flatlistofchildren").Descendants("teardown").Count();
            Write("TS NAME TODO", "Total [TEARDOWN] regions found: " + count);
        }

        //  TS
        //Empty tc
        //Unused modules

        static void InspectRecordingModulesXML()
        {
            //Set catagory for output file
            writeCatagory = "Module";

            //Get all recording modules files
            string[] recordingModules = FindFiles("rxrec");

            //Check if no modules found
            if (recordingModules == null)
                return;

            //Loop all modules
            foreach (string recordingModuleFilePath in recordingModules)
            {
                //Create XDocument/XElement
                XElement recordTable = XDocument.Load(recordingModuleFilePath).Element("recordtable");

                //Get module name
                string moduleName = recordTable.Element("codegen").Attribute("classname").Value;

                //Check Repeat Count
                int repeatCount = int.Parse(recordTable.Element("repeatcount").Value);
                if (repeatCount != 1)
                    Write(moduleName, $"Repeat count = ({repeatCount}) (generally = 1)");

                //TODO: Check if turbomode = True

                //TODO: Check Speed Factor not equal to 1

                //TODO: Check for default values on variables

                //TODO:Check for unused variable

                //Loop all actions (aka 'recorditems')
                IEnumerable<XElement> allActions = recordTable.Element("recorditems").Elements();

                //Check if Action count > 15
                if (allActions.Count() > 15)
                    Write(moduleName, $"More than 15 actions ({allActions.Count()})");

                foreach (XElement action in allActions)
                {
                    //TODO: Check for at least 1 action comment

                    //Check for static delays
                    if (action.Name == "explicitdelayitem")
                        Write(moduleName, $"Static delay found! ({action.Attribute("duration").Value})");

                    //Check for disabled actions
                    if (action.Attribute("enabled").Value == "False")
                        Write(moduleName, $"Disabled '{action.Name}' action found");

                    //TODO: Check for repo item bindings (only if required)

                    //TODO: Report line with no message

                    //TODO: Check for seperators (indicating for possible module split)
                }
            }
        }

        static void InspectRecordingModulesCS()
        {

        }

        static void InspectRecordingModulesUsercodeCS()
        {

        }

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
