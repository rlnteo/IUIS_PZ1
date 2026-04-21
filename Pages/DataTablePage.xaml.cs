using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Notification.Wpf;
using _90s_Minimalism_CMS_Project.Models;
using _90s_Minimalism_CMS_Project.Helpers;
using _90s_Minimalism_CMS_Project.Windows;

namespace _90s_Minimalism_CMS_Project.Pages
{
    public partial class DataTablePage : Page
    {
        private MainWindow _mainWindow;
        public ObservableCollection<FashionItem> FashionItems { get; set; }

        public DataTablePage()
        {
            InitializeComponent();

            _mainWindow = (MainWindow)Application.Current.MainWindow;
            FashionItems = _mainWindow.FashionItems;
            DataContext = this;

            ConfigureForRole();
            UpdateUI();
        }

        private void ConfigureForRole()
        {
            bool isAdmin = _mainWindow.LoggedInUser.Role == UserRole.Admin;
            CheckColumn.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            SelectAllCheckBox.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            AddButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            DeleteButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateUI()
        {
            int count = FashionItems.Count;
            ItemCountText.Text = $"{count} PIECE{(count != 1 ? "S" : "")}";
            EmptyStateBorder.Visibility = count == 0 ? Visibility.Visible : Visibility.Collapsed;
            FashionItemsDataGrid.Visibility = count == 0 ? Visibility.Collapsed : Visibility.Visible;
            StatusText.Text = count == 0 ? "No fashion items found."
                : $"Showing {count} item{(count != 1 ? "s" : "")}. Click a name to view or edit.";
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in FashionItems)
            {
                item.IsSelected = true;
            }
                
            FashionItemsDataGrid.Items.Refresh();
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in FashionItems)
            {
                item.IsSelected = false;
            }
            FashionItemsDataGrid.Items.Refresh();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateToAddEdit();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var toDelete = FashionItems.Where(i => i.IsSelected).ToList();
            if (toDelete.Count == 0)
            {
                _mainWindow.ShowToast(new ToastNotification(
                    "Nothing selected",
                    "Please check at least one item to delete.",
                    NotificationType.Warning));
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                $"Are you sure you want to delete {toDelete.Count} item(s)?\nThis action cannot be undone!",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                foreach (var item in toDelete)
                {
                    if (!string.IsNullOrEmpty(item.RtfFilePath))
                    {
                        string fullRtf = Path.GetFullPath(item.RtfFilePath);
                        if (File.Exists(fullRtf))
                            File.Delete(fullRtf);
                    }
                    FashionItems.Remove(item);
                }
                SelectAllCheckBox.IsChecked = false;
                _mainWindow.SaveData();
                UpdateUI();

                _mainWindow.ShowToast(new ToastNotification(
                    "Deleted",
                    $"{toDelete.Count} item(s) have been deleted.",
                    NotificationType.Success));
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.SaveData();
            _mainWindow.Close();
        }

        private void FashionItemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = FashionItems.Count(i => i.IsSelected);
            if (selected > 0)
            {
                StatusText.Text = $"{selected} item(s) selected out of {FashionItems.Count} total.";
            }
            else
            {
                UpdateUI();
            }
        }

        private void ItemHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            if (link == null) return;

            FashionItem item = null;
            DependencyObject obj = link.Parent as DependencyObject;
            while (obj != null)
            {
                if (obj is FrameworkElement element && element.DataContext is FashionItem fi)
                {
                    item = fi;
                    break;
                }
                obj = System.Windows.Media.VisualTreeHelper.GetParent(obj);
            }
            if (item == null) return;

            if (_mainWindow.LoggedInUser.Role == UserRole.Admin)
            {
                _mainWindow.NavigateToAddEdit(item);
            }
            else
            {
                ItemDetailWindow detailWindow = new ItemDetailWindow(item);
                detailWindow.Owner = _mainWindow;
                detailWindow.ShowDialog();
            }
        }
    }
}
