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
using _90s_Minimalism_CMS_Project.Models;
using _90s_Minimalism_CMS_Project.Helpers;
using _90s_Minimalism_CMS_Project.Pages;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.IO;

namespace _90s_Minimalism_CMS_Project
{
    public partial class MainWindow : Window
    {
        private readonly NotificationManager _notificationManager;
        private readonly DataIO _dataIO = new DataIO();
        public ObservableCollection<FashionItem> FashionItems { get; set; }
        public User LoggedInUser { get; private set; }
        public MainWindow(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _notificationManager = new NotificationManager();
            Application.Current.MainWindow = this;

            Directory.CreateDirectory("Data");
            FashionItems = _dataIO.DeSerializeObject<ObservableCollection<FashionItem>>("Data\\FashionItems.xml")
                           ?? new ObservableCollection<FashionItem>();

            ConfigureUIForRole();
            NavigateToDataTable();
        }

        private void ConfigureUIForRole()
        {
            string roleLabel = LoggedInUser.Role == UserRole.Admin ? "ADMIN" : "VISITOR";
            UserInfoText.Text = $"{LoggedInUser.Username.ToUpper()}  ·  {roleLabel}";

            NavAddButton.Visibility = LoggedInUser.Role == UserRole.Admin
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public void ShowToast(ToastNotification notification)
        {
            _notificationManager.Show(notification.Title, notification.Message,
                notification.Type, "WindowNotificationArea");
        }

        public void SaveData()
        {
            Directory.CreateDirectory("Data");
            _dataIO.SerializeObject(FashionItems, "Data\\FashionItems.xml");
        }

        public void NavigateToDataTable()
        {
            MainFrame.Navigate(new DataTablePage());
        }
        public void NavigateToAddEdit(FashionItem itemToEdit = null)
        {
            MainFrame.Navigate(new AddEditItemPage(itemToEdit));
        }
        private void NavAddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToAddEdit();
        }
        private void NavCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDataTable();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            this.Close();
        }
    }
}
