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
    public partial class SceneProgressForm : Form
    {
        public SceneProgressForm(
            ReCapSceneProgressNotifier notifier,
            string sceneName)
        {
            InitializeComponent();

            labelName.Text = sceneName;

            notifier.OnSceneProgressChanged +=
                new OnSceneProgressChangedHandler(
                    OnProgressChange);

            notifier.OnSceneProgressError += 
                new OnSceneProgressErrorHandler(
                    OnError);
        }

        void OnProgressChange(ReCapPhotosceneResponse response)
        {
            string progress = response.Photoscene.Progress.ToString("F2") + " %";

            this.Text = "Photoscene Progress - " + progress;

            labelStatus.Text = response.Photoscene.ProgressMsg;

            progressBar.Value = (int)(response.Photoscene.Progress * 100);
            progressBar.LabelText = progress;
        }

        void OnError(ReCapError error)
        {
            this.Text = "Photoscene Progress - Error";

            labelStatus.Text = error.Msg;
        }
    }
}
