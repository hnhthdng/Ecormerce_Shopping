using DataAccess.Repository.IRepository;
using DataObject.Model;
using ECormerceApp.UserControls;
using LiveCharts;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        public ChartValues<decimal> ProfitValues { get; set; }

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
            Show3TotalOnDashBoardCard();
            Show5LatestOrder();
            LoadProfitChartData();
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
        private void Show3TotalOnDashBoardCard()
        {
            //Show Total Product
            TotalProduct_Dashboard.Number = _unitOfWork.Product.GetAll().Count().ToString();
            //Show Total Order
            TotalOrder_Dashboard.Number = _unitOfWork.Order.GetAll().Count().ToString();
            //Show Total Revenue
            TotalRevenue_Dashboard.Number = _unitOfWork.Order.GetAll().Sum(x => x.Freight).ToString();
        }
        private void Show5LatestOrder()
        {
            var orderList = _unitOfWork.Order.GetAll(includeProperty: "OrderDetails,Accounts").OrderByDescending(x => x.OrderDate).Take(5).ToList();

            // Clear existing items
            LastOrdersStackPanel.Children.Clear();

            // Add the latest orders to the StackPanel
            foreach (var order in orderList)
            {
                var firtOrderDetail = order.OrderDetails.FirstOrDefault();
                var product = _unitOfWork.Product.GetFirstOrDefault(x => x.ProductID == firtOrderDetail.ProductID);
                var item = new Item
                {
                    Title = "Customer Id: " + order.CustomerID,  // Assuming you have a property for product name
                    Desc = $"{product.ProductName},... - {order.OrderDate.ToString("HH:mm")}",  // Assuming you have OrderTime and CustomerName
                    Icon = FontAwesome.Sharp.IconChar.ShoppingBag
                };
                LastOrdersStackPanel.Children.Add(item);
            }
        }
        private async void LoadProfitChartData()
        {
            List<int> profitEachMonth = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                var profit = _unitOfWork.Order.GetAll().Where(x => x.OrderDate.Month == i).Sum(x => x.Freight);
                profitEachMonth.Add((int)profit);
            }
            Slm.Values = new ChartValues<int>(profitEachMonth.ToArray());

        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(Welcome_Dashboard);
            Show3TotalOnDashBoardCard();
            Show5LatestOrder();
            LoadProfitChartData();
        }

        #endregion
    }
}
