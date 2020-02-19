namespace VSRipGrep.Ui
{
    using VSRipGrep.Models;
    using System.Windows.Controls;
    using VSRipGrep.Tasks;
    using System.ComponentModel;
    using System;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio;
    using VSRipGrep.Tools;
    using System.Windows;

    /// <summary>
    /// Interaction logic for ParametersToolWindowControl.
    /// </summary>
    public partial class ResultsToolWindowControl : UserControl, INotifyPropertyChanged
    {
        private RipGrepTask m_ripGrepTask = null;

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private void OnRipGrepTaskChanged(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSearching"));
        }

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
                    m_ripGrepTask.TaskChanged -= OnRipGrepTaskChanged;
                }

                DataContext = value?.Results;
                m_ripGrepTask = value;
                m_ripGrepTask.TaskChanged += OnRipGrepTaskChanged;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSearching"));
            }
        }

        public bool IsSearching
        {
            get
            {
                return m_ripGrepTask != null && m_ripGrepTask.IsRunning;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsToolWindowControl"/> class.
        /// </summary>
        public ResultsToolWindowControl()
        {
            DataContext = new ResultFilesModel(string.Empty);
            this.InitializeComponent();
        }

        public void ExecuteCommand(VSConstants.VSStd97CmdID commandId)
        {
            if (commandId != VSConstants.VSStd97CmdID.NextLocation
                && commandId != VSConstants.VSStd97CmdID.PreviousLocation)
            {
                return;
            }

            var treeView = FindName("Results") as TreeView;
            if (treeView == null)
            {
                return;
            }

            var selectedItem = treeView.SelectedItem as ResultModelBase;
            if (selectedItem == null)
            {
                selectedItem = DataContext as ResultModelBase;
            }

            var nextItem = commandId == VSConstants.VSStd97CmdID.NextLocation
                ? ResultModelHelper.Next(selectedItem) : ResultModelHelper.Previous(selectedItem);

            TreeViewHelper.ExpandTreeViewItemModel(treeView, nextItem);

            var treeViewItem = TreeViewHelper.TreeViewItemFromModel(treeView, nextItem);
            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;

                treeViewItem.BringIntoView();
                var scrollViewer = treeView.Template.FindName("_tv_scrollviewer_", treeView) as ScrollViewer;
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToLeftEnd();
                }

                ResultModelHelper.Edit(nextItem);
            }
        }

        public OLECMDF QueryCommandStatus(VSConstants.VSStd97CmdID commandId)
        {
            if (commandId != VSConstants.VSStd97CmdID.NextLocation 
                && commandId != VSConstants.VSStd97CmdID.PreviousLocation)
            {
                return OLECMDF.OLECMDF_INVISIBLE | OLECMDF.OLECMDF_DEFHIDEONCTXTMENU;
            }

            var treeView = FindName("Results") as TreeView;
            if (treeView == null)
            {
                return OLECMDF.OLECMDF_INVISIBLE | OLECMDF.OLECMDF_DEFHIDEONCTXTMENU;
            }

            var selectedValue = treeView.IsVisible ? treeView.SelectedValue as ResultModelBase : null;
            if (selectedValue == null)
            {
                selectedValue = DataContext as ResultModelBase;
            }

            var nextValue = commandId == VSConstants.VSStd97CmdID.NextLocation 
                ? ResultModelHelper.Next(selectedValue) : ResultModelHelper.Previous(selectedValue);

            return nextValue == null ? OLECMDF.OLECMDF_SUPPORTED : OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
        }

        private void Results_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeView = e.Source as TreeView;
            var selectedUiElement = treeView?.InputHitTest(e.GetPosition(treeView)) as FrameworkContentElement;

            if (ResultModelHelper.Edit(selectedUiElement?.DataContext as ResultModelBase))
            {
                e.Handled = true;
            }
        }

        private void Results_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }

            var treeView = e.Source as TreeView;
            var selectedModel = treeView.SelectedItem as ResultModelBase;
            if (ResultModelHelper.Edit(selectedModel))
            {
                e.Handled = true;
            }
        }
    }
}
