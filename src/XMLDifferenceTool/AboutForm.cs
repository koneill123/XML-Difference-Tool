using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using XMLDifferenceTool.Core;

namespace XMLDifferenceTool
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            this.Text = "About " + Application.ProductName;
            this.labelTitle.Text = Application.ProductName;
            this.labelVersion.Text = "Version " + GetVersionText();
            this.labelDescription.Text = ProjectInformation.DescriptionText;
            this.labelCopyright.Text = ProjectInformation.CopyrightText;
            this.linkLabelProject.Text = ProjectInformation.ProjectUrl;
        }

        private static string GetVersionText()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Prefer the informational version (e.g. "1.2.0"), dropping any "+commit" suffix
            string informational = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (!string.IsNullOrWhiteSpace(informational))
            {
                int plus = informational.IndexOf('+');
                return plus >= 0 ? informational.Substring(0, plus) : informational;
            }

            Version version = assembly.GetName().Version;
            return version != null ? version.ToString(3) : "unknown";
        }

        private void linkLabelProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(ProjectInformation.ProjectUrl) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not open the link:\n" + ex.Message, "About",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
