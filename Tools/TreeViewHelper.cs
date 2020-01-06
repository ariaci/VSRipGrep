using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using VSRipGrep.Models;

namespace VSRipGrep.Tools
{
    internal sealed class TreeViewHelper
    {
        internal static TreeViewItem TreeViewItemFromModel(ItemsControl itemsControl, ResultModelBase model)
        {
            if (model == null || itemsControl?.ItemContainerGenerator == null)
            {
                return null;
            }

            var treeViewItem = itemsControl.ItemContainerGenerator.ContainerFromItem(model) as TreeViewItem;
            if (treeViewItem != null)
            {
                return treeViewItem;
            }

            foreach (object currentModel in itemsControl.Items)
            {
                treeViewItem = itemsControl.ItemContainerGenerator.ContainerFromItem(currentModel) as TreeViewItem;
                treeViewItem = TreeViewItemFromModel(treeViewItem, model);

                if (treeViewItem != null)
                {
                    return treeViewItem;
                }
            }

            return null;
        }

        internal static void ExpandTreeViewItemModel(TreeView treeView, ResultModelBase model)
        {
            if (model == null)
            {
                return;
            }

            List<ResultModelBase> models = new List<ResultModelBase>();
            models.Insert(0, model as ResultLineModel);
            models.Insert(0, (model as ResultLineModel)?.File);
            models.Insert(0, model as ResultFileModel);

            bool updateLayout = false;
            foreach (var currentModel in models)
            {
                var treeViewItem = TreeViewItemFromModel(treeView, currentModel);
                if (treeViewItem != null && !treeViewItem.IsExpanded)
                {
                    treeViewItem.IsExpanded = true;
                    updateLayout = true;
                }
            }

            if (updateLayout)
            {
                treeView.UpdateLayout();
            }
        }
    }
}
