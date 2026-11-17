namespace XMLDifferenceTool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TreeView treeViewRight;
        private System.Windows.Forms.TreeView treeViewLeft;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelFileSelection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonFileLeftSelect;
        private System.Windows.Forms.Button buttonFileRightSelect;
        private System.Windows.Forms.TextBox textBoxFileLeft;
        private System.Windows.Forms.TextBox textBoxFileRight;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tableLayoutPanelMain = new TableLayoutPanel();
            treeViewRight = new TreeView();
            treeViewLeft = new TreeView();
            tableLayoutPanelFileSelection = new TableLayoutPanel();
            tableLayoutPanelRight = new TableLayoutPanel();
            buttonFileRightSelect = new Button();
            textBoxFileRight = new TextBox();
            label2 = new Label();
            buttonCompare = new Button();
            tableLayoutPanelLeft = new TableLayoutPanel();
            buttonFileLeftSelect = new Button();
            textBoxFileLeft = new TextBox();
            label1 = new Label();
            menuStripMain = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            refreshToolStripMenuItem = new ToolStripMenuItem();
            displayToolStripMenuItem = new ToolStripMenuItem();
            sameItemsToolStripMenuItem = new ToolStripMenuItem();
            missingItemsToolStripMenuItem = new ToolStripMenuItem();
            differentItemsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            attributesToolStripMenuItem = new ToolStripMenuItem();
            elementsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            expandAllToolStripMenuItem = new ToolStripMenuItem();
            collapseAllToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemtoolStripMenuItemOptions = new ToolStripMenuItem();
            matchRepeatedElementsByToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemOrderInFile = new ToolStripMenuItem();
            toolStripMenuItemNameAndValue = new ToolStripMenuItem();
            namespaceHandlingToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemNamespaceStandard = new ToolStripMenuItem();
            toolStripMenuItemNamespaceLiteral = new ToolStripMenuItem();
            toolStripMenuItemNamespaceAutomatic = new ToolStripMenuItem();
            showLineNumbersToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutXMLDifferenceToolToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelNotice = new ToolStripStatusLabel();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanelFileSelection.SuspendLayout();
            tableLayoutPanelRight.SuspendLayout();
            tableLayoutPanelLeft.SuspendLayout();
            menuStripMain.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.ColumnCount = 2;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMain.Controls.Add(treeViewRight, 1, 1);
            tableLayoutPanelMain.Controls.Add(treeViewLeft, 0, 1);
            tableLayoutPanelMain.Controls.Add(tableLayoutPanelFileSelection, 0, 0);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(0, 38);
            tableLayoutPanelMain.Margin = new Padding(6, 7, 6, 7);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 2;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle());
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanelMain.Size = new Size(961, 365);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // treeViewRight
            // 
            treeViewRight.CausesValidation = false;
            treeViewRight.Dock = DockStyle.Fill;
            treeViewRight.Location = new Point(486, 70);
            treeViewRight.Margin = new Padding(6, 7, 6, 7);
            treeViewRight.Name = "treeViewRight";
            treeViewRight.Size = new Size(469, 288);
            treeViewRight.TabIndex = 9;
            treeViewRight.AfterCollapse += treeViewRight_AfterCollapse;
            treeViewRight.AfterExpand += treeViewRight_AfterExpand;
            treeViewRight.BeforeSelect += treeViewRight_BeforeSelect;
            treeViewRight.AfterSelect += treeViewRight_AfterSelect;
            // 
            // treeViewLeft
            // 
            treeViewLeft.Dock = DockStyle.Fill;
            treeViewLeft.Location = new Point(6, 70);
            treeViewLeft.Margin = new Padding(6, 7, 6, 7);
            treeViewLeft.Name = "treeViewLeft";
            treeViewLeft.Size = new Size(468, 288);
            treeViewLeft.TabIndex = 7;
            treeViewLeft.AfterCollapse += treeViewLeft_AfterCollapse;
            treeViewLeft.AfterExpand += treeViewLeft_AfterExpand;
            treeViewLeft.BeforeSelect += treeViewLeft_BeforeSelect;
            treeViewLeft.AfterSelect += treeViewLeft_AfterSelect;
            // 
            // tableLayoutPanelFileSelection
            // 
            tableLayoutPanelFileSelection.AutoSize = true;
            tableLayoutPanelFileSelection.ColumnCount = 2;
            tableLayoutPanelMain.SetColumnSpan(tableLayoutPanelFileSelection, 2);
            tableLayoutPanelFileSelection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelFileSelection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelFileSelection.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanelFileSelection.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanelFileSelection.Controls.Add(tableLayoutPanelRight, 1, 0);
            tableLayoutPanelFileSelection.Controls.Add(tableLayoutPanelLeft, 0, 0);
            tableLayoutPanelFileSelection.Dock = DockStyle.Fill;
            tableLayoutPanelFileSelection.Location = new Point(6, 7);
            tableLayoutPanelFileSelection.Margin = new Padding(6, 7, 6, 7);
            tableLayoutPanelFileSelection.Name = "tableLayoutPanelFileSelection";
            tableLayoutPanelFileSelection.RowCount = 1;
            tableLayoutPanelFileSelection.RowStyles.Add(new RowStyle());
            tableLayoutPanelFileSelection.Size = new Size(949, 49);
            tableLayoutPanelFileSelection.TabIndex = 11;
            // 
            // tableLayoutPanelRight
            // 
            tableLayoutPanelRight.AutoSize = true;
            tableLayoutPanelRight.ColumnCount = 4;
            tableLayoutPanelRight.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelRight.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelRight.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelRight.Controls.Add(buttonFileRightSelect, 2, 0);
            tableLayoutPanelRight.Controls.Add(textBoxFileRight, 1, 0);
            tableLayoutPanelRight.Controls.Add(label2, 0, 0);
            tableLayoutPanelRight.Controls.Add(buttonCompare, 3, 0);
            tableLayoutPanelRight.Dock = DockStyle.Fill;
            tableLayoutPanelRight.Location = new Point(474, 0);
            tableLayoutPanelRight.Margin = new Padding(0);
            tableLayoutPanelRight.Name = "tableLayoutPanelRight";
            tableLayoutPanelRight.RowCount = 1;
            tableLayoutPanelRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelRight.Size = new Size(475, 49);
            tableLayoutPanelRight.TabIndex = 13;
            // 
            // buttonFileRightSelect
            // 
            buttonFileRightSelect.Anchor = AnchorStyles.Left;
            buttonFileRightSelect.Location = new Point(321, 7);
            buttonFileRightSelect.Margin = new Padding(6, 7, 6, 7);
            buttonFileRightSelect.Name = "buttonFileRightSelect";
            buttonFileRightSelect.Size = new Size(35, 35);
            buttonFileRightSelect.TabIndex = 9;
            buttonFileRightSelect.Text = "...";
            buttonFileRightSelect.UseVisualStyleBackColor = true;
            buttonFileRightSelect.Click += buttonFileRightSelect_Click;
            // 
            // textBoxFileRight
            // 
            textBoxFileRight.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxFileRight.Location = new Point(134, 7);
            textBoxFileRight.Margin = new Padding(6, 0, 6, 0);
            textBoxFileRight.Name = "textBoxFileRight";
            textBoxFileRight.Size = new Size(175, 35);
            textBoxFileRight.TabIndex = 11;
            textBoxFileRight.Text = "None";
            textBoxFileRight.TextChanged += textBox_TextChanged;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ImeMode = ImeMode.NoControl;
            label2.Location = new Point(6, 10);
            label2.Margin = new Padding(6, 9, 0, 12);
            label2.Name = "label2";
            label2.Size = new Size(122, 25);
            label2.TabIndex = 7;
            label2.Text = "Second File:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // buttonCompare
            // 
            buttonCompare.Anchor = AnchorStyles.Left;
            buttonCompare.AutoSize = true;
            buttonCompare.Location = new Point(365, 4);
            buttonCompare.Name = "buttonCompare";
            buttonCompare.Size = new Size(107, 40);
            buttonCompare.TabIndex = 12;
            buttonCompare.Text = "Compare";
            buttonCompare.UseVisualStyleBackColor = true;
            buttonCompare.Click += buttonCompare_Click;
            // 
            // tableLayoutPanelLeft
            // 
            tableLayoutPanelLeft.AutoSize = true;
            tableLayoutPanelLeft.ColumnCount = 3;
            tableLayoutPanelLeft.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLeft.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelLeft.Controls.Add(buttonFileLeftSelect, 2, 0);
            tableLayoutPanelLeft.Controls.Add(textBoxFileLeft, 1, 0);
            tableLayoutPanelLeft.Controls.Add(label1, 0, 0);
            tableLayoutPanelLeft.Dock = DockStyle.Fill;
            tableLayoutPanelLeft.Location = new Point(0, 0);
            tableLayoutPanelLeft.Margin = new Padding(0);
            tableLayoutPanelLeft.Name = "tableLayoutPanelLeft";
            tableLayoutPanelLeft.RowCount = 1;
            tableLayoutPanelLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelLeft.Size = new Size(474, 49);
            tableLayoutPanelLeft.TabIndex = 14;
            // 
            // buttonFileLeftSelect
            // 
            buttonFileLeftSelect.Anchor = AnchorStyles.Left;
            buttonFileLeftSelect.Location = new Point(433, 7);
            buttonFileLeftSelect.Margin = new Padding(6, 7, 6, 7);
            buttonFileLeftSelect.Name = "buttonFileLeftSelect";
            buttonFileLeftSelect.Size = new Size(35, 35);
            buttonFileLeftSelect.TabIndex = 8;
            buttonFileLeftSelect.Text = "...";
            buttonFileLeftSelect.UseVisualStyleBackColor = true;
            buttonFileLeftSelect.Click += buttonFileLeftSelect_Click;
            // 
            // textBoxFileLeft
            // 
            textBoxFileLeft.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxFileLeft.Location = new Point(103, 7);
            textBoxFileLeft.Margin = new Padding(6, 0, 6, 0);
            textBoxFileLeft.Name = "textBoxFileLeft";
            textBoxFileLeft.Size = new Size(318, 35);
            textBoxFileLeft.TabIndex = 10;
            textBoxFileLeft.Text = "None";
            textBoxFileLeft.TextChanged += textBox_TextChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ImeMode = ImeMode.NoControl;
            label1.Location = new Point(6, 10);
            label1.Margin = new Padding(6, 9, 0, 12);
            label1.Name = "label1";
            label1.Size = new Size(91, 25);
            label1.TabIndex = 6;
            label1.Text = "First File:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // menuStripMain
            // 
            menuStripMain.ImageScalingSize = new Size(28, 28);
            menuStripMain.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, displayToolStripMenuItem, toolStripMenuItemtoolStripMenuItemOptions, helpToolStripMenuItem });
            menuStripMain.Location = new Point(0, 0);
            menuStripMain.Name = "menuStripMain";
            menuStripMain.Size = new Size(961, 38);
            menuStripMain.TabIndex = 1;
            menuStripMain.Text = "menuStripMain";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { refreshToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(62, 34);
            fileToolStripMenuItem.Text = "&File";
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new Size(200, 40);
            refreshToolStripMenuItem.Text = "Refresh";
            // 
            // displayToolStripMenuItem
            // 
            displayToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { sameItemsToolStripMenuItem, missingItemsToolStripMenuItem, differentItemsToolStripMenuItem, toolStripSeparator1, attributesToolStripMenuItem, elementsToolStripMenuItem, toolStripSeparator2, expandAllToolStripMenuItem, collapseAllToolStripMenuItem });
            displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            displayToolStripMenuItem.Size = new Size(98, 34);
            displayToolStripMenuItem.Text = "&Display";
            // 
            // sameItemsToolStripMenuItem
            // 
            sameItemsToolStripMenuItem.Checked = true;
            sameItemsToolStripMenuItem.CheckOnClick = true;
            sameItemsToolStripMenuItem.CheckState = CheckState.Checked;
            sameItemsToolStripMenuItem.Name = "sameItemsToolStripMenuItem";
            sameItemsToolStripMenuItem.Size = new Size(270, 40);
            sameItemsToolStripMenuItem.Text = "Same Items";
            sameItemsToolStripMenuItem.CheckedChanged += sameItemsToolStripMenuItem_CheckedChanged;
            // 
            // missingItemsToolStripMenuItem
            // 
            missingItemsToolStripMenuItem.Checked = true;
            missingItemsToolStripMenuItem.CheckOnClick = true;
            missingItemsToolStripMenuItem.CheckState = CheckState.Checked;
            missingItemsToolStripMenuItem.Name = "missingItemsToolStripMenuItem";
            missingItemsToolStripMenuItem.Size = new Size(270, 40);
            missingItemsToolStripMenuItem.Text = "Missing Items";
            missingItemsToolStripMenuItem.CheckedChanged += missingItemsToolStripMenuItem_CheckedChanged;
            // 
            // differentItemsToolStripMenuItem
            // 
            differentItemsToolStripMenuItem.Checked = true;
            differentItemsToolStripMenuItem.CheckOnClick = true;
            differentItemsToolStripMenuItem.CheckState = CheckState.Checked;
            differentItemsToolStripMenuItem.Name = "differentItemsToolStripMenuItem";
            differentItemsToolStripMenuItem.Size = new Size(270, 40);
            differentItemsToolStripMenuItem.Text = "Different Items";
            differentItemsToolStripMenuItem.CheckedChanged += differentItemsToolStripMenuItem_CheckedChanged;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(267, 6);
            // 
            // attributesToolStripMenuItem
            // 
            attributesToolStripMenuItem.Checked = true;
            attributesToolStripMenuItem.CheckOnClick = true;
            attributesToolStripMenuItem.CheckState = CheckState.Checked;
            attributesToolStripMenuItem.Name = "attributesToolStripMenuItem";
            attributesToolStripMenuItem.Size = new Size(270, 40);
            attributesToolStripMenuItem.Text = "Attributes (a)";
            attributesToolStripMenuItem.CheckedChanged += attributesToolStripMenuItem_CheckedChanged;
            // 
            // elementsToolStripMenuItem
            // 
            elementsToolStripMenuItem.Checked = true;
            elementsToolStripMenuItem.CheckOnClick = true;
            elementsToolStripMenuItem.CheckState = CheckState.Checked;
            elementsToolStripMenuItem.Name = "elementsToolStripMenuItem";
            elementsToolStripMenuItem.Size = new Size(270, 40);
            elementsToolStripMenuItem.Text = "Elements (e)";
            elementsToolStripMenuItem.CheckedChanged += elementsToolStripMenuItem_CheckedChanged;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(267, 6);
            // 
            // expandAllToolStripMenuItem
            // 
            expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            expandAllToolStripMenuItem.Size = new Size(270, 40);
            expandAllToolStripMenuItem.Text = "&Expand All";
            expandAllToolStripMenuItem.Click += expandAllToolStripMenuItem_Click;
            // 
            // collapseAllToolStripMenuItem
            // 
            collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            collapseAllToolStripMenuItem.Size = new Size(270, 40);
            collapseAllToolStripMenuItem.Text = "&Collapse All";
            collapseAllToolStripMenuItem.Click += collapseAllToolStripMenuItem_Click;
            // 
            // toolStripMenuItemtoolStripMenuItemOptions
            // 
            toolStripMenuItemtoolStripMenuItemOptions.DropDownItems.AddRange(new ToolStripItem[] { matchRepeatedElementsByToolStripMenuItem, namespaceHandlingToolStripMenuItem, showLineNumbersToolStripMenuItem });
            toolStripMenuItemtoolStripMenuItemOptions.Name = "toolStripMenuItemtoolStripMenuItemOptions";
            toolStripMenuItemtoolStripMenuItemOptions.Size = new Size(104, 34);
            toolStripMenuItemtoolStripMenuItemOptions.Text = "&Options";
            // 
            // matchRepeatedElementsByToolStripMenuItem
            // 
            matchRepeatedElementsByToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemOrderInFile, toolStripMenuItemNameAndValue });
            matchRepeatedElementsByToolStripMenuItem.Name = "matchRepeatedElementsByToolStripMenuItem";
            matchRepeatedElementsByToolStripMenuItem.Size = new Size(396, 40);
            matchRepeatedElementsByToolStripMenuItem.Text = "Match repeated elements by";
            // 
            // toolStripMenuItemOrderInFile
            // 
            toolStripMenuItemOrderInFile.Checked = true;
            toolStripMenuItemOrderInFile.CheckOnClick = true;
            toolStripMenuItemOrderInFile.CheckState = CheckState.Checked;
            toolStripMenuItemOrderInFile.Name = "toolStripMenuItemOrderInFile";
            toolStripMenuItemOrderInFile.Size = new Size(284, 40);
            toolStripMenuItemOrderInFile.Text = "Order in file";
            toolStripMenuItemOrderInFile.CheckedChanged += toolStripMenuItemOrderInFile_CheckedChanged;
            // 
            // toolStripMenuItemNameAndValue
            // 
            toolStripMenuItemNameAndValue.CheckOnClick = true;
            toolStripMenuItemNameAndValue.Name = "toolStripMenuItemNameAndValue";
            toolStripMenuItemNameAndValue.Size = new Size(284, 40);
            toolStripMenuItemNameAndValue.Text = "Name and Value";
            toolStripMenuItemNameAndValue.CheckedChanged += toolStripMenuItemNameAndValue_CheckedChanged;
            // 
            // namespaceHandlingToolStripMenuItem
            // 
            namespaceHandlingToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemNamespaceStandard, toolStripMenuItemNamespaceLiteral, toolStripMenuItemNamespaceAutomatic });
            namespaceHandlingToolStripMenuItem.Name = "namespaceHandlingToolStripMenuItem";
            namespaceHandlingToolStripMenuItem.Size = new Size(396, 40);
            namespaceHandlingToolStripMenuItem.Text = "Namespace handling";
            // 
            // toolStripMenuItemNamespaceStandard
            // 
            toolStripMenuItemNamespaceStandard.Name = "toolStripMenuItemNamespaceStandard";
            toolStripMenuItemNamespaceStandard.Size = new Size(265, 40);
            toolStripMenuItemNamespaceStandard.Text = "Standard";
            toolStripMenuItemNamespaceStandard.ToolTipText = "Normal XML namespace rules. A file that uses an undeclared prefix is rejected.";
            toolStripMenuItemNamespaceStandard.Click += namespaceHandlingMenuItem_Click;
            // 
            // toolStripMenuItemNamespaceLiteral
            // 
            toolStripMenuItemNamespaceLiteral.Name = "toolStripMenuItemNamespaceLiteral";
            toolStripMenuItemNamespaceLiteral.Size = new Size(265, 40);
            toolStripMenuItemNamespaceLiteral.Text = "Literal prefixes";
            toolStripMenuItemNamespaceLiteral.ToolTipText = "Treat prefixes such as xlink: as part of the name. Names are compared exactly as written.";
            toolStripMenuItemNamespaceLiteral.Click += namespaceHandlingMenuItem_Click;
            // 
            // toolStripMenuItemNamespaceAutomatic
            // 
            toolStripMenuItemNamespaceAutomatic.Checked = true;
            toolStripMenuItemNamespaceAutomatic.CheckState = CheckState.Checked;
            toolStripMenuItemNamespaceAutomatic.Name = "toolStripMenuItemNamespaceAutomatic";
            toolStripMenuItemNamespaceAutomatic.Size = new Size(265, 40);
            toolStripMenuItemNamespaceAutomatic.Text = "Automatic";
            toolStripMenuItemNamespaceAutomatic.ToolTipText = "Standard rules, switching to literal prefixes only for a file that has a namespace problem.";
            toolStripMenuItemNamespaceAutomatic.Click += namespaceHandlingMenuItem_Click;
            // 
            // showLineNumbersToolStripMenuItem
            // 
            showLineNumbersToolStripMenuItem.Checked = true;
            showLineNumbersToolStripMenuItem.CheckOnClick = true;
            showLineNumbersToolStripMenuItem.CheckState = CheckState.Checked;
            showLineNumbersToolStripMenuItem.Name = "showLineNumbersToolStripMenuItem";
            showLineNumbersToolStripMenuItem.Size = new Size(396, 40);
            showLineNumbersToolStripMenuItem.Text = "&Show Line Numbers";
            showLineNumbersToolStripMenuItem.CheckedChanged += showLineNumbersToolStripMenuItem_CheckedChanged;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutXMLDifferenceToolToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(74, 34);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutXMLDifferenceToolToolStripMenuItem
            // 
            aboutXMLDifferenceToolToolStripMenuItem.Name = "aboutXMLDifferenceToolToolStripMenuItem";
            aboutXMLDifferenceToolToolStripMenuItem.Size = new Size(188, 40);
            aboutXMLDifferenceToolToolStripMenuItem.Text = "&About";
            aboutXMLDifferenceToolToolStripMenuItem.Click += aboutXMLDifferenceToolToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(28, 28);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelNotice });
            statusStrip1.Location = new Point(0, 403);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(961, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelNotice
            // 
            toolStripStatusLabelNotice.Name = "toolStripStatusLabelNotice";
            toolStripStatusLabelNotice.Size = new Size(946, 13);
            toolStripStatusLabelNotice.Spring = true;
            toolStripStatusLabelNotice.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(168F, 168F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            AutoScrollMinSize = new Size(640, 400);
            ClientSize = new Size(961, 425);
            Controls.Add(tableLayoutPanelMain);
            Controls.Add(menuStripMain);
            Controls.Add(statusStrip1);
            MainMenuStrip = menuStripMain;
            Margin = new Padding(6, 7, 6, 7);
            Name = "MainForm";
            Text = "XML Difference Tool";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelMain.PerformLayout();
            tableLayoutPanelFileSelection.ResumeLayout(false);
            tableLayoutPanelFileSelection.PerformLayout();
            tableLayoutPanelRight.ResumeLayout(false);
            tableLayoutPanelRight.PerformLayout();
            tableLayoutPanelLeft.ResumeLayout(false);
            tableLayoutPanelLeft.PerformLayout();
            menuStripMain.ResumeLayout(false);
            menuStripMain.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private Button buttonCompare;
        private MenuStrip menuStripMain;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem displayToolStripMenuItem;
        private ToolStripMenuItem sameItemsToolStripMenuItem;
        private ToolStripMenuItem missingItemsToolStripMenuItem;
        private ToolStripMenuItem differentItemsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem attributesToolStripMenuItem;
        private ToolStripMenuItem elementsToolStripMenuItem;
        private ToolStripMenuItem matchRepeatedElementsByToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemOrderInFile;
        private ToolStripMenuItem toolStripMenuItemNameAndValue;
        private ToolStripMenuItem toolStripMenuItemtoolStripMenuItemOptions;
        private ToolStripMenuItem showLineNumbersToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutXMLDifferenceToolToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanelRight;
        private TableLayoutPanel tableLayoutPanelLeft;
        private ToolStripMenuItem namespaceHandlingToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemNamespaceStandard;
        private ToolStripMenuItem toolStripMenuItemNamespaceLiteral;
        private ToolStripMenuItem toolStripMenuItemNamespaceAutomatic;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelNotice;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem expandAllToolStripMenuItem;
        private ToolStripMenuItem collapseAllToolStripMenuItem;
    }
}
