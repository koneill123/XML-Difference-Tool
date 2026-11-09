using Microsoft.VisualBasic.ApplicationServices;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Xml.Linq;
using XMLDifferenceTool.Core;
using System.Runtime.CompilerServices;


namespace XMLDifferenceTool
{
    public partial class MainForm : Form
    {
#if DEBUG
        // sourceFile is filled in by the compiler with the full path of the .cs file that calls this method
        // (here MainForm.cs, i.e. <repo>\src\XMLDifferenceTool\MainForm.cs). Two folders up is the repo root.
        private static string SamplePath(string fileName, [CallerFilePath] string sourceFile = "")
        {
            string sourceDir = Path.GetDirectoryName(sourceFile)!;
            return Path.GetFullPath(Path.Combine(sourceDir, "..", "..", "Samples", fileName));
        }
#endif

        private DiffNode leftDiffNode = new DiffNode("None", NodeKind.Element, MatchRepeateElementsBy.OrderInFile, -1);
        private DiffNode rightDiffNode = new DiffNode("None", NodeKind.Element, MatchRepeateElementsBy.OrderInFile, -1);
        private bool AutomatedUiInteraction = false;

        private Color previousRightSelectionBgColor = Color.Magenta;
        private Color previousLeftSelectionBgColor = Color.Magenta;

        private static Color DifferentColor = Color.LightYellow;
        private static Color MissingColor = Color.LightCoral;
        private Color SelectedColor = Color.LightGray;

        private static readonly Color ErrorBackColor = Color.FromArgb(255, 192, 192);

        private const string ErrorFormat = "Error opening file:{0}{1}{2}";

        private SyncedTreeView syncedTreeViewRight = new SyncedTreeView();
        private SyncedTreeView syncedTreeViewLeft = new SyncedTreeView();

        private MatchRepeateElementsBy matchRules = MatchRepeateElementsBy.OrderInFile;

        private NamespaceHandling namespaceHandling = NamespaceHandling.Automatic;

        // The progress dialog only appears if loading/comparing takes longer than this.
        private static readonly TimeSpan ProgressDialogDelay = TimeSpan.FromMilliseconds(400);

        private bool isComparing = false;           // a load/compare run is in progress
        private bool compareRequested = false;      // another run was requested while one was in progress
        private bool viewRefreshRequested = false;  // a filter changed while a run was in progress

        private readonly ISettingsStore settingsStore = new FileSettingsStore();
        private AppSettings settings = new();

        private bool LoadingSettings = true; // guard to prevent event handlers from firing while settings are being loaded

        private async void MainForm_Load(object sender, EventArgs e)
        {
            settings = await settingsStore.LoadAsync();
            ApplySettings(settings);          // wrapped in your "updating" guard
            this.LoadingSettings = false;
        }

        public MainForm()
        {
            InitializeComponent();

#if DEBUG
            // Debugging convenience: preselect the sample files from wwwroot/sample-data; press Compare to compare them
            this.textBoxFileLeft.Text = SamplePath("sample_left.xml");
            this.textBoxFileRight.Text = SamplePath("sample_right.xml");
#endif


            this.treeViewLeft.Dispose();
            this.treeViewLeft = null;
            this.treeViewRight.Dispose();
            this.treeViewRight = null;

            this.tableLayoutPanelMain.Controls.Add(this.syncedTreeViewRight, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.syncedTreeViewLeft, 0, 1);

            // syncedTreeViewRight
            this.syncedTreeViewRight.CausesValidation = false;
            this.syncedTreeViewRight.Dock = DockStyle.Fill;
            this.syncedTreeViewRight.TabIndex = 9;
            this.syncedTreeViewRight.Name = "treeViewRight";
            this.syncedTreeViewRight.AfterCollapse += new TreeViewEventHandler(this.treeViewRight_AfterCollapse);
            this.syncedTreeViewRight.AfterExpand += new TreeViewEventHandler(this.treeViewRight_AfterExpand);
            this.syncedTreeViewRight.BeforeSelect += new TreeViewCancelEventHandler(this.treeViewRight_BeforeSelect);
            this.syncedTreeViewRight.AfterSelect += new TreeViewEventHandler(this.treeViewRight_AfterSelect);
            this.syncedTreeViewRight.Partner = this.syncedTreeViewLeft;

            // syncedTreeViewLeft
            this.syncedTreeViewLeft.Dock = DockStyle.Fill;
            this.syncedTreeViewLeft.TabIndex = 7;
            this.syncedTreeViewLeft.Name = "treeViewLeft";
            this.syncedTreeViewLeft.AfterCollapse += new TreeViewEventHandler(this.treeViewLeft_AfterCollapse);
            this.syncedTreeViewLeft.AfterExpand += new TreeViewEventHandler(this.treeViewLeft_AfterExpand);
            this.syncedTreeViewLeft.BeforeSelect += new TreeViewCancelEventHandler(this.treeViewLeft_BeforeSelect);
            this.syncedTreeViewLeft.AfterSelect += new TreeViewEventHandler(this.treeViewLeft_AfterSelect);
            this.syncedTreeViewLeft.Partner = this.syncedTreeViewRight;

            LoadColorPallet();
            UpdateNamespaceMenuChecks();

            UpdateUI();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CollectSettings();                                        // controls -> settings
            Task.Run(() => settingsStore.SaveAsync(settings)).GetAwaiter().GetResult();
        }

