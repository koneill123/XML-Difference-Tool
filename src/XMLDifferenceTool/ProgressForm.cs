using XMLDifferenceTool.Core;

namespace XMLDifferenceTool
{
    /// <summary>
    /// A small modal dialog that shows a status message and a progress bar while files are loaded and compared.
    /// It has no Cancel button and cannot be closed by the user; it closes itself when the work finishes.
    /// </summary>
    internal sealed partial class ProgressForm : Form
    {
        private bool allowClose;

        public ProgressForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the status message and progress bar. Call on the UI thread (an IProgress created on the
        /// UI thread does this automatically).
        /// </summary>
        public void Report(DiffProgress progress)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.labelStatus.Text = progress.Message;

            if (progress.Fraction is double fraction)
            {
                if (this.progressBar.Style != ProgressBarStyle.Continuous)
                {
                    this.progressBar.Style = ProgressBarStyle.Continuous;
                }
                this.progressBar.Value = Math.Clamp((int)Math.Round(fraction * 100), 0, 100);
            }
            else if (this.progressBar.Style != ProgressBarStyle.Marquee)
            {
                this.progressBar.Style = ProgressBarStyle.Marquee;
            }
        }

        /// <summary>
        /// Closes the dialog. This is the only way it can be closed.
        /// </summary>
        public void CloseWhenDone()
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.allowClose = true;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Block Alt+F4 and similar; there is no cancel option.
            if (!this.allowClose && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Runs <paramref name="work"/> and shows the progress dialog (modal to <paramref name="owner"/>) only if the
        /// work is still running after <paramref name="showAfter"/>. The dialog closes when the work completes.
        /// Must be called on the UI thread. Exceptions thrown by the work are rethrown to the caller.
        /// </summary>
        public static async Task RunAsync(IWin32Window owner, TimeSpan showAfter, Func<IProgress<DiffProgress>, Task> work)
        {
            using (ProgressForm dialog = new ProgressForm())
            {
                // Created on the UI thread, so Report callbacks are marshalled back to the UI thread.
                Progress<DiffProgress> progress = new Progress<DiffProgress>(dialog.Report);

                Task workTask = work(progress);
                Task completed = await Task.WhenAny(workTask, Task.Delay(showAfter));

                if (completed != workTask)
                {
                    // Still running after the delay: show the dialog, and close it as soon as the work finishes.
                    _ = workTask.ContinueWith(
                        _ => dialog.CloseWhenDone(),
                        CancellationToken.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.FromCurrentSynchronizationContext());

                    dialog.ShowDialog(owner);
                }

                // Propagates any exception from the work.
                await workTask;
            }
        }
    }
}
