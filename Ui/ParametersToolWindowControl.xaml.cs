namespace VSRipGrep.Ui
{
    using VSRipGrep.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using VSRipGrep.Models;

    /// <summary>
    /// Interaction logic for ParametersToolWindowControl.
    /// </summary>
    public partial class ParametersToolWindowControl : UserControl
    {
        internal ParametersModel Parameters { get; } = new ParametersModel();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RipGrepToolWindowControl"/> class.
        /// </summary>
        public ParametersToolWindowControl()
        {
            this.InitializeComponent();
            this.DataContext = Parameters;
        }

        private void FindAll_Click(object sender, RoutedEventArgs e)
        {
            var resultsToolWindowControl = ToolWindowFactory.ShowResultsToolWindow()?.Content as ResultsToolWindowControl;
            if (resultsToolWindowControl != null)
            {
                var ripGrepTask = new RipGrepTask();

                resultsToolWindowControl.ResultTask = ripGrepTask;
                ripGrepTask.Run();
            }
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var folderSelection = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (folderSelection.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Parameters.Path = folderSelection.SelectedPath;
                }
            }
        }
    }
}