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
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.ADN.Toolkit.OAuth;
using Autodesk.ADN.Toolkit.ReCap;
using Autodesk.ADN.Toolkit.ReCap.DataContracts;
using Autodesk.ADN.Toolkit.UI;

namespace Autodesk.ADN.ReCapDemo
{
    public partial class MainForm : Form
    {  
        private AdnOAuthConnector _connector;
        private AdnReCapClient _reCapClient;
        
        private bool _isLoggedIn;

        private Dictionary<string, SceneProgressForm>
            _progressMap;

        public MainForm()
        {
            InitializeComponent();

            _isLoggedIn = false;

            _TreeViewScenes.AfterSelect += 
                TreeViewScenes_AfterSelect;

            _TreeViewScenes.NodeMouseClick += 
                TreeViewScenes_NodeMouseClick;

            _TreeViewScenes.AfterExpand +=
               TreeViewScenes_AfterExpand;

            _TreeViewScenes.AfterCollapse += 
                TreeViewScenes_AfterCollapse;

            _ctxMenuRoot.Items[0].Click +=
               NewScene_Click;
          
            // Download scene menu item
            _ctxMenuScene.Items[0].Click += 
                DownloadSceneResult_Click;

            // Download scene as menu item
            _ctxMenuScene.Items[1].Click +=
                DownloadSceneResultAs_Click;

            // Download scene images menu item
            _ctxMenuScene.Items[2].Click +=
               DownloadSceneImages_Click;

            // Add images menu item
            _ctxMenuScene.Items[3].Click +=
                AddImages_Click;

            // Reprocess scene menu item
            _ctxMenuScene.Items[4].Click +=
               ReprocessScene_Click;

            // Refresh scene data
            _ctxMenuScene.Items[5].Click +=
               RefreshScene_Click;

            // Delete scene menu item
            _ctxMenuScene.Items[6].Click += 
                DeleteScene_Click;

            _progressMap = 
                new Dictionary<string, SceneProgressForm>();
        }

        void TreeViewScenes_AfterCollapse(
            object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
            {
                e.Node.ImageIndex = 0;
                e.Node.SelectedImageIndex = 0;
            }
        }

        void TreeViewScenes_AfterExpand(
            object sender, TreeViewEventArgs e)
        {
            if(e.Node.Tag == null)
            {
                e.Node.ImageIndex = 1;
                e.Node.SelectedImageIndex = 1;
            }
        }

        void DownloadSceneResult(ReCapPhotoscene scene)
        {
            if (scene.SceneLink == null)
            {
                LogError("Scene Link unavailable ...");
                return;
            }

            string fileName = scene.SceneName + ".zip";

            fileName = UIHelper.GetValidFileName(fileName, '-');

            fileName = UIHelper.GetSaveFileName(fileName);

            if (fileName != string.Empty)
            {
                AdnFileDownloader fd = new AdnFileDownloader(
                    scene.SceneLink,
                    fileName);

                fd.OnDownloadFileCompleted +=
                    OnDownloadFileCompleted;

                fd.Download();
            }         
        }

        void DownloadSceneResult_Click(object sender, EventArgs e)
        {
            var scene = _TreeViewScenes.SelectedNode.Tag 
                as ReCapPhotoscene;

            DownloadSceneResult(scene);
        }

