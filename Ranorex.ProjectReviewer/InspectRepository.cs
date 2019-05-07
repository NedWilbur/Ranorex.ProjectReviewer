using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ranorex.ProjectReviewer
{ 
    partial class Program
	{
        static void InspectReposities()
        {
            //Set catagory for output file
            Writer.catagory = "Repo";

            //Get all recording modules files
            string[] repositories = Utilities.FindFiles("rxrep");

            //Check if no modules found
            if (repositories == null)
            {
                Writer.Write("Repo", $"No repositories found", 3);
                return;
            }

            //Loop all repos
            StatTracker.totalRepositorys += repositories.Count();
            foreach (string repositoryFilePath in repositories)
            {
                //Create XDocument/XElement
                XDocument repo = XDocument.Load(repositoryFilePath);

                //Get Repo Name
                string RepoName = Path.GetFileNameWithoutExtension(repositoryFilePath);

                //Loop All Variables
                List<string> variables = new List<string>();
                foreach (XElement var in repo.Descendants("var"))
                {
                    //Get variable name
                    string varName = var.Attribute("name").Value;

                    //Check for variables with no default value
                    if (string.IsNullOrEmpty(var.Value))
                        Writer.Write(RepoName, varName, $"No default value for variable", 2);

                    //Check for unused variables (start)
                    variables.Add(varName);
                }

                //List of used RxPaths (Check if any items with same rxpath)
                List<string> rxPaths = new List<string>();

                //Loop all items
                foreach (XElement item in repo.Descendants("item"))
                {
                    StatTracker.totalRepoItems++;

                    //Get RxPath value
                    string RxPath = Utilities.CleanWhiteSpace(item.Value);

                    //Check Search timeout
                    int searchtimeout = int.Parse(item.Attribute("searchtimeout").Value.Replace("ms", ""));
                    if (searchtimeout > 30000)
                        Writer.Write(RepoName, item.Attribute("name").Value, $"Searchtime out > 30s", 1);
                    if (searchtimeout < 30000)
                        Writer.Write(RepoName, item.Attribute("name").Value, $"Searchtime out < 30s", 2);

                    //Check for unused variables (remove from list if found)
                    if (RxPath.Contains("$"))
                    {
                        for (int i = variables.Count - 1; i >= 0; i--)
                        {
                            if (RxPath.Contains("$" + variables[i]))
                                variables.Remove(variables[i]);
                        }
                    }

                    //Check for long names
                    if (item.Attribute("name").Value.Length > 20)
                        Writer.Write(RepoName, item.Attribute("name").Value, $"Item name > 20 characters", 1);

                    //Check if any items with same rxpath
                    string rxPath = Utilities.CleanWhiteSpace(item.Value);
                    if (rxPaths.Contains(rxPath))
                        Writer.Write(RepoName, item.Attribute("name").Value, $"RxPath already exists, duplicate item", 2);
                    else
                        rxPaths.Add(rxPath);

                    //TODO: [HARD] Check for 2+ elements with matching root RxPath
                }

                //Check for unused variables (report any not found)
                if (variables.Count > 0)
                    foreach (string var in variables)
                        Writer.Write(RepoName, var, "Unused variable found", 2);
            }
        }
    }
}
