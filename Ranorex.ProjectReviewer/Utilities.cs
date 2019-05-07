using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Ranorex.ProjectReviewer
{
    public static class Utilities
    {
        /// <summary>
        /// Finds files with a specific extension. Ignores /bin folder.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>String array of files found</returns>
        public static string[] FindFiles(string extension)
        {
            try
            {
                string[] foundFiles = Directory.GetFiles(Program.solutionFilePath, $"*.{extension}", SearchOption.AllDirectories)
                .Where(file => !file.Contains("bin"))
                .ToArray();

                if (foundFiles.Length <= 0)
                {
                    Writer.Write("ERROR", $"No {extension} files found!", 3);
                    return null;
                }

                return foundFiles;
            }
            catch (Exception ex)
            {
                Writer.WriteError($"ERROR: Failed to read files with extension: {extension}", ex);
                Console.ReadKey();
                throw ex;
            }
        }

        /// <summary>
        /// Cleans white space from values
        /// </summary>
        /// <param name="value">String value to clean</param>
        /// <returns>Cleaned version of string</returns>
        public static string CleanWhiteSpace(string value) => Regex.Replace(value, @"\s+", "");

        /// <summary>
        /// Delete existing csv file if it exists
        /// </summary>
        public static void DeleteExistingCSVFile()
        {
            //Delete existing csv file
            if (File.Exists(Writer.csvFilePath))
            {
                try
                {
                    File.Delete(Writer.csvFilePath);
                }
                catch (Exception ex)
                {
                    Writer.WriteError("Unable to delete existing CSV file!", ex);
                }
            }

        }
    }
}