        async void DownloadSceneResultAs_Click(object sender, EventArgs e)
        {
            var scene = _TreeViewScenes.SelectedNode.Tag
                as ReCapPhotoscene;

            string sceneName = scene.SceneName;

            SceneSettingsForm settingsForm = new SceneSettingsForm(
                sceneName,
                scene.MeshQuality,
                MeshFormatEnumExtensions.FromString(scene.ConvertFormat));

            if (settingsForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return; 

            var linkResult = await _reCapClient.GetPhotosceneLinkAsync(
                scene.PhotosceneId,
                settingsForm.MeshFormat);

            if (!linkResult.IsOk())
            {
                LogReCapError(linkResult.Error);
                return;
            }

            scene = linkResult.Photoscene;

            if (scene.Progress != 100.0)
            {
                LogMessage("Start processing for scene: " + sceneName);

                ShowProgressForm(sceneName, scene.PhotosceneId);
            }
            else
            {
                DownloadSceneResult(scene);
            }
        }

        async void DownloadSceneImages_Click(object sender, EventArgs e)
        {
            var folderPath = UIHelper.FolderSelect("Select folder");

            if (folderPath == null)
                return;

            var scene = _TreeViewScenes.SelectedNode.Tag
                as ReCapPhotoscene;

            var propsResult = await _reCapClient.GetPhotoscenePropertiesAsync(
               scene.PhotosceneId);

            if (!propsResult.IsOk())
            {
                LogReCapError(propsResult.Error);
                return;
            }

            var files = propsResult.Photoscene.Files;

            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileLinkResponse = await _reCapClient.GetFileLinkAsync(
                        file.FileId);

                    if (!fileLinkResponse.IsOk())
                    {
                        LogReCapError(fileLinkResponse.Error);
                        continue;
                    }

                    AdnFileDownloader fd = new AdnFileDownloader(
                        fileLinkResponse.Files[0].FileLink,
                        System.IO.Path.Combine(folderPath,file.Filename));

                    fd.OnDownloadFileCompleted +=
                        OnDownloadFileCompleted;

                    var res = await fd.DownloadAsync();
                }
            }
        }

        void OnDownloadFileCompleted(
            Uri url, 
            string location, 
            AsyncCompletedEventArgs e, 
            TimeSpan elapsed)
        {
            string dt = elapsed.ToString(@"hh\:mm\:ss"); 

            LogMessage("File download completed in " + dt);
            LogMessage(location, false);
        }

        async void AddImages_Click(object sender, EventArgs e)
        {
             var scene = _TreeViewScenes.SelectedNode.Tag 
                as ReCapPhotoscene;

             string[] files = UIHelper.FileSelect(
                   "Select Pictures",
                   "(*.jpg)|*.jpg",
                   true);

            if (files == null)
                return;

            var uploadResult = await _reCapClient.UploadFilesAsync(
                scene.PhotosceneId,
                files);

            if (!uploadResult.IsOk())
            {
                LogReCapError(uploadResult.Error);
                return;
            }

            LogMessage("Files uploaded for scene: " + scene.SceneName);
        }

        async void ReprocessScene_Click(object sender, EventArgs e)
        {
            var scene = _TreeViewScenes.SelectedNode.Tag
               as ReCapPhotoscene;

            var processResult = await _reCapClient.ProcessPhotosceneAsync(
               scene.PhotosceneId);

            if (!processResult.IsOk())
            {
                LogReCapError(processResult.Error);
                return;
            }

            LogMessage("Start processing for scene: " + scene.SceneName);

            ShowProgressForm(scene.SceneName, scene.PhotosceneId);
        }

        async void RefreshScene_Click(object sender, EventArgs e)
        {
            var scene = _TreeViewScenes.SelectedNode.Tag
               as ReCapPhotoscene;

            var sceneWithInfo = await RetrieveSceneInfo(
                scene.PhotosceneId);

            if (sceneWithInfo != null)
            {
                _TreeViewScenes.SelectedNode.Tag = sceneWithInfo;

                if (_propGrid.SelectedObject == scene)
                {
                    _propGrid.SelectedObject = sceneWithInfo;
                }

                LogMessage("Refreshed data for scene: " + 
                    scene.SceneName);
            }
            else
            {
                LogError("Failed to refresh data for scene: " + 
                    scene.SceneName);
            }
        }

        async void DeleteScene_Click(object sender, EventArgs e)
        {
            var scene = _TreeViewScenes.SelectedNode.Tag 
                as ReCapPhotoscene;

            var deleteResult = await _reCapClient.DeletePhotosceneAsync(
                scene.PhotosceneId);

            if(!deleteResult.IsOk())
            {
                LogReCapError(deleteResult.Error);
                return;
            }
            else
            {
                LogMessage("Deleted Scene: " + scene.SceneName + "\n" +
                    "Number of deleted resources: " + 
                    deleteResult.NumberOfDeletedResources);

                _TreeViewScenes.SelectedNode.Remove();
            }
        }
        
