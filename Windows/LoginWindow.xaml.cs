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
using System.Windows.Shapes;
using _90s_Minimalism_CMS_Project.Helpers;
using _90s_Minimalism_CMS_Project.Models;
using System.IO;

namespace _90s_Minimalism_CMS_Project.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly DataIO _dataIO = new DataIO();
        private List<User> _users;
        private const string UsernamePlaceholder = "Enter username";
        public LoginWindow()
        {
            InitializeComponent();
            _users = _dataIO.DeSerializeObject<List<User>>("Data\\Users.xml");
            if(_users == null || _users.Count == 0) 
            {
                SeedDefaultUsers();
            }

            //Placeholder
            UsernameTextBox.Text = UsernamePlaceholder;
            UsernameTextBox.Foreground = (Brush)Application.Current.FindResource("BrushMidtone");
        }

        private void SeedDefaultUsers()
        {
            _users = new List<User>
            {
                new User("admin",   "admin123",   UserRole.Admin),
                new User("visitor", "visitor123", UserRole.Visitor)
            };

            Directory.CreateDirectory("Data");
            _dataIO.SerializeObject(_users, "Data\\Users.xml");
        }

        private bool ValidateInputs()
        {
            bool isValid = true;
            string username = UsernameTextBox.Text.Trim();
            if(string.IsNullOrEmpty(username) || username == UsernamePlaceholder)
            {
                UsernameErrorText.Text = "Username cannot be empty!";
                UsernameErrorText.Visibility = Visibility.Visible;

                UsernameTextBox.BorderBrush = (Brush)Application.Current.FindResource("BrushError");
                isValid = false;
            }
            else
            {
                UsernameErrorText.Visibility = Visibility.Collapsed;
                UsernameTextBox.BorderBrush = (Brush)Application.Current.FindResource("BrushDivider");
            }

            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordErrorText.Text = "Password cannot be empty.";
                PasswordErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                PasswordErrorText.Visibility = Visibility.Collapsed;
            }
            return isValid;
        }

        private void ResetForm()
        {
            UsernameTextBox.Text = UsernamePlaceholder;
            UsernameTextBox.Foreground = (Brush)Application.Current.FindResource("BrushMidtone");
            UsernameTextBox.ClearValue(BorderBrushProperty);

            PasswordBox.Password = string.Empty;
            LoginErrorText.Visibility = Visibility.Collapsed;
            UsernameErrorText.Visibility = Visibility.Collapsed;
            PasswordErrorText.Visibility = Visibility.Collapsed;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text.Trim().Equals(UsernamePlaceholder))
            {
                UsernameTextBox.Text = string.Empty;
                UsernameTextBox.Foreground = (Brush)Application.Current.FindResource("BrushInk");
            }
            LoginErrorText.Visibility = Visibility.Collapsed;
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text.Trim().Equals(string.Empty))
            {
                UsernameTextBox.Text = UsernamePlaceholder;
                UsernameTextBox.Foreground = (Brush)Application.Current.FindResource("BrushMidtone");
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginErrorText.Visibility = Visibility.Collapsed;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(!ValidateInputs())
            {
                return;
            }

            string enteredUsername = UsernameTextBox.Text.Trim();
            string enteredPassword = PasswordBox.Password;

            User matchedUser = _users.Find(u =>
               u.Username == enteredUsername && u.Password == enteredPassword);
            if (matchedUser != null)
            { 
                MainWindow mainWindow = new MainWindow(matchedUser);
                Application.Current.MainWindow = mainWindow;  // Add this line
                mainWindow.Show();

                this.Hide();
                mainWindow.Closed += (s, args) =>
                {
                    this.Show();
                    ResetForm();
                };
            }
            else
            {
                LoginErrorText.Text = "Incorrect username or password. Please try again.";
                LoginErrorText.Visibility = Visibility.Visible;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
