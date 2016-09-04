﻿using AnyStatus.Models;
using AnyStatus.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AnyStatus.Views
{
    /// <summary>
    /// Interaction logic for ToolWindowControl.xaml
    /// </summary>
    public partial class ToolWindowControl : UserControl
    {
        ToolWindowViewModel _viewModel;
        public ToolWindowControl(ToolWindowViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            DataContext = _viewModel = viewModel;

            InitializeComponent();
        }

        /// <summary>
        /// Select a tree view item on right-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Unselects a tree view item when clicking on the empty space in the tree view.
        /// </summary>
        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = sender as TreeView;

            if (treeView != null && treeView.SelectedItem != null && treeView.SelectedItem is Item)
            {
                var item = (Item)treeView.SelectedItem;
                item.IsSelected = false;
                treeView.Focus();
            }
        }

        #region Drag and Drop

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;

            if (treeViewItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var data = new DataObject(typeof(Item), treeViewItem.DataContext);

                DragDrop.DoDragDrop(treeViewItem, data, DragDropEffects.Move);
            }
        }

        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            try
            {
                if (!e.Data.GetDataPresent(typeof(Item)))
                    return;

                var source = GetSource(e);
                var target = GetTarget(sender);

                if (target != null && source != null && source.CanMoveTo(target))
                {
                    e.Effects = DragDropEffects.Move;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                e.Handled = true;
            }
        }

        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(typeof(Item)))
                    return;

                var source = GetSource(e);
                var target = GetTarget(sender);

                if (target == null || source == null || !source.CanMoveTo(target))
                    return;

                source.MoveTo(target);

                target.IsExpanded = true;

                if (_viewModel.SaveCommand.CanExecute(null))
                    _viewModel.SaveCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private static Item GetSource(DragEventArgs e)
        {
            return (Item)e.Data.GetData(typeof(Item));
        }

        private Item GetTarget(object sender)
        {
            Item target = null;

            if (sender is TreeViewItem)
            {
                target = ((TreeViewItem)sender).DataContext as Item;
            }
            else if (sender is TreeView)
            {
                target = _viewModel?.RootItem;
            }

            return target;
        }

        #endregion
    }
}
