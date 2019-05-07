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
            Writer.catagory = "StatTracker";

            Writer.WriteStat("totalMinorFindings", totalMinorFindings);
            Writer.WriteStat("totalMajorFindings", totalMajorFindings);
            Writer.WriteStat("totalSevereFindings",  totalSevereFindings);

            Writer.WriteStat("totalTestSuites", totalTestSuites);
            Writer.WriteStat("totalModulesUsedTestSuites", totalModulesUsedTestSuites);

            Writer.WriteStat("totalRecordingModules", totalRecordingModules);
            Writer.WriteStat("totalActions", totalActions);

            Writer.WriteStat("totalRepositorys", totalRepositorys);
            Writer.WriteStat("totalRepoItems", totalRepoItems);
        }
    }
}