        private void ApplySettings(AppSettings s)
        {
            // Match mode: checking the chosen box lets your existing handlers sync the
            // other box and the menu items. OrderInFile is already the default.
            this.matchRules = s.MatchMode;
            if (s.MatchMode == MatchRepeateElementsBy.NameAndValue)
                this.toolStripMenuItemNameAndValue.Checked = true;

            this.namespaceHandling = s.NamespaceMode;
            UpdateNamespaceMenuChecks();

            this.showLineNumbersToolStripMenuItem.Checked = s.ShowLineNumbers;
            this.sameItemsToolStripMenuItem.Checked = s.ShowSame;
            this.differentItemsToolStripMenuItem.Checked = s.ShowDifferent;
            this.missingItemsToolStripMenuItem.Checked = s.ShowMissing;
            this.attributesToolStripMenuItem.Checked = s.ShowAttributes;
            this.elementsToolStripMenuItem.Checked = s.ShowElements;

            // Window position and size, only if it still lands on a connected screen
            if (s.Window is { Width: > 0, Height: > 0 } w)
            {
                var bounds = new Rectangle(w.X, w.Y, w.Width, w.Height);
                var titleStrip = new Rectangle(bounds.X, bounds.Y, bounds.Width, 40);
                if (Screen.AllScreens.Any(sc => sc.WorkingArea.IntersectsWith(titleStrip)))
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Bounds = bounds;
                }
                if (w.Maximized)
                    this.WindowState = FormWindowState.Maximized;
            }
        }

        private void CollectSettings()
        {
            settings.MatchMode = this.matchRules;
            settings.NamespaceMode = this.namespaceHandling;
            settings.ShowSame = this.sameItemsToolStripMenuItem.Checked;
            settings.ShowDifferent = this.differentItemsToolStripMenuItem.Checked;
            settings.ShowMissing = this.missingItemsToolStripMenuItem.Checked;
            settings.ShowAttributes = this.attributesToolStripMenuItem.Checked;
            settings.ShowElements = this.elementsToolStripMenuItem.Checked;
            settings.ShowLineNumbers = this.showLineNumbersToolStripMenuItem.Checked;

            // RestoreBounds holds the normal-size rectangle while maximized or minimized
            Rectangle b = this.WindowState == FormWindowState.Normal ? this.Bounds : this.RestoreBounds;
            settings.Window = new WindowSettings
            {
                X = b.X,
                Y = b.Y,
                Width = b.Width,
                Height = b.Height,
                Maximized = this.WindowState == FormWindowState.Maximized
            };
        }


        private void LoadColorPallet()
        {
            this.differentItemsToolStripMenuItem.BackColor = DifferentColor;
            this.missingItemsToolStripMenuItem.BackColor = MissingColor;

            this.differentItemsToolStripMenuItem.BackColor = DifferentColor;
            this.missingItemsToolStripMenuItem.BackColor = MissingColor;

            // 35% highlight over the tree's background
            this.SelectedColor = Blend(SystemColors.Highlight, SystemColors.Window, 0.35);
        }

