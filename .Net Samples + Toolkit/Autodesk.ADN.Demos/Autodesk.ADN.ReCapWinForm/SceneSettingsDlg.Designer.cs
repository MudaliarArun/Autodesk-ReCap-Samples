namespace Autodesk.ADN.ReCapDemo
{
    partial class SceneSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SceneSettingsForm));
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.tbSceneName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbMeshQuality = new System.Windows.Forms.ComboBox();
            this.cbMeshFormat = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(163, 168);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 7;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(244, 168);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 8;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // tbSceneName
            // 
            this.tbSceneName.Location = new System.Drawing.Point(15, 27);
            this.tbSceneName.Name = "tbSceneName";
            this.tbSceneName.Size = new System.Drawing.Size(304, 20);
            this.tbSceneName.TabIndex = 2;
            this.tbSceneName.TextChanged += new System.EventHandler(this.tbSceneName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Scene name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Mesh quality:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Mesh format:";
            // 
            // cbMeshQuality
            // 
            this.cbMeshQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMeshQuality.FormattingEnabled = true;
            this.cbMeshQuality.ItemHeight = 13;
            this.cbMeshQuality.Location = new System.Drawing.Point(15, 78);
            this.cbMeshQuality.Name = "cbMeshQuality";
            this.cbMeshQuality.Size = new System.Drawing.Size(121, 21);
            this.cbMeshQuality.TabIndex = 4;
            // 
            // cbMeshFormat
            // 
            this.cbMeshFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMeshFormat.FormattingEnabled = true;
            this.cbMeshFormat.ItemHeight = 13;
            this.cbMeshFormat.Location = new System.Drawing.Point(17, 130);
            this.cbMeshFormat.Name = "cbMeshFormat";
            this.cbMeshFormat.Size = new System.Drawing.Size(121, 21);
            this.cbMeshFormat.TabIndex = 6;
            // 
            // SceneSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 203);
            this.Controls.Add(this.cbMeshFormat);
            this.Controls.Add(this.cbMeshQuality);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSceneName);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SceneSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scene Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.TextBox tbSceneName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbMeshQuality;
        private System.Windows.Forms.ComboBox cbMeshFormat;
    }
}