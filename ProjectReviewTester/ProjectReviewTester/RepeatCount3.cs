﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// DO NOT MODIFY THIS FILE! It is regenerated by the designer.
// All your modifications will be lost!
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;
using Ranorex.Core.Repository;

namespace ProjectReviewTester
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The RepeatCount3 recording.
    /// </summary>
    [TestModule("8a3a6c79-fec6-4523-9919-8e4e47b86878", ModuleType.Recording, 3)]
    public partial class RepeatCount3 : ITestModule
    {
        /// <summary>
        /// Holds an instance of the ProjectReviewTesterRepository repository.
        /// </summary>
        public static ProjectReviewTesterRepository repo = ProjectReviewTesterRepository.Instance;

        static RepeatCount3 instance = new RepeatCount3();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public RepeatCount3()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static RepeatCount3 Instance
        {
            get { return instance; }
        }

#region Variables

#endregion

        /// <summary>
        /// Starts the replay of the static recording <see cref="Instance"/>.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCode("Ranorex", global::Ranorex.Core.Constants.CodeGenVersion)]
        public static void Start()
        {
            TestModuleRunner.Run(Instance);
        }

        /// <summary>
        /// Performs the playback of actions in this recording.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        [System.CodeDom.Compiler.GeneratedCode("Ranorex", global::Ranorex.Core.Constants.CodeGenVersion)]
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.00;

            Init();

            Report.Log(ReportLevel.Info, "Delay", "Waiting for 500ms.", new RecordItemIndex(0));
            Delay.Duration(500, false);
            
            // Disabled Item Test
            //Report.Log(ReportLevel.Info, "User", "", new RecordItemIndex(1));
            
            // SeperatorTest
            Report.Log(ReportLevel.Info, "Section", "SeperatorTest", new RecordItemIndex(2));
            
            Report.Log(ReportLevel.Info, "User", "", new RecordItemIndex(3));
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
