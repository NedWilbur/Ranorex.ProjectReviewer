using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranorex.ProjectReviewer
{
    public static class StatTracker
    {
        //Finding Counts
        public static int totalMinorFindings { get; set; } = 0;
        public static int totalMajorFindings { get; set; } = 0;
        public static int totalSevereFindings { get; set; } = 0;

        //File Counts
        public static int totalTestSuites { get; set; } = 0;
        public static int totalRecordingModules { get; set; } = 0;
        public static int totalRepositorys { get; set; } = 0;

        //Specific Counts
        public static int totalModulesUsedTestSuites { get; set; } = 0;
        public static int totalTestContainers { get; set; } = 0;
        public static int totalActions { get; set; } = 0;
        public static int totalRepoItems { get; set; } = 0;

        /// <summary>
        /// Writes stats to console/output file via Write(...) method
        /// </summary>
        public static void WriteStats()
        {
            //Set Catagory
            Program.writeCatagory = "StatTracker";

            Program.WriteStat("totalMinorFindings", totalMinorFindings);
            Program.WriteStat("totalMajorFindings", totalMajorFindings);
            Program.WriteStat("totalSevereFindings",  totalSevereFindings);

            Program.WriteStat("totalTestSuites", totalTestSuites);
            Program.WriteStat("totalModulesUsedTestSuites", totalModulesUsedTestSuites);

            Program.WriteStat("totalRecordingModules", totalRecordingModules);

            Program.WriteStat("totalRepositorys", totalRepositorys);
            Program.WriteStat("totalRepoItems", totalRepoItems);
        }
    }
}
