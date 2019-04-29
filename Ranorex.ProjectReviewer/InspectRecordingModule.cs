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
        static void InspectRecordingModules()
        {
            //Set catagory for output file
            Writer.catagory = "RecordingModule";

            //Get all recording modules files
            string[] recordingModules = Utilities.FindFiles("rxrec");

            //Check if no modules found
            if (recordingModules == null)
            {
                Writer.Write(Writer.catagory, "No recording modules found", 3);
                return;
            }

            //Loop all modules
            StatTracker.totalRecordingModules += recordingModules.Count();
            foreach (string recordingModuleFilePath in recordingModules)
            {
                //Create XDocument/XElement
                XElement recordTable = XDocument.Load(recordingModuleFilePath).Element("recordtable");

                //Get module name
                string moduleName = recordTable.Element("codegen").Attribute("classname").Value;

                //Check Repeat Count
                int repeatCount = int.Parse(recordTable.Element("repeatcount").Value);
                if (repeatCount != 1)
                    Writer.Write(moduleName, $"Repeat count = ({repeatCount}) (generally = 1)", 1);

                //Check if turbomode = True
                if (recordTable.Element("turbomode").Value.Contains("True"))
                    Writer.Write(moduleName, "TurboMode Enabled", 1);

                //Check Speed Factor not equal to 1
                float speedFactor = float.Parse(recordTable.Element("speedfactor").Value);
                if (speedFactor != 1)
                    Writer.Write(moduleName, $"Speed Factor = ({speedFactor}) (generally = 1)", 1);

                //TODO: Check for default values on variables
                //TODO: Check for unused variable

                //Loop all actions (aka 'recorditems')
                IEnumerable<XElement> allActions = recordTable.Element("recorditems").Elements();
                bool commentFound = false;

                //Check if Action count > 15
                if (allActions.Count() > 15)
                    Writer.Write(moduleName, $"More than 15 actions ({allActions.Count()})", 2);

                //Check for empty modules (no actions)
                if (allActions.Count() <= 0)
                {
                    Writer.Write(moduleName, $"Empty module with no actions", 2);
                    commentFound = true; //No need to warn on comments if 0 actions found
                }

                //Loop all actions
                StatTracker.totalActions += allActions.Count();
                int actionNumber = 1;//Used for logging
                foreach (XElement action in allActions)
                {
                    //Check for static delays
                    if (action.Name == "explicitdelayitem")
                        Writer.Write(moduleName + $" [#{actionNumber}]", $"Static delay found ({action.Attribute("duration").Value})", 3);

                    //Check for disabled actions
                    if (action.Attribute("enabled").Value == "False")
                        Writer.Write(moduleName + $" [#{actionNumber}]", $"Disabled '{action.Name}' action found", 1);

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
                            Writer.Write(moduleName + $" [#{actionNumber}]", $"{action.Name} missing a repository item", 3);

                    //Check for report line without a message
                    if (action.Name == "loggingrecorditem")
                        if (string.IsNullOrEmpty(action.Attribute("message").Value))
                            Writer.Write(moduleName + $" [#{actionNumber}]", "Empty 'Log Message' action found", 1);

                    //Check for seperators (indicating for possible module split)
                    if (action.Name == "separatoritem")
                        Writer.Write(moduleName + $" [#{actionNumber}]", $"Seperator found - may be split into smaller modules (Text: {Utilities.CleanWhiteSpace(action.Element("comment").Value)}", 2);

                    //Check for any action comments (output below loop)
                    if (action.Element("comment") != null)
                        commentFound = true;

                    //Mouse Action Checks
                    if (action.Name == "mouseitem")
                    {
                        //Check for mouse {down}/{up} actions
                        if (action.Attribute("action").Value == "Up")
                            Writer.Write(moduleName + $" [#{actionNumber}]", "{Mouse-Up} action found", 3);

                        if (action.Attribute("action").Value == "Down")
                            Writer.Write(moduleName + $" [#{actionNumber}]", "{Mouse-Down} action found", 3);

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
                                        Writer.Write(moduleName + $" [#{actionNumber}]", "Proportional (%) mouse click action spot > 100%", 3);
                                        break;
                                    }
                                }
                                //Absolute location
                                else
                                {
                                    Writer.Write(moduleName + $" [#{actionNumber}]", "Absolute pixel mouse click action spot", 3);
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
                            Writer.Write(moduleName + $" [#{actionNumber}]", "{} found in keysequence found (possible issue)", 2);

                        //Check for non-merged keyboard actions
                        XElement nextAction = action.ElementsAfterSelf().FirstOrDefault();
                        if (nextAction != null)
                            if (nextAction.Name == "keysequenceitem")
                                Writer.Write(moduleName + $" [#{actionNumber}]", "Non-merged keyboard action found", 2);
                    }

                    //Used for logging
                    actionNumber++;
                }

                if (!commentFound)
                    Writer.Write(moduleName, "No action comments found", 1);
            }
        }
    }
}
