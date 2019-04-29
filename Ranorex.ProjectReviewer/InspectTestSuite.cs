using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ranorex.ProjectReviewer
{
    partial class Program
    {
        static void InspectTestSuites()
        {
            //Set catagory for output file
            Writer.catagory = "TestSuite";

            //Get all TS files
            string[] testSuites = Utilities.FindFiles("rxtst");

            //Check if any TS files
            if (testSuites == null)
                return;

            //Loop all TS files
            StatTracker.totalTestSuites += testSuites.Count();
            int totalTestCases = 0;
            foreach (string testSuiteFile in testSuites)
            {
                //Create XML Reader for TS file
                XDocument testSuite = XDocument.Load(testSuiteFile);
                string testsuiteName = testSuiteFile.Split('\\').Last();

                //Check for Setup/Teardowns
                if (testSuite.Descendants("flatlistofchildren").Descendants("setup").Count() <= 0)
                    Writer.Write(testsuiteName, "No [SETUP] regions found", 2);

                if (testSuite.Descendants("flatlistofchildren").Descendants("teardown").Count() <= 0)
                    Writer.Write(testsuiteName, "No [TEARDOWN] regions found", 3);

                //Loop all TC in TS
                IEnumerable<XElement> AllFlatTestCases = testSuite.Descendants("flatlistofchildren").Descendants("testcase");
                totalTestCases += AllFlatTestCases.Count<XElement>();
                StatTracker.totalTestContainers += totalTestCases;
                foreach (XElement tc in AllFlatTestCases)
                {
                    //Check for TC descriptions
                    if (!TCContainsDescription(tc))
                        Writer.Write(testsuiteName, tc.Attribute("name").Value, "Test case is missing a description", 1);
                }

                //Check for empty test containers
                IEnumerable<XElement> allChildTestCases = testSuite.Descendants("childhierarchy").Descendants("testcase");
                foreach (XElement testCase in allChildTestCases)
                {
                    if (!testCase.Elements().Any())
                        Writer.Write(testsuiteName, testCase.Attribute("name").Value, "Testcase is empty", 2);
                }

                //Loop all Modules in TS
                IEnumerable<XElement> allFlatModules = testSuite.Descendants("flatlistofchildren").Descendants("testmodule");
                StatTracker.totalModulesUsedTestSuites += allFlatModules.Count();
                foreach (XElement module in allFlatModules)
                {
                    //Check for disabled modules 
                    XAttribute enabledAttribute = module.Attribute("enabled");
                    if (enabledAttribute == null)
                        continue;

                    if (enabledAttribute.Value == "False")
                        Writer.Write(testsuiteName, module.Attribute("name").Value, "Disabled module in test suite", 1);
                }

                //Check for test configurations (exluding default 'TestRun')
                IEnumerable<XElement> allTestConfigurations = testSuite.Descendants("testconfiguration");
                if (allTestConfigurations.Count() == 1)
                    if (allTestConfigurations.FirstOrDefault().Attribute("name").Value == "TestRun")
                        Writer.Write(testsuiteName, "Only default 'TestRun' test configuration found", 1);
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
    }
}
