namespace XMLDifferenceTool
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.LinkLabel linkLabelProject;
        private System.Windows.Forms.Button buttonOk;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tableLayoutPanelMain = new TableLayoutPanel();
            labelTitle = new Label();
            labelVersion = new Label();
            labelDescription = new Label();
            labelCopyright = new Label();
            linkLabelProject = new LinkLabel();
            buttonOk = new Button();
            tableLayoutPanelMain.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.AutoSize = true;
            tableLayoutPanelMain.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelMain.ColumnCount = 1;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelMain.Controls.Add(labelTitle, 0, 0);
            tableLayoutPanelMain.Controls.Add(labelVersion, 0, 1);
            tableLayoutPanelMain.Controls.Add(labelDescription, 0, 2);
            tableLayoutPanelMain.Controls.Add(labelCopyright, 0, 3);
            tableLayoutPanelMain.Controls.Add(linkLabelProject, 0, 4);
            tableLayoutPanelMain.Controls.Add(buttonOk, 0, 5);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(0, 0);
            tableLayoutPanelMain.Margin = new Padding(5, 5, 5, 5);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.Padding = new Padding(28, 28, 28, 28);
            tableLayoutPanelMain.RowCount = 6;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle());
            tableLayoutPanelMain.RowStyles.Add(new RowStyle());
            tableLayoutPanelMain.RowStyles.Add(new RowStyle());
            tableLayoutPanelMain.RowStyles.Add(new RowStyle());
            tableLayoutPanelMain.RowStyles.Add(new RowStyle());
            tableLayoutPanelMain.RowStyles.Add(new RowStyle());
            tableLayoutPanelMain.Size = new Size(791, 438);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            labelTitle.Location = new Point(28, 28);
            labelTitle.Margin = new Padding(0, 0, 0, 7);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(88, 45);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Title";
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Location = new Point(28, 80);
            labelVersion.Margin = new Padding(0, 0, 0, 21);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(81, 30);
            labelVersion.TabIndex = 1;
            labelVersion.Text = "Version";
            // 
            // labelDescription
            // 
            labelDescription.AutoSize = true;
            labelDescription.Location = new Point(28, 131);
            labelDescription.Margin = new Padding(0, 0, 0, 21);
            labelDescription.MaximumSize = new Size(735, 0);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(118, 30);
            labelDescription.TabIndex = 2;
            labelDescription.Text = "Description";
            // 
            // labelCopyright
            // 
            labelCopyright.AutoSize = true;
            labelCopyright.Location = new Point(28, 182);
            labelCopyright.Margin = new Padding(0, 0, 0, 7);
            labelCopyright.Name = "labelCopyright";
            labelCopyright.Size = new Size(307, 60);
            labelCopyright.TabIndex = 3;
            labelCopyright.Text = "Copyright © 2026 Keith O'Neill\r\nReleased under the MIT License";
            // 
            // linkLabelProject
            // 
            linkLabelProject.AutoSize = true;
            linkLabelProject.Location = new Point(28, 249);
            linkLabelProject.Margin = new Padding(0, 0, 0, 28);
            linkLabelProject.Name = "linkLabelProject";
            linkLabelProject.Size = new Size(115, 30);
            linkLabelProject.TabIndex = 4;
            linkLabelProject.TabStop = true;
            linkLabelProject.Text = "Project link";
            linkLabelProject.LinkClicked += linkLabelProject_LinkClicked;
            // 
            // buttonOk
            // 
            buttonOk.Anchor = AnchorStyles.Right;
            buttonOk.AutoSize = true;
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Location = new Point(609, 323);
            buttonOk.Margin = new Padding(0);
            buttonOk.MinimumSize = new Size(154, 52);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(154, 70);
            buttonOk.TabIndex = 1;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            // 
            // AboutForm
            // 
            AcceptButton = buttonOk;
            AutoScaleDimensions = new SizeF(168F, 168F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CancelButton = buttonOk;
            ClientSize = new Size(791, 438);
            Controls.Add(tableLayoutPanelMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(5, 5, 5, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About";
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
