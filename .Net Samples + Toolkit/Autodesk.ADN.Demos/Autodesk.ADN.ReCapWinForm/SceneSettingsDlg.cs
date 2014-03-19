/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2014 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.ADN.Toolkit.ReCap.DataContracts;

namespace Autodesk.ADN.ReCapDemo
{
    public partial class SceneSettingsForm : Form
    {
        class MeshQualityItem
        {
            string _text;

            public MeshQualityEnum Value
            {
                get;
                private set;
            }

            public MeshQualityItem(
                string text, 
                MeshQualityEnum value)
            {
                _text = text;
                Value = value;
            }

            public override string ToString()
            {
                return _text;
            }
        }

        class MeshFormatItem
        {
            string _text;

            public MeshFormatEnum Value
            {
                get;
                private set;
            }

            public MeshFormatItem(
                string text,
                MeshFormatEnum value)
            {
                _text = text;
                Value = value;
            }

            public override string ToString()
            {
                return _text;
            }
        }

        private static int _qualityIndex = 1;

        private static int _formatIndex = 5;

        private bool _saveSettings = true;


        public SceneSettingsForm(string sceneName)
        {
            InitializeComponent();

            tbSceneName.Text = sceneName;

            // Mesh Quality
            cbMeshQuality.Items.Add(
                new MeshQualityItem(
                    "Draft", 
                    MeshQualityEnum.kDraft));

            cbMeshQuality.Items.Add(
               new MeshQualityItem(
                   "Standard",
                   MeshQualityEnum.kStandard));

            cbMeshQuality.Items.Add(
               new MeshQualityItem(
                   "High",
                   MeshQualityEnum.kHigh));

            // Mesh Format
            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "3dp", 
                    MeshFormatEnum.k3dp));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Fbx",
                    MeshFormatEnum.kFbx));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Fysc",
                    MeshFormatEnum.kFysc));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Ipm",
                    MeshFormatEnum.kIpm));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Las",
                    MeshFormatEnum.kLas));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Obj",
                    MeshFormatEnum.kObj));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Rcm",
                    MeshFormatEnum.kRcm));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Rcs",
                    MeshFormatEnum.kRcs));


            cbMeshQuality.SelectedIndex = _qualityIndex;
            cbMeshFormat.SelectedIndex = _formatIndex;

            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public SceneSettingsForm(
            string sceneName, 
            int quality,
            MeshFormatEnum format)
        {
            InitializeComponent();

            _saveSettings = false;

            tbSceneName.Text = sceneName;
            tbSceneName.ReadOnly = true;

            // Mesh Quality
            switch (quality)
            {
                case 7:

                    cbMeshQuality.Items.Add(
                        new MeshQualityItem(
                            "Draft",
                            MeshQualityEnum.kDraft));
                    break;

                case 9:

                    cbMeshQuality.Items.Add(
                       new MeshQualityItem(
                           "High",
                           MeshQualityEnum.kHigh));

                    break;

                case 8:
                default:

                    cbMeshQuality.Items.Add(
                      new MeshQualityItem(
                          "Standard",
                          MeshQualityEnum.kStandard));

                    break;
            }

            // Mesh Format
            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "3dp",
                    MeshFormatEnum.k3dp));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Fbx",
                    MeshFormatEnum.kFbx));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Fysc",
                    MeshFormatEnum.kFysc));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Ipm",
                    MeshFormatEnum.kIpm));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Las",
                    MeshFormatEnum.kLas));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Obj",
                    MeshFormatEnum.kObj));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Rcm",
                    MeshFormatEnum.kRcm));

            cbMeshFormat.Items.Add(
                new MeshFormatItem(
                    "Rcs",
                    MeshFormatEnum.kRcs));

            cbMeshQuality.SelectedIndex = 0;
            cbMeshQuality.Enabled = false;

            switch (format)
            {
                case MeshFormatEnum.k3dp:
                    cbMeshFormat.SelectedIndex = 0;
                    break;

                case MeshFormatEnum.kFbx:
                    cbMeshFormat.SelectedIndex = 1;
                    break;

                case MeshFormatEnum.kFysc:
                    cbMeshFormat.SelectedIndex = 2;
                    break;

                case MeshFormatEnum.kIpm:
                    cbMeshFormat.SelectedIndex = 3;
                    break;

                case MeshFormatEnum.kLas:
                    cbMeshFormat.SelectedIndex = 4;
                    break;

                case MeshFormatEnum.kObj:
                    cbMeshFormat.SelectedIndex = 5;
                    break;

                case MeshFormatEnum.kRcm:
                    cbMeshFormat.SelectedIndex = 6;
                    break;

                case MeshFormatEnum.kRcs:
                    cbMeshFormat.SelectedIndex = 7;
                    break;
            }

            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public string SceneName
        {
            get
            {
                return tbSceneName.Text;
            }
        }

        public MeshQualityEnum MeshQuality
        {
            get
            { 
                var item = cbMeshQuality.SelectedItem
                    as MeshQualityItem;

                return item.Value;
            }
        }

        public MeshFormatEnum MeshFormat
        {
            get
            {
                var item = cbMeshFormat.SelectedItem
                    as MeshFormatItem;

                return item.Value;
            }
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;

            if (_saveSettings)
            {
                _qualityIndex = cbMeshQuality.SelectedIndex;

                _formatIndex = cbMeshFormat.SelectedIndex;
            }

            Close();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbSceneName_TextChanged(object sender, EventArgs e)
        {
            if (tbSceneName.Text.Length == 0)
            {
                bOK.Enabled = false;
            }
            else
            {
                bOK.Enabled = true;
            }
        }
    }
}