        static Color Blend(Color fore, Color back, double amount)
        {
            // amount: 0.0 = all back, 1.0 = all fore
            int r = (int)(fore.R * amount + back.R * (1 - amount));
            int g = (int)(fore.G * amount + back.G * (1 - amount));
            int b = (int)(fore.B * amount + back.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        private DiffKindState SelectedElementDiffKindState
        {
            get
            {
                return ((this.sameItemsToolStripMenuItem.Checked ? DiffKindState.ElementSame : DiffKindState.None)
                    | (this.differentItemsToolStripMenuItem.Checked ? DiffKindState.ElementDifferent : DiffKindState.None)
                    | (this.missingItemsToolStripMenuItem.Checked ? DiffKindState.ElementMissing : DiffKindState.None));
            }
        }

        private DiffKindState SelectedAttributeDiffKindState
        {
            get
            {
                return ((this.sameItemsToolStripMenuItem.Checked ? DiffKindState.AttributeSame : DiffKindState.None)
                    | (this.differentItemsToolStripMenuItem.Checked ? DiffKindState.AttributeDifferent : DiffKindState.None)
                    | (this.missingItemsToolStripMenuItem.Checked ? DiffKindState.AttributeMissing : DiffKindState.None));
            }
        }

        private static bool HasDirectText(XElement xelement)
        {
            return xelement.Nodes().OfType<XText>().Any(x => !string.IsNullOrWhiteSpace(x.Value));
        }

        private static string FullUniqueTag(TreeNode treeNode)
        {
            string retVal = "<None>";
            if (treeNode != null)
            {
                retVal = treeNode.Tag as string;
                TreeNode parentNode = treeNode.Parent;
                while (parentNode != null)
                {
                    retVal = (string)parentNode.Tag + Helpers.PathSeparator + retVal;
                    parentNode = parentNode.Parent;
                }
            }
            return retVal;
        }

        //***************************************************** UI Portion *****************************************************

        TreeNode FindNodeByTag(TreeNodeCollection nodes, string tag)
        {
            foreach (TreeNode node in nodes)
            {
                if (FullUniqueTag(node) == tag)
                {
                    return node;
                }

                TreeNode found = FindNodeByTag(node.Nodes, tag);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        private void UpdateUI()
        {
            this.buttonCompare.Enabled = File.Exists(this.textBoxFileLeft.Text) && File.Exists(this.textBoxFileRight.Text);
            this.textBoxFileLeft.BackColor = File.Exists(this.textBoxFileLeft.Text) ? SystemColors.Window : ErrorBackColor;
            this.textBoxFileRight.BackColor = File.Exists(this.textBoxFileRight.Text) ? SystemColors.Window : ErrorBackColor;
        }

        /// <summary>
        /// Loads both files, diffs them and refreshes the tree views. The work runs in the background and a modal
        /// progress dialog is shown only if it takes longer than <see cref="ProgressDialogDelay"/>.
        /// </summary>
        public async Task LoadAndCompareFilesAsync()
        {
            UpdateUI();

            if (!File.Exists(this.textBoxFileLeft.Text) || !File.Exists(this.textBoxFileRight.Text))
            {
                return;
            }

            if (this.isComparing)
            {
                // A run is already in progress; run once more when it finishes so the latest settings are used.
                this.compareRequested = true;
                return;
            }

            this.isComparing = true;
            try
            {
                do
                {
                    this.compareRequested = false;
                    await RunCompareAsync();
                }
                while (this.compareRequested);
            }
            finally
            {
                this.isComparing = false;
            }

            if (this.viewRefreshRequested)
            {
                this.viewRefreshRequested = false;
                UpdateActiveNodes();
            }
        }

        private sealed record CompareOutcome(DiffResult Result, string FailedFile, string ErrorMessage, string Notice);

        private async Task RunCompareAsync()
        {
            string leftPath = this.textBoxFileLeft.Text;
            string rightPath = this.textBoxFileRight.Text;
            MatchRepeateElementsBy rules = this.matchRules;
            NamespaceHandling namespaces = this.namespaceHandling;
            CompareOutcome outcome = null;

            this.AutomatedUiInteraction = true;
            this.tableLayoutPanelMain.Enabled = false;

            try
            {
                await ProgressForm.RunAsync(this, ProgressDialogDelay, async progress =>
                {
                    outcome = await CompareFilesAsync(leftPath, rightPath, rules, namespaces, progress);
                    ApplyOutcome(outcome, rules);
                    await ShowResultsAsync(progress);
                });
            }
            finally
            {
                this.AutomatedUiInteraction = false;
                this.tableLayoutPanelMain.Enabled = true;
                UpdateUI();
            }

            // Shown after the progress dialog has closed so the message box is not competing with it.
            if (outcome != null && outcome.ErrorMessage != null)
            {
                string message = outcome.FailedFile != null
                    ? string.Format(ErrorFormat, Path.GetFileName(outcome.FailedFile), Environment.NewLine, outcome.ErrorMessage)
                    : "Error comparing files:" + Environment.NewLine + outcome.ErrorMessage;

                MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Loads and diffs the two files off the UI thread. Never shows UI; failures are returned in the outcome.
        /// </summary>
        private static async Task<CompareOutcome> CompareFilesAsync(string leftPath, string rightPath, MatchRepeateElementsBy rules, NamespaceHandling namespaces, IProgress<DiffProgress> progress)
        {
            byte[] leftBytes;
            byte[] rightBytes;
            LoadedXml leftXml;
            LoadedXml rightXml;

            progress.Report(new DiffProgress("Loading " + Path.GetFileName(leftPath) + "..."));
            try
            {
                (leftBytes, leftXml) = await Task.Run(() => LoadFile(leftPath, namespaces));
            }
            catch (Exception ex)
            {
                return new CompareOutcome(null, leftPath, ex.Message, null);
            }

            progress.Report(new DiffProgress("Loading " + Path.GetFileName(rightPath) + "..."));
            try
            {
                (rightBytes, rightXml) = await Task.Run(() => LoadFile(rightPath, namespaces));
            }
            catch (Exception ex)
            {
                return new CompareOutcome(null, rightPath, ex.Message, null);
            }

            string notice = null;
            try
            {
                // In Automatic mode, if only one file needed literal prefixes, the other is loaded the same way
                // so that both files use the same names.
                LoadedXml firstLoaded = leftXml;
                LoadedXml secondLoaded = rightXml;
                (leftXml, rightXml) = XmlLoader.Reconcile(firstLoaded, leftBytes, secondLoaded, rightBytes, namespaces);

                notice = XmlLoader.DescribeNotices(Path.GetFileName(leftPath), leftXml, Path.GetFileName(rightPath), rightXml, "  |  ");

                DiffResult result = await Task.Run(() => XmlDiffService.CompareAsync(leftXml.Document, rightXml.Document, rules, progress));
                return new CompareOutcome(result, null, null, notice);
            }
            catch (Exception ex)
            {
                return new CompareOutcome(null, null, ex.Message, notice);
            }
        }

        private static (byte[] Bytes, LoadedXml Xml) LoadFile(string path, NamespaceHandling namespaces)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return (bytes, XmlLoader.Load(bytes, namespaces));
        }

        private void ApplyOutcome(CompareOutcome outcome, MatchRepeateElementsBy rules)
        {
            // Tell the user when a file was loaded with literal prefixes
            this.toolStripStatusLabelNotice.Text = outcome.Notice ?? string.Empty;
            this.toolStripStatusLabelNotice.ToolTipText = outcome.Notice ?? string.Empty;

            if (outcome.Result != null)
            {
                this.leftDiffNode = outcome.Result.Left;
                this.rightDiffNode = outcome.Result.Right;
            }
            else
            {
                // Same fallback as before: show empty "None" trees when a file could not be loaded.
                this.leftDiffNode = new DiffNode("None", NodeKind.Element, rules, -1);
                this.rightDiffNode = new DiffNode("None", NodeKind.Element, rules, -1);
                XmlDiffer.DiffNodeDataCombined(this.leftDiffNode, this.rightDiffNode);
            }
        }

        /// <summary>
        /// The kinds of nodes currently selected for display by the Display menu / checkboxes.
        /// </summary>
        private DiffKindState GetDisplayDiffKindStates()
        {
            return (this.elementsToolStripMenuItem.Checked ? this.SelectedElementDiffKindState : DiffKindState.None)
                 | (this.attributesToolStripMenuItem.Checked ? this.SelectedAttributeDiffKindState : DiffKindState.None);
        }

        /// <summary>
        /// Builds the two TreeNode trees on the UI thread, yielding regularly so the progress dialog stays
        /// responsive, then fills both tree views.
        /// </summary>
        private async Task ShowResultsAsync(IProgress<DiffProgress> progress)
        {
            DiffKindState displayTypes = GetDisplayDiffKindStates();

            TreeBuildContext context = new TreeBuildContext(progress, CountNodes(this.leftDiffNode) + CountNodes(this.rightDiffNode));
            progress.Report(new DiffProgress("Building the tree views...", 0));

            TreeNode leftNode = await DiffNodeToTreeNode(this.leftDiffNode, displayTypes, context);
            TreeNode rightNode = await DiffNodeToTreeNode(this.rightDiffNode, displayTypes, context);

            if (leftNode == null || rightNode == null)
            {
                leftNode = new TreeNode("None");
                rightNode = new TreeNode("None");
            }

            // Adding the finished trees to the controls is one blocking call, so let the message paint first.
            progress.Report(new DiffProgress("Displaying the results..."));
            await Task.Delay(20);

            this.syncedTreeViewLeft.BeginUpdate();
            this.syncedTreeViewRight.BeginUpdate();

            this.syncedTreeViewRight.Nodes.Clear();
            this.syncedTreeViewLeft.Nodes.Clear();

            this.syncedTreeViewLeft.Nodes.Add(leftNode);
            this.syncedTreeViewRight.Nodes.Add(rightNode);

            this.syncedTreeViewLeft.EndUpdate();
            this.syncedTreeViewRight.EndUpdate();
        }

        private static int CountNodes(DiffNode node)
        {
            int count = 1;
            foreach (DiffNode child in node.Children)
            {
                count += CountNodes(child);
            }
            return count;
        }

        /// <summary>
        /// Tracks how many nodes have been processed and decides when to report progress and yield to the UI.
        /// </summary>
        private sealed class TreeBuildContext
        {
            private readonly IProgress<DiffProgress> progress;
            private readonly int total;
            private readonly Stopwatch sinceLastYield = Stopwatch.StartNew();
            private int done;

            public TreeBuildContext(IProgress<DiffProgress> progress, int total)
            {
                this.progress = progress;
                this.total = Math.Max(1, total);
            }

            public async Task StepAsync()
            {
                this.done++;

                if (this.sinceLastYield.ElapsedMilliseconds < 50)
                {
                    return;
                }

                this.progress.Report(new DiffProgress("Building the tree views...", Math.Min(1.0, (double)this.done / this.total)));

                // Task.Delay (rather than Task.Yield) lets the message loop go idle so the dialog can repaint.
                await Task.Delay(1);
                this.sinceLastYield.Restart();
            }
        }

        /// <summary>
        /// Async version of <see cref="DiffNodeToTreeNode"/> used while the progress dialog may be showing.
        /// The logic is the same; it additionally yields to the UI periodically.
        /// </summary>
        private async Task<TreeNode> DiffNodeToTreeNode(DiffNode node, DiffKindState displayDiffKindStates, TreeBuildContext context = null)
        {
            TreeNode retVal = null;

            if (context != null)
            {
                await context.StepAsync();
            }

            if (((node.ChangeType & displayDiffKindStates) > 0) || (node.ChildrenChangeType & displayDiffKindStates) > 0)
            {
                retVal = new TreeNode(node.ToDisplayText(this.showLineNumbersToolStripMenuItem.Checked));
                retVal.Tag = node.Kind + @"\" + node.Tag;

                if ((node.ChildrenChangeType & displayDiffKindStates) > 0)
                {
                    var childrenWithDiffKindStates = node.ChildrenOfDiffKindState(displayDiffKindStates).OrderBy(u => u.Tag).ToList();

                    foreach (var child in childrenWithDiffKindStates)
                    {
                        var ctn = await DiffNodeToTreeNode(child, displayDiffKindStates, context);
                        if (ctn != null)
                        {
                            retVal.Nodes.Add(ctn);
                        }
                    }
                }

                if (((node.ChangeType | node.ChildrenChangeType)
                        & ((displayDiffKindStates & DiffKindState.ElementMissing)
                            | (displayDiffKindStates & DiffKindState.AttributeMissing))) > 0)
                {
                    retVal.BackColor = MissingColor;
                }
                else if (((node.ChangeType | node.ChildrenChangeType)
                        & ((displayDiffKindStates & DiffKindState.ElementDifferent)
                            | (displayDiffKindStates & DiffKindState.AttributeDifferent))) > 0)
                {
                    retVal.BackColor = DifferentColor;
                }
            }

            if (retVal == null)
            {
                retVal = new TreeNode("None");
            }

            return retVal;
        }

        public void UpdateActiveNodes()
        {
            if (this.isComparing)
            {
                // The in-progress run will pick up the current filters; refresh once it has finished.
                this.viewRefreshRequested = true;
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            UpdateUI();
            this.AutomatedUiInteraction = true;
            this.tableLayoutPanelMain.Enabled = false;

            this.syncedTreeViewLeft.BeginUpdate();
            this.syncedTreeViewRight.BeginUpdate();

            this.syncedTreeViewRight.Nodes.Clear();
            this.syncedTreeViewLeft.Nodes.Clear();

            DiffKindState displayTypes = GetDisplayDiffKindStates();
            TreeNode leftNode = DiffNodeToTreeNode(this.leftDiffNode, displayTypes).GetAwaiter().GetResult(); // synchronous call to the async method, since we are already on the UI thread and want to block until it is done
            TreeNode rightNode = DiffNodeToTreeNode(this.rightDiffNode, displayTypes).GetAwaiter().GetResult(); // see above

            if (leftNode == null || rightNode == null)
            {
                leftNode = new TreeNode("None");
                rightNode = new TreeNode("None");
            }

            this.syncedTreeViewLeft.Nodes.Add(leftNode);
            this.syncedTreeViewRight.Nodes.Add(rightNode);

            this.syncedTreeViewLeft.EndUpdate();
            this.syncedTreeViewRight.EndUpdate();

            this.AutomatedUiInteraction = false;
            this.tableLayoutPanelMain.Enabled = true;
            UpdateUI();

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Handles the AfterExpand event of the treeViewLeft control.
        /// </summary>
        void treeViewLeft_AfterExpand(Object sender, TreeViewEventArgs e)
        {
            if (!this.AutomatedUiInteraction && e.Node != null)
            {
                TreeNode tn = FindNodeByTag(syncedTreeViewRight.Nodes, FullUniqueTag(e.Node));
                if (tn != null)
                {
                    tn.Expand();
                }
            }
        }

        /// <summary>
        /// Handles the AfterExpand event of the treeViewRight control.
        /// </summary>
        void treeViewRight_AfterExpand(Object sender, TreeViewEventArgs e)
        {
            if (!this.AutomatedUiInteraction && e.Node != null)
            {
                TreeNode tn = FindNodeByTag(syncedTreeViewLeft.Nodes, FullUniqueTag(e.Node));
                if (tn != null)
                {
                    tn.Expand();
                }
            }
        }

        /// <summary>
        /// Handles the AfterCollapse event of the treeViewLeft control.
        /// </summary>
        void treeViewLeft_AfterCollapse(Object sender, TreeViewEventArgs e)
        {
            if (!this.AutomatedUiInteraction && e.Node != null)
            {
                TreeNode tn = FindNodeByTag(syncedTreeViewRight.Nodes, FullUniqueTag(e.Node));
                if (tn != null)
                {
                    this.AutomatedUiInteraction = true;
                    this.syncedTreeViewRight.SelectedNode = tn;
                    this.syncedTreeViewRight.SimulatedCollapse(tn);
                    this.AutomatedUiInteraction = false;
                }
            }
        }

        /// <summary>
        /// Handles the AfterCollapse event of the treeViewRight control.
        /// </summary>
        void treeViewRight_AfterCollapse(Object sender, TreeViewEventArgs e)
        {
            if (!this.AutomatedUiInteraction && e.Node != null)
            {
                TreeNode tn = FindNodeByTag(syncedTreeViewLeft.Nodes, FullUniqueTag(e.Node));
                if (tn != null)
                {
                    this.AutomatedUiInteraction = true;
                    syncedTreeViewLeft.SelectedNode = tn;
                    this.syncedTreeViewLeft.SimulatedCollapse(tn);
                    this.AutomatedUiInteraction = false;
                }
            }
        }

        /// <summary>
        /// Handles the BeforeSelect event of the treeViewLeft control.
        /// </summary>
        void treeViewLeft_BeforeSelect(Object sender, TreeViewCancelEventArgs e)
        {
            if (this.syncedTreeViewLeft.SelectedNode != null && this.previousLeftSelectionBgColor != Color.Magenta)
            {
                this.syncedTreeViewLeft.SelectedNode.BackColor = this.previousLeftSelectionBgColor;
                this.previousLeftSelectionBgColor = Color.Magenta;
            }
        }

        /// <summary>
        /// Handles the BeforeSelect event of the treeViewRight control.
        /// </summary>
        void treeViewRight_BeforeSelect(Object sender, TreeViewCancelEventArgs e)
        {
            if (this.syncedTreeViewRight.SelectedNode != null && this.previousRightSelectionBgColor != Color.Magenta)
            {
                this.syncedTreeViewRight.SelectedNode.BackColor = this.previousRightSelectionBgColor;
                this.previousRightSelectionBgColor = Color.Magenta;
            }
        }

        /// <summary>
        /// Handles the AfterSelect event of the treeViewRight control.
        /// </summary>
        void treeViewRight_AfterSelect(Object sender, TreeViewEventArgs e)
        {
            if (!this.AutomatedUiInteraction)
            {
                if (this.syncedTreeViewRight.SelectedNode != null && this.syncedTreeViewRight.SelectedNode.Tag != null)
                {
                    this.previousRightSelectionBgColor = this.syncedTreeViewRight.SelectedNode.BackColor;
                    this.syncedTreeViewRight.SelectedNode.BackColor = SelectedColor;

                    if (this.syncedTreeViewLeft.SelectedNode != null && this.previousLeftSelectionBgColor != Color.Magenta)
                    {
                        this.syncedTreeViewLeft.SelectedNode.BackColor = this.previousLeftSelectionBgColor;
                    }

                    TreeNode tn = FindNodeByTag(this.syncedTreeViewLeft.Nodes, FullUniqueTag(this.syncedTreeViewRight.SelectedNode));
                    if (tn != null)
                    {
                        this.AutomatedUiInteraction = true;
                        this.syncedTreeViewLeft.SelectedNode = tn;
                        this.previousLeftSelectionBgColor = this.syncedTreeViewLeft.SelectedNode.BackColor;
                        this.syncedTreeViewLeft.SelectedNode.BackColor = SelectedColor;
                        this.AutomatedUiInteraction = false;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the AfterSelect event of the treeViewLeft control.
        /// </summary>
        void treeViewLeft_AfterSelect(Object sender, TreeViewEventArgs e)
        {
            if (!this.AutomatedUiInteraction)
            {
                if (this.syncedTreeViewLeft.SelectedNode != null && this.syncedTreeViewLeft.SelectedNode.Tag != null)
                {
                    this.previousLeftSelectionBgColor = this.syncedTreeViewLeft.SelectedNode.BackColor;
                    this.syncedTreeViewLeft.SelectedNode.BackColor = SelectedColor;

                    if (this.syncedTreeViewRight.SelectedNode != null && this.previousRightSelectionBgColor != Color.Magenta)
                    {
                        this.syncedTreeViewRight.SelectedNode.BackColor = this.previousRightSelectionBgColor;
                    }

                    TreeNode tn = FindNodeByTag(this.syncedTreeViewRight.Nodes, FullUniqueTag(this.syncedTreeViewLeft.SelectedNode));
                    if (tn != null)
                    {
                        this.AutomatedUiInteraction = true;
                        this.syncedTreeViewRight.SelectedNode = tn;
                        this.previousRightSelectionBgColor = this.syncedTreeViewRight.SelectedNode.BackColor;
                        this.syncedTreeViewRight.SelectedNode.BackColor = SelectedColor;
                        this.AutomatedUiInteraction = false;
                    }
                }
            }
        }

        public void ExpandAllItems()
        {
            Cursor.Current = Cursors.WaitCursor;

            UpdateUI();
            this.AutomatedUiInteraction = true;
            this.tableLayoutPanelMain.Enabled = false;

            this.syncedTreeViewLeft.BeginUpdate();
            this.syncedTreeViewRight.BeginUpdate();

            this.syncedTreeViewLeft.ExpandAll();
            this.syncedTreeViewRight.ExpandAll();

            this.syncedTreeViewLeft.EndUpdate();
            this.syncedTreeViewRight.EndUpdate();

            this.AutomatedUiInteraction = false;
            this.tableLayoutPanelMain.Enabled = true;

            UpdateUI();
            Cursor.Current = Cursors.Default;
        }

        public void CollapseAllItems()
        {
            Cursor.Current = Cursors.WaitCursor;

            UpdateUI();
            this.AutomatedUiInteraction = true;
            this.tableLayoutPanelMain.Enabled = false;

            this.syncedTreeViewLeft.BeginUpdate();
            this.syncedTreeViewRight.BeginUpdate();

            this.syncedTreeViewLeft.CollapseAll();
            this.syncedTreeViewRight.CollapseAll();

            this.syncedTreeViewLeft.EndUpdate();
            this.syncedTreeViewRight.EndUpdate();

            this.AutomatedUiInteraction = false;
            this.tableLayoutPanelMain.Enabled = true;

            Cursor.Current = Cursors.Default;
            UpdateUI();
        }

        private string PickFile(string startingLocation)
        {
            string retVal = null;

            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Select a file";
                fileDialog.Filter = "XML files (*.XML)|*.XML";
                fileDialog.CheckFileExists = true;
                fileDialog.Multiselect = false;

                if (File.Exists(startingLocation))
                {
                    fileDialog.InitialDirectory = Path.GetDirectoryName(startingLocation);
                }

                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    retVal = fileDialog.FileName;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Handles the Click event of the buttonFileLeftSelect_Click control.
        /// </summary>
        private void buttonFileLeftSelect_Click(object sender, EventArgs e)

        {
            string newFileName = PickFile(this.textBoxFileLeft.Text);

            if (!string.IsNullOrEmpty(newFileName))
            {
                this.textBoxFileLeft.Text = newFileName;
            }

            UpdateUI();
        }

        /// <summary>
        /// Handles the Click event of the buttonFileRightSelect control.
        /// </summary>
        void buttonFileRightSelect_Click(Object sender, EventArgs e)
        {
            string newFileName = PickFile(this.textBoxFileRight.Text);

            if (!string.IsNullOrEmpty(newFileName))
            {
                this.textBoxFileRight.Text = newFileName;
            }

            UpdateUI();
        }

        /// <summary>
        /// Handles the TextChanged event of the file path textboxes.
        /// </summary>
        async void textBox_TextChanged(Object sender, EventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// A TreeView that syncs vertical scrolling with its Partner.
        /// </summary>
        public class SyncedTreeView : TreeView
        {
            [DllImport("user32.dll")]
            private static extern int GetScrollPos(IntPtr hWnd, int nBar);

            [DllImport("user32.dll")]
            private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

            [DllImport("user32.dll")]
            private static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

            [DllImport("user32.dll")]
            private static extern bool SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            private const int WM_VSCROLL = 0x0115;
            private const int WM_MOUSEWHEEL = 0x020A;
            private const int SB_VERT = 1;
            private const int SB_THUMBPOSITION = 4;
            private const int TVM_EXPAND = 0x1102;
            private const int TVE_COLLAPSE = 0x0001;

            public SyncedTreeView Partner;

            private bool isSyncing = false;

            /// <summary>
            /// Collapse a node as if the user had clicked it, retaining child expansion states.
            /// </summary>
            public void SimulatedCollapse(TreeNode treeNode)
            {
                SendMessage(this.Handle, TVM_EXPAND, (IntPtr)TVE_COLLAPSE, treeNode.Handle);
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (this.Partner == null || isSyncing)
                {
                    return;
                }

                if (m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)
                {
                    SyncPartner();
                }
            }

            protected override void OnAfterExpand(TreeViewEventArgs e)
            {
                base.OnAfterExpand(e);
                SyncPartner();
            }

            protected override void OnAfterCollapse(TreeViewEventArgs e)
            {
                base.OnAfterCollapse(e);
                SyncPartner();
            }

            private void SyncPartner()
            {
                this.Partner.isSyncing = true;
                try
                {
                    int scrollPos = GetScrollPos(this.Handle, SB_VERT);
                    SetScrollPos(Partner.Handle, SB_VERT, scrollPos, true);
                    SendMessage(Partner.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * scrollPos, 0);
                }
                finally
                {
                    this.Partner.isSyncing = false;
                }
            }
        }

        private async void buttonCompare_Click(object sender, EventArgs e)
        {
            await LoadAndCompareFilesAsync();
        }

        private void sameItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.LoadingSettings)
            {
                UpdateActiveNodes();
            }
        }

        private void missingItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.LoadingSettings)
            {
                UpdateActiveNodes();
            }
        }

        private void differentItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.LoadingSettings)
            {
                UpdateActiveNodes();
            }
        }

        private void attributesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.LoadingSettings)
            {
                UpdateActiveNodes();
            }
        }

        private void showLineNumbersToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.LoadingSettings)
            {
                UpdateActiveNodes();
            }
        }

        private void elementsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.LoadingSettings)
            {
                UpdateActiveNodes();
            }
        }

        private async void toolStripMenuItemOrderInFile_CheckedChanged(object sender, EventArgs e)
        {
            if (this.toolStripMenuItemOrderInFile.Checked)
            {
                this.toolStripMenuItemNameAndValue.Checked = !this.toolStripMenuItemOrderInFile.Checked;
                this.matchRules = MatchRepeateElementsBy.OrderInFile;
                if (!this.LoadingSettings)
                {
                    await LoadAndCompareFilesAsync();
                }
            }
        }

        private async void toolStripMenuItemNameAndValue_CheckedChanged(object sender, EventArgs e)
        {
            if (this.toolStripMenuItemNameAndValue.Checked)
            {
                this.toolStripMenuItemOrderInFile.Checked = !this.toolStripMenuItemNameAndValue.Checked;
                this.matchRules = MatchRepeateElementsBy.NameAndValue;
                if (!this.LoadingSettings)
                {
                    await LoadAndCompareFilesAsync();
                }
            }
        }

        private void UpdateNamespaceMenuChecks()
        {
            this.toolStripMenuItemNamespaceStandard.Checked = this.namespaceHandling == NamespaceHandling.Standard;
            this.toolStripMenuItemNamespaceLiteral.Checked = this.namespaceHandling == NamespaceHandling.LiteralPrefixes;
            this.toolStripMenuItemNamespaceAutomatic.Checked = this.namespaceHandling == NamespaceHandling.Automatic;
        }

        /// <summary>
        /// Handles the Click event of the three Namespace handling menu items. Like a change to the match mode,
        /// choosing a different setting compares the files again.
        /// </summary>
        private async void namespaceHandlingMenuItem_Click(object sender, EventArgs e)
        {
            NamespaceHandling chosen =
                sender == this.toolStripMenuItemNamespaceStandard ? NamespaceHandling.Standard
                : sender == this.toolStripMenuItemNamespaceLiteral ? NamespaceHandling.LiteralPrefixes
                : NamespaceHandling.Automatic;

            bool changed = chosen != this.namespaceHandling;
            this.namespaceHandling = chosen;
            UpdateNamespaceMenuChecks();

            if (changed && !this.LoadingSettings)
            {
                await LoadAndCompareFilesAsync();
            }
        }

        private void aboutXMLDifferenceToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new AboutForm())
            {
                about.ShowDialog(this);
            }
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExpandAllItems();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CollapseAllItems();
        }
    }
}
