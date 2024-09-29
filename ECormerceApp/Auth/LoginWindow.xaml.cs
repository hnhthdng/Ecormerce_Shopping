using DataAccess.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataObject.Model;
using ECormerceApp.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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

namespace ECormerceApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UserManager<Accounts> _userManager;

        public LoginWindow(UserManager<Accounts> userManager)
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

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtUser.Text;
            string password = txtPass.Password;

            // Tìm người dùng theo email
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // Kiểm tra mật khẩu
                var result = await _userManager.CheckPasswordAsync(user, password);

                if (result)
                {
                    // Đăng nhập thành công
                    MessageBox.Show("Đăng nhập thành công!");

                    // Lấy MainWindow từ ServiceProvider để inject các dependency (nếu có)
                    var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                    this.Hide();

                    // Hiển thị lại cửa sổ Login khi MainWindow đóng
                    mainWindow.Closed += (s, args) => this.Show();
                }
                else
                {
                    MessageBox.Show("Mật khẩu không chính xác.");
                }
            }
            else
            {
                MessageBox.Show("Email không tồn tại.");
            }
        }

        private void TextBlock_MouseLeftButtonDown_SignUp(object sender, MouseButtonEventArgs e)
        {
            // Lấy MainWindow từ ServiceProvider để inject các dependency (nếu có)
            var registerWindow = App.ServiceProvider.GetRequiredService<RegisterWindow>();
            registerWindow.Show();
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown_Reset(object sender, MouseButtonEventArgs e)
        {
            // Lấy MainWindow từ ServiceProvider để inject các dependency (nếu có)
            var ForgotPassword = App.ServiceProvider.GetRequiredService<ForgotPassword>();
            ForgotPassword.Show();
            this.Close();
        }
    }
}
