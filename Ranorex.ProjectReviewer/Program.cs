using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            //TODO: Error handling of bad file path
            string[] foundFiles = Directory.GetFiles(solutionFilePath, $"*.{extension}", SearchOption.AllDirectories)
                .Where(file => !file.Contains("bin"))
                .ToArray();

            if (foundFiles.Length <= 0)
            {
                Write("ERROR", $"No {extension} files found!", 3);
                return null;
            }

            return foundFiles;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName">File name</param>
        /// <param name="message">Description of issue</param>
        /// <param name="severity">1-3 (3 needs immediate attention)</param>
        static void Write(string itemName, string message, int severity = 0)
        {
            //Write to console
            switch (severity)
            {
                case 1:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;
                case 2:
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    break;
                case 3:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.WriteLine(
                    $"{severity,-1} | " +
                    $"{writeCatagory,-15} | " +
                    $"{itemName,-50} | " +
                    $"{message,5}");
            Console.ResetColor();

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
                string testsuiteName = testSuiteFile.Split('\\').Last();

                //Check for Setup/Teardowns
                if (testSuite.Descendants("flatlistofchildren").Descendants("setup").Count() <= 0)
                    Write(testsuiteName, "No [SETUP] regions found", 2);

                if (testSuite.Descendants("flatlistofchildren").Descendants("teardown").Count() <= 0)
                    Write(testsuiteName, "No [TEARDOWN] regions found", 3);

                //Loop all TC in TS
                IEnumerable<XElement> AllFlatTestCases = testSuite.Descendants("flatlistofchildren").Descendants("testcase");
                totalTC += AllFlatTestCases.Count<XElement>();
                foreach (XElement tc in AllFlatTestCases)
                {
                    //Check for TC descriptions
                    if (!TCContainsDescription(tc))
                        Write(tc.Attribute("name").Value, "Test case is missing a description", 1);
                }

                //Check for empty test containers
                IEnumerable<XElement> allChildTestCases = testSuite.Descendants("childhierarchy").Descendants("testcase");
                foreach (XElement testCase in allChildTestCases)
                {
                    if (!testCase.Elements().Any())
                        Write($"({testsuiteName}) {testCase.Attribute("name").Value}", "Testcase is empty", 2);
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
                        Write($"({testsuiteName}) {module.Attribute("name").Value}", "Disabled module in test suite", 1);
                }

                //TODO: Check for test configurations (exluding default 'TestRun')
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
                    Write(moduleName, $"Repeat count = ({repeatCount}) (generally = 1)", 1);

                //TODO: Check if turbomode = True

                //TODO: Check Speed Factor not equal to 1

                //TODO: Check for default values on variables

                //TODO:Check for unused variable

                //Loop all actions (aka 'recorditems')
                IEnumerable<XElement> allActions = recordTable.Element("recorditems").Elements();

                //Check if Action count > 15
                if (allActions.Count() > 15)
                    Write(moduleName, $"More than 15 actions ({allActions.Count()})", 2);

                bool commentFound = false;
                foreach (XElement action in allActions)
                {
                    //TODO: Write the culprit action number

                    //Check for static delays
                    if (action.Name == "explicitdelayitem")
                        Write(moduleName, $"Static delay found ({action.Attribute("duration").Value})", 3);

                    //Check for disabled actions
                    if (action.Attribute("enabled").Value == "False")
                        Write(moduleName, $"Disabled '{action.Name}' action found", 1);

                    //Check for repo item bindings (only on recommended actions)
                    if (action.Name == "mouseitem" ||
                        action.Name == "keyitem" ||
                        action.Name == "keysequenceitem" ||
                        action.Name == "touchrecorditem" ||
                        action.Name == "swiperecorditem" ||
                        action.Name == "validationitem" ||
                        action.Name == "invokeactionrecorditem" ||
                        action.Name == "getvaluerecorditem" ||
                        action.Name == "setvaluerecorditem" ||
                        action.Name == "WaitForRecordItem" ||
                        action.Name == "closeapprecorditem")
                        if (action.Element("info") == null)
                            Write(moduleName, $"{action.Name} missing a repository item", 3);

                    //Check for report line without a message
                    if (action.Name == "loggingrecorditem")
                        if (string.IsNullOrEmpty(action.Attribute("message").Value))
                            Write(moduleName, "Empty 'Log Message' action found", 1);

                    //Check for seperators (indicating for possible module split)
                    if (action.Name == "separatoritem")
                        Write(moduleName, $"Seperator found - may be split into smaller modules (Text: {Regex.Replace(action.Element("comment").Value, @"\s+", "")})", 2);

                    //Check for any action comments (output below loop)
                    if (action.Element("comment") != null)
                        commentFound = true;

                    //Check for fixed pixel mouse action spot
                    if (action.Name == "mouseitem")
                    {
                        if (action.Attribute("loc").Value.Any(Char.IsDigit))
                        {
                            string[] xyLocations = action.Attribute("loc").Value.Split(';');
                            foreach (string location in xyLocations)
                            {
                                //Check if % based
                                if (location.Contains("."))
                                {
                                    if (float.Parse(location) > 1)
                                    {
                                        Write(moduleName, "Proportional (%) mouse click action spot > 100%", 3);
                                        break;
                                    }
                                }
                                //Absolute location
                                else
                                {
                                    Write(moduleName, "Absolute pixel mouse click action spot", 3);
                                    break;
                                }
                            }

                        }

                    }


                    //Check for mouse {down}/{up} actions

                    //Check for empty modules (no actions)

                    //Using {back} or shitty key presses\ keyup keydown

                    //Check for non-merged keyboard actions


                }
                if (!commentFound)
                    Write(moduleName, "No action comments found", 1);
            }
        }

        static void InspectRecordingModulesCS()
        {

        }

        static void InspectRecordingModulesUsercodeCS()
        {

        }


    }
}