        async void NewScene_Click(object sender, EventArgs e)
        {
            SceneSettingsForm settingsForm = new SceneSettingsForm(
                "ADN - " + GetTimeStamp());

            if (settingsForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            ReCapPhotosceneOptionsBuilder options = 
                new ReCapPhotosceneOptionsBuilder(
                    settingsForm.MeshQuality,
                    settingsForm.MeshFormat);

            var id = await CreateNewPhotoscene(
                settingsForm.SceneName, 
                options);

            if (id != string.Empty)
            {
                var sceneWithInfo = await RetrieveSceneInfo(id);

                if (sceneWithInfo != null)
                {
                    var root = _TreeViewScenes.Nodes[0];

                    TreeNode node =
                        root.Nodes.Add(
                            sceneWithInfo.PhotosceneId,
                            sceneWithInfo.SceneName,
                            2, 2);

                    node.Tag = sceneWithInfo;
                }

                ShowProgressForm(settingsForm.SceneName, id);
            }
        }

        void ShowProgressForm(string sceneName, string photoSceneId)
        {
            ReCapSceneProgressNotifier notifier =
                  new ReCapSceneProgressNotifier(
                      _reCapClient,
                      photoSceneId,
                      5000);

            notifier.Activate();

            SceneProgressForm progressForm =
                new SceneProgressForm(notifier, sceneName);

            progressForm.Show(
                new WindowWrapper(this.Handle));

            notifier.OnSceneProgressCompleted +=
                new OnSceneProgressCompletedHandler(
                    OnSceneCompleted);

            _progressMap.Add(photoSceneId, progressForm);
        }

        void TreeViewScenes_NodeMouseClick(
            object sender, 
            TreeNodeMouseClickEventArgs e)
        {
            _TreeViewScenes.SelectedNode = e.Node;

            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;

            if (e.Node.Tag is ReCapPhotoscene)
            {
                _ctxMenuScene.Show(_TreeViewScenes, e.Location);
            }
            else
            {
                _ctxMenuRoot.Show(_TreeViewScenes, e.Location);
            }
        }

        void TreeViewScenes_AfterSelect(
            object sender, 
            TreeViewEventArgs e)
        {
            _propGrid.SelectedObject = e.Node.Tag;
        }

        private void OnLoginError(string problem, string msg)
        {
            LogError("Authentification Error \n" + 
                problem + ": " + msg);

            _isLoggedIn = false;

            this.Text = "A360 Storage Demo - Logged out";
        }

        private async void bLogin_Click(object sender, EventArgs e)
        {
            bool isNetworkAvailable =
               NetworkInterface.GetIsNetworkAvailable();

            if (!isNetworkAvailable)
            {
                LogError("Network Error \n" +
                    "Check your network connection and try again...");

                return;
            }

            //Already login? -> do a refresh 
            if (_isLoggedIn)
            {
                _isLoggedIn = await _connector.DoRefreshAsync();
            }
            else
            {
                _connector = new AdnOAuthConnector(
                   UserSettings.OAUTH_URL,
                   UserSettings.CONSUMER_KEY,
                   UserSettings.CONSUMER_SECRET);

                _connector.LoginViewMode = LoginViewModeEnum.iFrame;

                _connector.OnError += OnLoginError;

                _isLoggedIn = await _connector.DoLoginAsync();
            }

            if (_isLoggedIn)
            {
                bLogout.Enabled = true;

                this.Text = "ADN ReCap Demo - Logged as "
                    + _connector.UserName;

                _tbLog.Text = string.Empty;

                LogMessage("Logged in as " + _connector.UserName, true, string.Empty);

                _reCapClient = new AdnReCapClient(
                    UserSettings.RECAP_URL,
                    UserSettings.RECAP_CLIENTID,
                    _connector.ConsumerKey,
                    _connector.ConsumerSecret,
                    _connector.AccessToken,
                    _connector.AccessTokenSecret);

                var versionResponse = await _reCapClient.GetVersionAsync();

                if (!versionResponse.IsOk())
                {
                    LogReCapError(versionResponse.Error);
                }
                else
                {
                    LogMessage("Service version: " + 
                        versionResponse.Version);
                }

                var timeResponse = await _reCapClient.GetServerTimeAsync();

                if (!timeResponse.IsOk())
                {
                    LogReCapError(timeResponse.Error);
                }
                else
                {
                    LogMessage("Server time: " +
                        timeResponse.Date.ToLongDateString() + " - " +
                        timeResponse.Date.ToLongTimeString());
                }

                var res = await LoadScenes();
            }
            else
            {
                LogError("Authentication failed...");
            }
        }

        private async void bLogout_Click(object sender, EventArgs e)
        {
            bLogout.Enabled = false;

            bool res = await _connector.DoLogoutAsync();

            _connector = null;

            _isLoggedIn = false;

            this.Text = "ADN ReCap Demo - Logged out";

            LogMessage("Logged out ...");

            _TreeViewScenes.Nodes.Clear();
        }

        private async Task<ReCapPhotoscene> RetrieveSceneInfo(string photosceneId)
        {
            var scenePropsResponse = await _reCapClient.GetPhotoscenePropertiesAsync(
                photosceneId);

            if (!scenePropsResponse.IsOk())
            {
                LogReCapError(scenePropsResponse.Error);
                return null;
            }

            ReCapPhotoscene scene = scenePropsResponse.Photoscene;

            var sceneProgResponse = await _reCapClient.GetPhotosceneProgressAsync(
                photosceneId);

            if (!sceneProgResponse.IsOk())
            {
                LogReCapError(sceneProgResponse.Error);
                return null;
            }

            double progress = sceneProgResponse.Photoscene.Progress;
            string progressMsg = sceneProgResponse.Photoscene.ProgressMsg;

            Uri link = null;

            if (progress == 100.0)
            {
                var sceneLinkResponse = await _reCapClient.GetPhotosceneLinkAsync(
                    scene.PhotosceneId,
                    MeshFormatEnumExtensions.FromString(
                        scene.ConvertFormat));

                if (!sceneLinkResponse.IsOk())
                {
                    LogReCapError(sceneLinkResponse.Error);
                }
                else
                {
                    link = sceneLinkResponse.Photoscene.SceneLink;
                }
            }

            return new ReCapPhotoscene(
                scene.SceneName,
                scene.PhotosceneId,
                progressMsg,
                progress,
                link,
                scene.FileSize, 
                scene.UserId,
                scene.MeshQuality,
                scene.ConvertFormat,
                scene.ConvertStatus,
                scene.ProcessingTime,
                scene.Deleted, 
                scene.Files);
        }

        private async Task<bool> LoadScenes()
        {
            bLogin.Enabled = false;
            bLogout.Enabled = false;

            _TreeViewScenes.Nodes.Clear();

            TreeNode root = _TreeViewScenes.Nodes.Add(
               "root",
               "Photoscenes",
               0, 0);

            var sceneListResponse = await 
                _reCapClient.GetPhotosceneListAsync();

            if (!sceneListResponse.IsOk())
            {
                // We get Deserialization.Exception
                // if deleted scenes exist as Name 
                // is left empty

                if (sceneListResponse.Error.Msg !=
                    "Deserialization.Exception")
                {
                    LogReCapError(sceneListResponse.Error);

                    bLogin.Enabled = true;
                    bLogout.Enabled = true;

                    return false;
                }
            }
            
            foreach (var scene in sceneListResponse.Photoscenes)
            {
                if (!scene.Deleted)
                {
                    string name = scene.SceneName;

                    TreeNode sceneNode =
                        root.Nodes.Add(
                            scene.PhotosceneId,
                            name,
                            2, 2);

                    sceneNode.Tag = scene;

                    var sceneWithInfo = await RetrieveSceneInfo(
                        scene.PhotosceneId);

                    if (sceneWithInfo != null)
                        sceneNode.Tag = sceneWithInfo;

                    root.ExpandAll();
                }
            }

            bLogin.Enabled = true;
            bLogout.Enabled = true;

            return true;
        }

        async void OnSceneCompleted(ReCapPhotosceneResponse response)
        {
            var sceneWithInfo = await RetrieveSceneInfo(
                response.Photoscene.PhotosceneId);

            string id = response.Photoscene.PhotosceneId;

            if (_progressMap.ContainsKey(id))
            {
                _progressMap[id].Close();

                _progressMap.Remove(id);
            }

            if (sceneWithInfo != null)
            {
                LogMessage("Scene completed: " +
                    sceneWithInfo.SceneName);

                var root = _TreeViewScenes.Nodes[0];

                if (root.Nodes.ContainsKey(id))
                {
                    var node = root.Nodes[id];

                    node.Tag = sceneWithInfo;
                }
                else
                {
                    TreeNode node =
                        root.Nodes.Add(
                            sceneWithInfo.PhotosceneId,
                            sceneWithInfo.SceneName,
                            2, 2);

                    node.Tag = sceneWithInfo;
                }
            }
        }

        async Task<string> CreateNewPhotoscene(
            string sceneName,
            ReCapPhotosceneOptionsBuilder options)
        {
            string[] files = UIHelper.FileSelect(
                   "Select Pictures",
                   "(*.jpg)|*.jpg",
                   true);

            if (files == null)
                return string.Empty;

            // Step 1 - Create a new Photoscene

            var createResult = await _reCapClient.CreatePhotosceneAsync(
                sceneName,
                options);

            if (!createResult.IsOk())
            {
                LogReCapError(createResult.Error);

                return string.Empty;
            }

            LogMessage("New scene created: " + sceneName);

            // Step 2 - Upload pictures

            string photosceneId = createResult.Photoscene.PhotosceneId;

            var uploadResult = await _reCapClient.UploadFilesAsync(
                photosceneId,
                files);

            if (!uploadResult.IsOk())
            {
                LogReCapError(uploadResult.Error);

                return string.Empty;
            }

            LogMessage("Files uploaded for scene: " + sceneName);

            // Step 3 - start processing the Photoscene

            var processResult = await _reCapClient.ProcessPhotosceneAsync(
                photosceneId);

            if (!processResult.IsOk())
            {
                LogReCapError(processResult.Error);

                return string.Empty;
            }

            LogMessage("Start processing for scene: " + sceneName);

            return photosceneId;
        }

        string GetTimeStamp()
        {
            DateTime now = DateTime.Now;

            return now.ToString(
                "dd/MM/yyyy - HH:mm:ss",
                CultureInfo.InvariantCulture);
        }

        void LogReCapError(ReCapError error)
        {
            switch (error.Msg)
            {
                case "System.Net.HttpStatusCode":

                    LogError("Http Error \n" + 
                        "Status Code = " + 
                       error.StatusCode.ToString());

                    break;

                case "System.Exception":

                    LogError("Exception \n" +
                        error.Exception.Message);

                    break;

                case "Deserialization.Exception":

                    LogError("Deserialization Exception");

                    foreach (var jsonErr in error.JsonErrors)
                    {
                        LogError("\nMember: " + jsonErr.ErrorContext.Member, false);
                        LogError(jsonErr.ErrorContext.Error.Message, false);
                    }

                    break;

                default:

                    LogError("ReCap Server Error \n" +
                        error.Msg);

                    break;
            }
        }

        void LogMessage(
            string msg, 
            bool appendDateTime = true,
            string separator = "\n\n")
        {
            if (appendDateTime)
            {
                _tbLog.AppendText(
                    separator + GetTimeStamp(),
                    new Font("Microsoft San Serif", 10, FontStyle.Italic),
                    Color.Blue);
            }

            Font font = new Font(
                "Microsoft San Serif", 
                10, 
                FontStyle.Regular);

            _tbLog.AppendText(
                "\n" + msg,
                font,
                Color.Black);
        }

        void LogError(
            string msg,
            bool appendDateTime = true)
        {
            if (appendDateTime)
            {
                _tbLog.AppendText(
                    "\n\n" + GetTimeStamp(),
                    new Font(
                        "Microsoft San Serif", 
                        10, 
                        FontStyle.Italic),
                    Color.Blue);
            }

            Font font = new Font(
              "Microsoft San Serif",
              10,
              FontStyle.Regular);

            Color clr = Color.Red;

            _tbLog.AppendText(
                "\n" + msg,
                font,
                clr);
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(
            this RichTextBox box,
            string text,
            Font font,
            Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectionFont = font;

            box.AppendText(text);

            box.SelectionColor = box.ForeColor;

            // scroll to last line
            box.SelectionLength = 0;
            box.SelectionStart = box.Text.Length;
            box.ScrollToCaret();
        }
    }
}
