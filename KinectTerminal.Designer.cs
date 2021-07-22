namespace KinectTerminal
{
    partial class KinectTerminal
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KinectTerminal));
            this.KinectIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.KinectInformationLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PoseInformationLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SceneViewer = new System.Windows.Forms.Integration.ElementHost();
            this.toggleButton1 = new ToggleButtonTestForm.ToggleButton();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // KinectIcon
            // 
            this.KinectIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("KinectIcon.Icon")));
            this.KinectIcon.Text = "notifyIcon1";
            this.KinectIcon.Visible = true;
            this.KinectIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.KinectIcon_MouseDoubleClick);
            // 
            // KinectInformationLabel
            // 
            this.KinectInformationLabel.AutoSize = true;
            this.KinectInformationLabel.Location = new System.Drawing.Point(18, 58);
            this.KinectInformationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.KinectInformationLabel.Name = "KinectInformationLabel";
            this.KinectInformationLabel.Size = new System.Drawing.Size(134, 20);
            this.KinectInformationLabel.TabIndex = 0;
            this.KinectInformationLabel.Text = "KinectInformation";
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1227, 33);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // PoseInformationLabel
            // 
            this.PoseInformationLabel.AutoSize = true;
            this.PoseInformationLabel.Location = new System.Drawing.Point(18, 297);
            this.PoseInformationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PoseInformationLabel.Name = "PoseInformationLabel";
            this.PoseInformationLabel.Size = new System.Drawing.Size(126, 20);
            this.PoseInformationLabel.TabIndex = 3;
            this.PoseInformationLabel.Text = "PoseInformation";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SceneViewer
            // 
            this.SceneViewer.Location = new System.Drawing.Point(391, 38);
            this.SceneViewer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SceneViewer.Name = "SceneViewer";
            this.SceneViewer.Size = new System.Drawing.Size(788, 800);
            this.SceneViewer.TabIndex = 15;
            this.SceneViewer.Text = "ImageViewer";
            this.SceneViewer.Child = null;
            // 
            // toggleButton1
            // 
            this.toggleButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.toggleButton1.AutoSize = true;
            this.toggleButton1.CheckedColor = System.Drawing.Color.Gray;
            this.toggleButton1.CheckedText = "Recording";
            this.toggleButton1.Image = ((System.Drawing.Image)(resources.GetObject("toggleButton1.Image")));
            this.toggleButton1.Location = new System.Drawing.Point(22, 568);
            this.toggleButton1.Name = "toggleButton1";
            this.toggleButton1.Size = new System.Drawing.Size(314, 153);
            this.toggleButton1.TabIndex = 16;
            this.toggleButton1.Text = "KinectPoseRecord";
            this.toggleButton1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toggleButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toggleButton1.UncheckedColor = System.Drawing.SystemColors.Control;
            this.toggleButton1.UncheckedText = "Stop";
            this.toggleButton1.UseVisualStyleBackColor = true;
            this.toggleButton1.Click += new System.EventHandler(this.toggleButton1_Click);
            // 
            // KinectTerminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 908);
            this.Controls.Add(this.toggleButton1);
            this.Controls.Add(this.SceneViewer);
            this.Controls.Add(this.PoseInformationLabel);
            this.Controls.Add(this.KinectInformationLabel);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "KinectTerminal";
            this.Text = "Kinect Terminal";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KinectTerminal_FormClosing);
            this.Load += new System.EventHandler(this.KinectTerminal_Load);
            this.Resize += new System.EventHandler(this.KinectTerminal_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon KinectIcon;
        private System.Windows.Forms.Label KinectInformationLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Integration.ElementHost SceneViewer;
        private System.Windows.Forms.Label PoseInformationLabel;
        private System.Windows.Forms.Timer timer1;
        private ToggleButtonTestForm.ToggleButton toggleButton1;
    }
}

