using DataObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace ECormerceApp
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly UserManager<Accounts> _userManager;
        public RegisterWindow(UserManager<Accounts> userManager)
        {
            InitializeComponent();
            _userManager = userManager;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtUser.Text;
                string password = txtPass.Password;
                string confirmPassword = txtConfirmPass.Password;

                if (password != confirmPassword)
                {
                    MessageBox.Show("Password and Confirm Password do not match");
                    return;
                }

                var user = new Accounts
                {
                    Email = email,
                    UserName = email
                };

                var result = _userManager.CreateAsync(user, password).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Staff");
                    user.Type = 1;
                    _userManager.UpdateAsync(user);
                    MessageBox.Show("User created successfully");
                    var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
                    loginWindow.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Error: " + result.Errors);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
            this.Hide();
        }
    }
}
