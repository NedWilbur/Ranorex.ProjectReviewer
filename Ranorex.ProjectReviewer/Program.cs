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
        static string csvFilePath = "project_analysis.csv";

        static void Main(string[] args)
        {
            //Delete existing csv file
            if (File.Exists(csvFilePath))
                File.Delete(csvFilePath);

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
            InspectReposities();


            //Finished
            Console.WriteLine($"\nFinished, project analysis here: {csvFilePath} \n\n(press any key to exit)");
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

            //Write to CSV file
            using (StreamWriter writer = new StreamWriter(new FileStream(csvFilePath, FileMode.Append, FileAccess.Write)))
                writer.WriteLine($"{severity},{writeCatagory},{itemName},{message},");
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
            {
                Write("Modules", $"No recording modules found", 3);
                return;
            }

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

                //Check if turbomode = True
                if (recordTable.Element("turbomode").Value.Contains("True"))
                    Write(moduleName, "TurboMode Enabled", 1);

                //Check Speed Factor not equal to 1
                float speedFactor = float.Parse(recordTable.Element("speedfactor").Value);
                if (speedFactor != 1)
                    Write(moduleName, $"Speed Factor = ({speedFactor}) (generally = 1)", 1);

                //TODO: Check for default values on variables

                //TODO: Check for unused variable

                //Loop all actions (aka 'recorditems')
                IEnumerable<XElement> allActions = recordTable.Element("recorditems").Elements();
                bool commentFound = false;

                //Check if Action count > 15
                if (allActions.Count() > 15)
                    Write(moduleName, $"More than 15 actions ({allActions.Count()})", 2);

                //Check for empty modules (no actions)
                if (allActions.Count() <= 0)
                {
                    Write(moduleName, $"Empty module with no actions", 2);
                    commentFound = true; //No need to warn on comments if 0 actions found
                }

                //Loop all actions
                int actionNumber = 1;//Used for logging
                foreach (XElement action in allActions)
                {
                    //Check for static delays
                    if (action.Name == "explicitdelayitem")
                        Write(moduleName + $" [#{actionNumber}]", $"Static delay found ({action.Attribute("duration").Value})", 3);

                    //Check for disabled actions
                    if (action.Attribute("enabled").Value == "False")
                        Write(moduleName + $" [#{actionNumber}]", $"Disabled '{action.Name}' action found", 1);

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
                            Write(moduleName + $" [#{actionNumber}]", $"{action.Name} missing a repository item", 3);

                    //Check for report line without a message
                    if (action.Name == "loggingrecorditem")
                        if (string.IsNullOrEmpty(action.Attribute("message").Value))
                            Write(moduleName + $" [#{actionNumber}]", "Empty 'Log Message' action found", 1);

                    //Check for seperators (indicating for possible module split)
                    if (action.Name == "separatoritem")
                        Write(moduleName + $" [#{actionNumber}]", $"Seperator found - may be split into smaller modules (Text: {Regex.Replace(action.Element("comment").Value, @"\s+", "")})", 2);

                    //Check for any action comments (output below loop)
                    if (action.Element("comment") != null)
                        commentFound = true;

                    //Mouse Action Checks
                    if (action.Name == "mouseitem")
                    {
                        //Check for mouse {down}/{up} actions
                        if (action.Attribute("action").Value == "Up")
                            Write(moduleName + $" [#{actionNumber}]", "Mouse-Up action found", 3);

                        if (action.Attribute("action").Value == "Down")
                            Write(moduleName + $" [#{actionNumber}]", "Mouse-Down action found", 3);

                        //Check for fixed pixel mouse action spot
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
                                        Write(moduleName + $" [#{actionNumber}]", "Proportional (%) mouse click action spot > 100%", 3);
                                        break;
                                    }
                                }
                                //Absolute location
                                else
                                {
                                    Write(moduleName + $" [#{actionNumber}]", "Absolute pixel mouse click action spot", 3);
                                    break;
                                }
                            }
                        }
                    }

                    //Keysequence Action Checks
                    if (action.Name == "keysequenceitem")
                    {
                        //Check for {} in keysequence, could mean up/down keypress or some other bad practice
                        if (action.Attribute("KeySequence").Value.Contains('{'))
                            Write(moduleName + $" [#{actionNumber}]", "{} found in keysequence found (possible issue)", 2);

                        //Check for non-merged keyboard actions
                        XElement nextAction = action.ElementsAfterSelf().FirstOrDefault();
                        if (nextAction != null)
                            if (nextAction.Name == "keysequenceitem")
                                Write(moduleName + $" [#{actionNumber}]", "Non-merged keyboard action found", 2);
                    }

                    //Used for logging
                    actionNumber++;
                }

                if (!commentFound)
                    Write(moduleName, "No action comments found", 1);
            }
        }

        static void InspectReposities()
        {
            //Set catagory for output file
            writeCatagory = "Repo";

            //Get all recording modules files
            string[] repositories = FindFiles("rxrep");

            //Check if no modules found
            if (repositories == null)
            {
                Write("Repo", $"No repositories found", 3);
                return;
            }

            //Loop all repos
            foreach (string repositoryFilePath in repositories)
            {
                //Create XDocument/XElement
                XDocument repo = XDocument.Load(repositoryFilePath);

                //TODO Get Repo Name
                string RepoName = "TODO";

                //Loop all items
                foreach (XElement item in repo.Descendants("item"))
                {
                    //Check Search timeout
                    int searchtimeout = int.Parse(item.Attribute("searchtimeout").Value.Replace("ms", ""));
                    if (searchtimeout > 30000)
                        Write($"{RepoName} - {item.Name}", $"Searchtime out > 30s", 1);
                    if (searchtimeout < 30000)
                        Write($"{RepoName} - {item.Name}", $"Searchtime out < 30s", 1);
                }

                //Check if any items with same rxpath
                //Check for long names
                //Check for 2+ elements with matching root RxPath
                //Check for unused variables
                //Check for variables with no default value
            }
        }
    }
}
