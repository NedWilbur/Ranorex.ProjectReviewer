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
using System.Drawing;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace ProjectReviewTester
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    /// The class representing the ProjectReviewTesterRepository element repository.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Ranorex", global::Ranorex.Core.Constants.CodeGenVersion)]
    [RepositoryFolder("d1f376e7-b334-4b42-b75b-58eca915fdc9")]
    public partial class ProjectReviewTesterRepository : RepoGenBaseFolder
    {
        static ProjectReviewTesterRepository instance = new ProjectReviewTesterRepository();
        ProjectReviewTesterRepositoryFolders.FormRunAppFolder _formrun;

        /// <summary>
        /// Gets the singleton class instance representing the ProjectReviewTesterRepository element repository.
        /// </summary>
        [RepositoryFolder("d1f376e7-b334-4b42-b75b-58eca915fdc9")]
        public static ProjectReviewTesterRepository Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Repository class constructor.
        /// </summary>
        public ProjectReviewTesterRepository() 
            : base("ProjectReviewTesterRepository", "/", null, 0, false, "d1f376e7-b334-4b42-b75b-58eca915fdc9", ".\\RepositoryImages\\ProjectReviewTesterRepositoryd1f376e7.rximgres")
        {
            _formrun = new ProjectReviewTesterRepositoryFolders.FormRunAppFolder(this);
        }

#region Variables

        string _UnusedVariable = "123";

        /// <summary>
        /// Gets or sets the value of variable UnusedVariable.
        /// </summary>
        [TestVariable("b4297d22-ea90-4495-8075-5fb09d010267")]
        public string UnusedVariable
        {
            get { return _UnusedVariable; }
            set { _UnusedVariable = value; }
        }

        string _NoDefaultValue = "";

        /// <summary>
        /// Gets or sets the value of variable NoDefaultValue.
        /// </summary>
        [TestVariable("d4293ca2-d3ff-4b36-95dd-5a26b65adb6f")]
        public string NoDefaultValue
        {
            get { return _NoDefaultValue; }
            set { _NoDefaultValue = value; }
        }

#endregion

        /// <summary>
        /// The Self item info.
        /// </summary>
        [RepositoryItemInfo("d1f376e7-b334-4b42-b75b-58eca915fdc9")]
        public virtual RepoItemInfo SelfInfo
        {
            get
            {
                return _selfInfo;
            }
        }

        /// <summary>
        /// The FormRun folder.
        /// </summary>
        [RepositoryFolder("a38b661d-e88e-4815-b34f-4c78943c358c")]
        public virtual ProjectReviewTesterRepositoryFolders.FormRunAppFolder FormRun
        {
            get { return _formrun; }
        }
    }

    /// <summary>
    /// Inner folder classes.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Ranorex", global::Ranorex.Core.Constants.CodeGenVersion)]
    public partial class ProjectReviewTesterRepositoryFolders
    {
        /// <summary>
        /// The FormRunAppFolder folder.
        /// </summary>
        [RepositoryFolder("a38b661d-e88e-4815-b34f-4c78943c358c")]
        public partial class FormRunAppFolder : RepoGenBaseFolder
        {
            RepoItemInfo _buttonokInfo;
            RepoItemInfo _text1001Info;
            RepoItemInfo _longnamelongnamelongnamelongnameInfo;
            RepoItemInfo _n45timeoutInfo;
            RepoItemInfo _n15timeoutInfo;

            /// <summary>
            /// Creates a new FormRun  folder.
            /// </summary>
            public FormRunAppFolder(RepoGenBaseFolder parentFolder) :
                    base("FormRun", "/form[@title='Run']", parentFolder, 30000, null, true, "a38b661d-e88e-4815-b34f-4c78943c358c", "")
            {
                _buttonokInfo = new RepoItemInfo(this, "ButtonOK", "button[@text='OK']", 30000, null, "c1ff9891-fdbc-4c63-b962-131f0add012c");
                _text1001Info = new RepoItemInfo(this, "Text1001", "?/?/text[@controlid='1001']", 30000, null, "0e6d114b-87cc-4f3b-88ac-aa7621068ab8");
                _longnamelongnamelongnamelongnameInfo = new RepoItemInfo(this, "LongNameLongNameLongNameLongName", "?/?/text[@controlid=$NoDefaultValue]", 30000, null, "3a5ed404-da49-42f3-9430-dc04ba142efe");
                _n45timeoutInfo = new RepoItemInfo(this, "N45Timeout", "?/?/text[@controlid='1001']", 45000, null, "b7c269d8-6b29-4a76-8fc2-faf84f926bfe");
                _n15timeoutInfo = new RepoItemInfo(this, "N15Timeout", "?/?/text[@controlid='1001']", 15000, null, "7e41465a-52d5-40ec-9414-ee38624baeb1");
            }

            /// <summary>
            /// The Self item.
            /// </summary>
            [RepositoryItem("a38b661d-e88e-4815-b34f-4c78943c358c")]
            public virtual Ranorex.Form Self
            {
                get
                {
                    return _selfInfo.CreateAdapter<Ranorex.Form>(true);
                }
            }

            /// <summary>
            /// The Self item info.
            /// </summary>
            [RepositoryItemInfo("a38b661d-e88e-4815-b34f-4c78943c358c")]
            public virtual RepoItemInfo SelfInfo
            {
                get
                {
                    return _selfInfo;
                }
            }

            /// <summary>
            /// The ButtonOK item.
            /// </summary>
            [RepositoryItem("c1ff9891-fdbc-4c63-b962-131f0add012c")]
            public virtual Ranorex.Button ButtonOK
            {
                get
                {
                    return _buttonokInfo.CreateAdapter<Ranorex.Button>(true);
                }
            }

            /// <summary>
            /// The ButtonOK item info.
            /// </summary>
            [RepositoryItemInfo("c1ff9891-fdbc-4c63-b962-131f0add012c")]
            public virtual RepoItemInfo ButtonOKInfo
            {
                get
                {
                    return _buttonokInfo;
                }
            }

            /// <summary>
            /// The Text1001 item.
            /// </summary>
            [RepositoryItem("0e6d114b-87cc-4f3b-88ac-aa7621068ab8")]
            public virtual Ranorex.Text Text1001
            {
                get
                {
                    return _text1001Info.CreateAdapter<Ranorex.Text>(true);
                }
            }

            /// <summary>
            /// The Text1001 item info.
            /// </summary>
            [RepositoryItemInfo("0e6d114b-87cc-4f3b-88ac-aa7621068ab8")]
            public virtual RepoItemInfo Text1001Info
            {
                get
                {
                    return _text1001Info;
                }
            }

            /// <summary>
            /// The LongNameLongNameLongNameLongName item.
            /// </summary>
            [RepositoryItem("3a5ed404-da49-42f3-9430-dc04ba142efe")]
            public virtual Ranorex.Text LongNameLongNameLongNameLongName
            {
                get
                {
                    return _longnamelongnamelongnamelongnameInfo.CreateAdapter<Ranorex.Text>(true);
                }
            }

            /// <summary>
            /// The LongNameLongNameLongNameLongName item info.
            /// </summary>
            [RepositoryItemInfo("3a5ed404-da49-42f3-9430-dc04ba142efe")]
            public virtual RepoItemInfo LongNameLongNameLongNameLongNameInfo
            {
                get
                {
                    return _longnamelongnamelongnamelongnameInfo;
                }
            }

            /// <summary>
            /// The N45Timeout item.
            /// </summary>
            [RepositoryItem("b7c269d8-6b29-4a76-8fc2-faf84f926bfe")]
            public virtual Ranorex.Text N45Timeout
            {
                get
                {
                    return _n45timeoutInfo.CreateAdapter<Ranorex.Text>(true);
                }
            }

            /// <summary>
            /// The N45Timeout item info.
            /// </summary>
            [RepositoryItemInfo("b7c269d8-6b29-4a76-8fc2-faf84f926bfe")]
            public virtual RepoItemInfo N45TimeoutInfo
            {
                get
                {
                    return _n45timeoutInfo;
                }
            }

            /// <summary>
            /// The N15Timeout item.
            /// </summary>
            [RepositoryItem("7e41465a-52d5-40ec-9414-ee38624baeb1")]
            public virtual Ranorex.Text N15Timeout
            {
                get
                {
                    return _n15timeoutInfo.CreateAdapter<Ranorex.Text>(true);
                }
            }

            /// <summary>
            /// The N15Timeout item info.
            /// </summary>
            [RepositoryItemInfo("7e41465a-52d5-40ec-9414-ee38624baeb1")]
            public virtual RepoItemInfo N15TimeoutInfo
            {
                get
                {
                    return _n15timeoutInfo;
                }
            }
        }

    }
#pragma warning restore 0436
}
