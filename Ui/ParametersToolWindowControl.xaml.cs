namespace VSRipGrep.Ui
{
    using VSRipGrep.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using VSRipGrep.Models;
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for ParametersToolWindowControl.
    /// </summary>
    public partial class ParametersToolWindowControl : UserControl, INotifyPropertyChanged
    {
        internal ParametersModel Parameters { get; } = new ParametersModel();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RipGrepToolWindowControl"/> class.
        /// </summary>
        public ParametersToolWindowControl()
        {
            this.InitializeComponent();
            this.DataContext = Parameters;

            var configuration = ToolWindowFactory.Package.GetDialogPage(typeof(Package.Configuration)) as Package.Configuration;
            if (configuration != null)
            {
                configuration.PropertyChanged += Configuration_PropertyChanged;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Configuration_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RipGrepExecutable")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsValidRipGrepExecutable"));
            }
        }

        public bool IsValidRipGrepExecutable
        {
            get
            {
                var configuration = ToolWindowFactory.Package.GetDialogPage(typeof(Package.Configuration)) as Package.Configuration;
                return configuration != null ? System.IO.File.Exists(configuration.RipGrepExecutable) : false;
            }
        }

        private void FindAll_Click(object sender, RoutedEventArgs e)
        {
            var resultsToolWindowControl = ToolWindowFactory.ShowResultsToolWindow()?.Content as ResultsToolWindowControl;
            if (resultsToolWindowControl != null)
            {
                var ripGrepTask = new RipGrepTask(Parameters.Clone());

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