﻿using AnyStatus.Models;
using AnyStatus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnyStatus.Views
{
    /// <summary>
    /// Interaction logic for ToolWindowControl.xaml
    /// </summary>
    public partial class ToolWindowControl : UserControl
    {
        public ToolWindowControl(ToolWindowViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            DataContext = viewModel;

            InitializeComponent();
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();

                e.Handled = true;
            }
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        #region Drag and Drop

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;

            if (treeViewItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var data = new DataObject(typeof(Item), treeViewItem.DataContext);

                DragDrop.DoDragDrop(treeViewItem, data, DragDropEffects.Move); //should drag source  be TreeView?
            }
        }

        private void TreeViewItem_DragEnter(object sender, DragEventArgs e)
        {
            //var treeViewItem = sender as TreeViewItem;
            //if (treeViewItem != null &&
            //    treeViewItem.DataContext is Folder &&
            //    e.Data.GetDataPresent(typeof(Item)))
            //{
            //    treeViewItem.Opacity = 0.8;
            //}
        }

        private void TreeViewItem_DragLeave(object sender, DragEventArgs e)
        {
            //var treeViewItem = sender as TreeViewItem;
            //if (treeViewItem != null)
            //{
            //    treeViewItem.Opacity = 1;
            //}
        }

        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = CanMoveItemToTargetFolder(sender as TreeViewItem, e.Data) ?
                            DragDropEffects.Move :
                            DragDropEffects.None;

            e.Handled = true;
        }

        private static bool CanMoveItemToTargetFolder(TreeViewItem targetTreeViewItem, IDataObject data)
        {
            if (targetTreeViewItem != null && data != null && data.GetDataPresent(typeof(Item)))
            {
                var source = data.GetData(typeof(Item)) as Item;
                var target = targetTreeViewItem.DataContext as Folder;

                if (source != null &&
                    target != null &&
                    target != source.Parent &&
                    !source.IsParentOf(target))
                {
                    return true;
                }
            }

            return false;
        }

        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            if (!CanMoveItemToTargetFolder(sender as TreeViewItem, e.Data))
            {
                return;
            }

            var treeViewItem = (TreeViewItem)sender;
            var target = (Item)treeViewItem.DataContext;
            var source = (Item)e.Data.GetData(typeof(Item));

            if (target.Items != null)
            {
                target.Items.Add(source);
            }

            if (source.Parent != null && source.Parent.Items != null && source.Parent.Items.Contains(source))
            {
                source.Parent.Items.Remove(source);
            }

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        #endregion
    }
}
