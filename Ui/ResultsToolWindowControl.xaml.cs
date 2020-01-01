﻿namespace VSRipGrep.Ui
{
    using VSRipGrep.Models;
    using System.Windows.Controls;
    using VSRipGrep.Tasks;

    /// <summary>
    /// Interaction logic for ParametersToolWindowControl.
    /// </summary>
    public partial class ResultsToolWindowControl : UserControl
    {
        private RipGrepTask m_ripGrepTask = null;
        internal RipGrepTask ResultTask
        { 
            get
            {
                return m_ripGrepTask;
            }
            set
            {
                if (m_ripGrepTask == value)
                {
                    return;
                }

                if (m_ripGrepTask != null)
                {
                    m_ripGrepTask.Cancel();
                }

                DataContext = value?.Results;
                m_ripGrepTask = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsToolWindowControl"/> class.
        /// </summary>
        public ResultsToolWindowControl()
        {
            DataContext = new ResultFilesModel();
            this.InitializeComponent();
        }

        private void Results_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeView = e.Source as TreeView;
            var selectedUiElement = treeView?.InputHitTest(e.GetPosition(treeView)) as TextBlock;
            var selectedFileModel = selectedUiElement?.DataContext as ResultFileModel;
            var selectedLineModel = selectedUiElement?.DataContext as ResultLineModel;

            if (selectedFileModel != null && selectedFileModel.ResultLines.Count == 0)
            {
                selectedFileModel.GotoFile();
                e.Handled = true;
            }
            else if (selectedLineModel != null)
            {
                selectedLineModel.GotoLocation();
                e.Handled = true;
            }
        }
    }
}