namespace Autodesk.ADN.ReCapDemo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.bLogout = new System.Windows.Forms.Button();
            this.bLogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this._splitContainerMain = new System.Windows.Forms.SplitContainer();
            this._splitContainerTop = new System.Windows.Forms.SplitContainer();
            this._TreeViewScenes = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this._propGrid = new Autodesk.ADN.Toolkit.UI.ReadOnlyPropertyGrid();
            this._tbLog = new System.Windows.Forms.RichTextBox();
            this._ctxMenuRoot = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this._ctxMenuScene = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DownloadScene = new System.Windows.Forms.ToolStripMenuItem();
            this.DownloadSceneAs = new System.Windows.Forms.ToolStripMenuItem();
            this.DownloadImages = new System.Windows.Forms.ToolStripMenuItem();
            this.AddImages = new System.Windows.Forms.ToolStripMenuItem();
            this.Reprocess = new System.Windows.Forms.ToolStripMenuItem();
            this.RefreshSceneData = new System.Windows.Forms.ToolStripMenuItem();
            this.Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerMain)).BeginInit();
            this._splitContainerMain.Panel1.SuspendLayout();
            this._splitContainerMain.Panel2.SuspendLayout();
            this._splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerTop)).BeginInit();
            this._splitContainerTop.Panel1.SuspendLayout();
            this._splitContainerTop.Panel2.SuspendLayout();
            this._splitContainerTop.SuspendLayout();
            this._ctxMenuRoot.SuspendLayout();
            this._ctxMenuScene.SuspendLayout();
            this.SuspendLayout();
            // 
            // bLogout
            // 
            this.bLogout.Dock = System.Windows.Forms.DockStyle.Left;
            this.bLogout.Enabled = false;
            this.bLogout.Location = new System.Drawing.Point(103, 13);
            this.bLogout.Name = "bLogout";
            this.bLogout.Size = new System.Drawing.Size(103, 32);
            this.bLogout.TabIndex = 20;
            this.bLogout.Text = "Log Out ...";
            this.bLogout.UseVisualStyleBackColor = true;
            this.bLogout.Click += new System.EventHandler(this.bLogout_Click);
            // 
            // bLogin
            // 
            this.bLogin.Dock = System.Windows.Forms.DockStyle.Left;
            this.bLogin.Location = new System.Drawing.Point(0, 13);
            this.bLogin.Name = "bLogin";
            this.bLogin.Size = new System.Drawing.Size(103, 32);
            this.bLogin.TabIndex = 19;
            this.bLogin.Text = "Log In/Refresh ...";
            this.bLogin.UseVisualStyleBackColor = true;
            this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 15;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.bLogout);
            this.panel2.Controls.Add(this.bLogin);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(896, 58);
            this.panel2.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 18;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._splitContainerMain);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(896, 637);
            this.panel1.TabIndex = 16;
            // 
            // _splitContainerMain
            // 
            this._splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainerMain.Location = new System.Drawing.Point(0, 58);
            this._splitContainerMain.Name = "_splitContainerMain";
            this._splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainerMain.Panel1
            // 
            this._splitContainerMain.Panel1.Controls.Add(this._splitContainerTop);
            // 
            // _splitContainerMain.Panel2
            // 
            this._splitContainerMain.Panel2.Controls.Add(this._tbLog);
            this._splitContainerMain.Size = new System.Drawing.Size(896, 579);
            this._splitContainerMain.SplitterDistance = 381;
            this._splitContainerMain.TabIndex = 16;
            // 
            // _splitContainerTop
            // 
            this._splitContainerTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainerTop.Location = new System.Drawing.Point(0, 0);
            this._splitContainerTop.Name = "_splitContainerTop";
            // 
            // _splitContainerTop.Panel1
            // 
            this._splitContainerTop.Panel1.Controls.Add(this._TreeViewScenes);
            // 
            // _splitContainerTop.Panel2
            // 
            this._splitContainerTop.Panel2.Controls.Add(this._propGrid);
            this._splitContainerTop.Panel2MinSize = 0;
            this._splitContainerTop.Size = new System.Drawing.Size(896, 381);
            this._splitContainerTop.SplitterDistance = 447;
            this._splitContainerTop.TabIndex = 0;
            // 
            // _TreeViewScenes
            // 
            this._TreeViewScenes.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TreeViewScenes.ImageIndex = 0;
            this._TreeViewScenes.ImageList = this.imageList1;
            this._TreeViewScenes.Indent = 22;
            this._TreeViewScenes.ItemHeight = 25;
            this._TreeViewScenes.Location = new System.Drawing.Point(0, 0);
            this._TreeViewScenes.Name = "_TreeViewScenes";
            this._TreeViewScenes.SelectedImageIndex = 0;
            this._TreeViewScenes.Size = new System.Drawing.Size(447, 381);
            this._TreeViewScenes.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder_close.png");
            this.imageList1.Images.SetKeyName(1, "folder_open.png");
            this.imageList1.Images.SetKeyName(2, "file.png");
            // 
            // _propGrid
            // 
            this._propGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._propGrid.Location = new System.Drawing.Point(0, 0);
            this._propGrid.Name = "_propGrid";
            this._propGrid.ReadOnly = true;
            this._propGrid.Size = new System.Drawing.Size(445, 381);
            this._propGrid.TabIndex = 0;
            // 
            // _tbLog
            // 
            this._tbLog.BackColor = System.Drawing.SystemColors.ControlLight;
            this._tbLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tbLog.Location = new System.Drawing.Point(0, 0);
            this._tbLog.Name = "_tbLog";
            this._tbLog.ReadOnly = true;
            this._tbLog.Size = new System.Drawing.Size(896, 194);
            this._tbLog.TabIndex = 0;
            this._tbLog.Text = "";
            // 
            // _ctxMenuRoot
            // 
            this._ctxMenuRoot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this._ctxMenuRoot.Name = "contextMenuStrip1";
            this._ctxMenuRoot.Size = new System.Drawing.Size(174, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::Autodesk.ADN.ReCapWinForm.Properties.Resources.add;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItem1.Text = "Add Photoscene ...";
            this.toolStripMenuItem1.ToolTipText = "Create a new photoscene ...";
            // 
            // _ctxMenuScene
            // 
            this._ctxMenuScene.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DownloadScene,
            this.DownloadSceneAs,
            this.DownloadImages,
            this.AddImages,
            this.Reprocess,
            this.RefreshSceneData,
            this.Delete});
            this._ctxMenuScene.Name = "_ctxMenuScene";
            this._ctxMenuScene.Size = new System.Drawing.Size(187, 158);
            // 
            // DownloadScene
            // 
            this.DownloadScene.Image = global::Autodesk.ADN.ReCapWinForm.Properties.Resources.file;
            this.DownloadScene.Name = "DownloadScene";
            this.DownloadScene.Size = new System.Drawing.Size(186, 22);
            this.DownloadScene.Text = "Download result ...";
            this.DownloadScene.ToolTipText = "Download scene result";
            // 
            // DownloadSceneAs
            // 
            this.DownloadSceneAs.Image = global::Autodesk.ADN.ReCapWinForm.Properties.Resources.file;
            this.DownloadSceneAs.Name = "DownloadSceneAs";
            this.DownloadSceneAs.Size = new System.Drawing.Size(186, 22);
            this.DownloadSceneAs.Text = "Download result as ...";
            this.DownloadSceneAs.ToolTipText = "Download scene result with different format";
            // 
            // DownloadImages
            // 
            this.DownloadImages.Image = global::Autodesk.ADN.ReCapWinForm.Properties.Resources.folder_open;
            this.DownloadImages.Name = "DownloadImages";
            this.DownloadImages.Size = new System.Drawing.Size(186, 22);
            this.DownloadImages.Text = "Download images ...";
            this.DownloadImages.ToolTipText = "Download scene images";
            // 
            // AddImages
            // 
            this.AddImages.Image = global::Autodesk.ADN.ReCapWinForm.Properties.Resources.add;
            this.AddImages.Name = "AddImages";
            this.AddImages.Size = new System.Drawing.Size(186, 22);
            this.AddImages.Text = "Add images ...";
            this.AddImages.ToolTipText = "Add extra images to scene";
            // 
            // Reprocess
            // 
            this.Reprocess.Image = global::Autodesk.ADN.ReCapWinForm.Properties.Resources.props;
            this.Reprocess.Name = "Reprocess";
            this.Reprocess.Size = new System.Drawing.Size(186, 22);
            this.Reprocess.Text = "Reprocess scene";
            this.Reprocess.ToolTipText = "Reprocess the scene";
            // 
            // RefreshSceneData
            // 
            this.RefreshSceneData.Image = global::Autodesk.ADN.ReCapWinForm.Properties.Resources.refresh;
            this.RefreshSceneData.Name = "RefreshSceneData";
            this.RefreshSceneData.Size = new System.Drawing.Size(186, 22);
            this.RefreshSceneData.Text = "Refresh scene data";
            this.RefreshSceneData.ToolTipText = "Refresh scene data from server";
            // 
            // Delete
            // 
            this.Delete.Image = global::Autodesk.ADN.ReCapWinForm.Properties.Resources.cross;
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(186, 22);
            this.Delete.Text = "Delete scene";
            this.Delete.ToolTipText = "Delete scene on server";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 637);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "ADN ReCap Demo - Logged out";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this._splitContainerMain.Panel1.ResumeLayout(false);
            this._splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerMain)).EndInit();
            this._splitContainerMain.ResumeLayout(false);
            this._splitContainerTop.Panel1.ResumeLayout(false);
            this._splitContainerTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainerTop)).EndInit();
            this._splitContainerTop.ResumeLayout(false);
            this._ctxMenuRoot.ResumeLayout(false);
            this._ctxMenuScene.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bLogout;
        private System.Windows.Forms.Button bLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer _splitContainerMain;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TreeView _TreeViewScenes;
        private System.Windows.Forms.SplitContainer _splitContainerTop;
        private Toolkit.UI.ReadOnlyPropertyGrid _propGrid;
        private System.Windows.Forms.RichTextBox _tbLog;
        private System.Windows.Forms.ContextMenuStrip _ctxMenuRoot;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip _ctxMenuScene;
        private System.Windows.Forms.ToolStripMenuItem DownloadScene;
        private System.Windows.Forms.ToolStripMenuItem Delete;
        private System.Windows.Forms.ToolStripMenuItem DownloadImages;
        private System.Windows.Forms.ToolStripMenuItem AddImages;
        private System.Windows.Forms.ToolStripMenuItem Reprocess;
        private System.Windows.Forms.ToolStripMenuItem DownloadSceneAs;
        private System.Windows.Forms.ToolStripMenuItem RefreshSceneData;

    }
}

