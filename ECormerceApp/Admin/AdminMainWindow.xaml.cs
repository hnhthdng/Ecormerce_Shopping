using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.VisualBasic.ApplicationServices;
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

namespace ECormerceApp.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public Accounts loggedInUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Accounts> _userManager;

        public AdminMainWindow(IUnitOfWork unitOfWork, UserManager<Accounts> userManager)
        {
            InitializeComponent();
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }
        private void ShowOnlyStackPanel(StackPanel visiblePanel)
        {
            // Ẩn tất cả các StackPanel
            Welcome_Dashboard.Visibility = Visibility.Hidden;
            AccountContent.Visibility = Visibility.Hidden;
            CategoryContent.Visibility = Visibility.Hidden;
            ProfileContent.Visibility = Visibility.Hidden;
            ChangePasswordContent.Visibility = Visibility.Hidden;
            // Hiển thị StackPanel mong muốn
            visiblePanel.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (loggedInUser.FullName == null)
            {
                txtUser.Text = loggedInUser.Email;

                FirtAlphaNameProfile.Text = loggedInUser.Email.Substring(0, 1).ToUpper();
            }
            else
            {
                txtUser.Text = loggedInUser.FullName;
                FirtAlphaNameProfile.Text = loggedInUser.FullName.Substring(0, 1).ToUpper();
            }
            txtWelcomeUser.Text = "Welcome " + loggedInUser.FullName;
            ShowOnlyStackPanel(Welcome_Dashboard);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Profile

        private void Profile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowOnlyStackPanel(ProfileContent);

            txtEmailInProfileContent.Text = loggedInUser.Email;
            txtFullNameInProfileContent.Text = loggedInUser.FullName;
            txtPhoneNumberInProfileContent.Text = loggedInUser.PhoneNumber;

            txtFullNameInProfileContent.IsEnabled = false;
            txtPhoneNumberInProfileContent.IsEnabled = false;

            btnUpdateInProfileContent.Visibility = Visibility.Visible;
            btnSaveInProfileContent.Visibility = Visibility.Collapsed;
        }


        private void btnUpdateInProfileContent_Click(object sender, RoutedEventArgs e)
        {
            txtFullNameInProfileContent.IsEnabled = true;
            txtPhoneNumberInProfileContent.IsEnabled = true;
            btnUpdateInProfileContent.Visibility = Visibility.Collapsed;
            btnSaveInProfileContent.Visibility = Visibility.Visible;
        }

        private void btnSaveInProfileContent_Click(object sender, RoutedEventArgs e)
        {
            if (txtFullNameInProfileContent.Text == "" || txtPhoneNumberInProfileContent.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                loggedInUser.FullName = txtFullNameInProfileContent.Text;
                loggedInUser.PhoneNumber = txtPhoneNumberInProfileContent.Text;

                _userManager.UpdateAsync(loggedInUser);

                MessageBox.Show("Update successfully");
                txtFullNameInProfileContent.IsEnabled = false;
                txtPhoneNumberInProfileContent.IsEnabled = false;
                btnUpdateInProfileContent.Visibility = Visibility.Visible;
                btnSaveInProfileContent.Visibility = Visibility.Collapsed;
            }
        }

        private void btnChangePasswordInProfileContent_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(ChangePasswordContent);
            btnChangePasswordInProfileContent.Visibility = Visibility.Collapsed;
            btnSaveInProfileContent.Visibility = Visibility.Visible;
        }

        #endregion

        #region ChangePassword
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldPassword = txtOldPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            if (oldPassword == newPassword)
            {
                MessageBox.Show("New password must be different from old password");
            }
            else if (oldPassword != newPassword)
            {
                if (newPassword == confirmPassword)
                {
                    var result = _userManager.ChangePasswordAsync(loggedInUser, oldPassword, newPassword);
                    if (result.Result.Succeeded)
                    {
                        MessageBox.Show("Change password successfully");
                        txtOldPassword.Password = "";
                        txtNewPassword.Password = "";
                        txtConfirmPassword.Password = "";
                    }
                    else
                    {
                        // Biến lưu trữ chuỗi lỗi
                        string errors = string.Join("\n", result.Result.Errors.Select(e => e.Description));

                        // Hiển thị lỗi trong MessageBox
                        MessageBox.Show(errors, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Confirm password is not match");
                }
            }
        }
        #endregion

        #region Button Daskboard

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(Welcome_Dashboard);
        }

        #endregion

        #region Button Account
        private void Account_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(AccountContent);
        }
        #endregion

    }
}
