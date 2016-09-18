using AnyStatus.Infrastructure;
using AnyStatus.Models;
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
            DataContext = _viewModel = Preconditions.CheckNotNull(viewModel, nameof(viewModel));

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
            try
            {
                var source = GetSource(e);
                var target = GetTarget(sender);

                e.Effects = source != null && target != null && source.CanMoveTo(target) ?
                     DragDropEffects.Move :
                     DragDropEffects.None;
            }
            catch
            {
                e.Effects = DragDropEffects.None;
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
                var source = GetSource(e);
                var target = GetTarget(sender);

                if (source == null || target == null || source.CanMoveTo(target) == false)
                    return;

                source.MoveTo(target);

                target.IsExpanded = true;

                Save();
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

        private void Save()
        {
            if (_viewModel.SaveCommand.CanExecute(null))
                _viewModel.SaveCommand.Execute(null);
        }

        private static Item GetSource(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Item)))
                return (Item)e.Data.GetData(typeof(Item));

            return null;
        }

        private Item GetTarget(object sender)
        {
            if (sender is TreeViewItem && ((TreeViewItem)sender).DataContext is Item)
            {
                return (Item)((TreeViewItem)sender).DataContext;
            }

            return _viewModel?.RootItem;
        }

        #endregion
    }
}
