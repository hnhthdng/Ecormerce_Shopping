using DataAccess.Repository.IRepository;
using DataObject.Model;
using ECormerceApp.UserControls;
using LiveCharts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility;

namespace ECormerceApp.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public ChartValues<decimal> ProfitValues { get; set; }

        public Accounts loggedInUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Accounts> _userManager;

        //Pagination
        private PaginationHelper<Accounts> paginationHelperAccount;

        public AdminMainWindow(IUnitOfWork unitOfWork, UserManager<Accounts> userManager)
        {
            InitializeComponent();
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            paginationHelperAccount = new PaginationHelper<Accounts>(7);

        }

        #region Method
        private void ShowOnlyStackPanel(StackPanel visiblePanel)
        {
            // Ẩn tất cả các StackPanel
            Welcome_Dashboard.Visibility = Visibility.Hidden;
            ProfileContent.Visibility = Visibility.Hidden;
            ChangePasswordContent.Visibility = Visibility.Hidden;
            AccountContent.Visibility = Visibility.Hidden;

            // Hiển thị StackPanel mong muốn
            visiblePanel.Visibility = Visibility.Visible;
        }

        private void LoadDataForPage<T>(DataGrid dataGrid, IEnumerable<T> list, PaginationHelper<T> paginationHelper)
        {
            var pagedData = paginationHelper.GetPagedData(list);

            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = pagedData;
        }

        private void LoadFullData<T>(DataGrid dataGrid, IEnumerable<T> list)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = list;
        }

        #endregion

        #region Window Event

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
        #endregion

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

        #region Account
        private string currentTableAccount = "Total";
        private void Account_Click(object sender, RoutedEventArgs e)
        {
            currentTableAccount = "Total";
            ShowOnlyStackPanel(AccountContent);
            var accounts = _unitOfWork.Account.GetAll();
            LoadDataForPage<Accounts>(AccountDataGrid, accounts, paginationHelperAccount);
            TotalAccountWithType.Text = "Total: " + accounts.Count().ToString();
        }

        private void PreviousPageAccountContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Accounts> accounts;
            // Lọc dựa trên loại bảng hiện tại (Admin, Normal User, Total)
            if (currentTableAccount == "Admin")
            {
                accounts = _unitOfWork.Account.GetAll().Where(a => a.Type == 1);
            }
            else if (currentTableAccount == "NormalUser")
            {
                accounts = _unitOfWork.Account.GetAll().Where(a => a.Type == 2);
            }
            else
            {
                accounts = _unitOfWork.Account.GetAll(); // Total - hiển thị tất cả
            }
            paginationHelperAccount.PreviousPage();
            LoadDataForPage<Accounts>(AccountDataGrid, accounts, paginationHelperAccount);
        }

        private void NextPageAccountContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Accounts> accounts;
            // Lọc dựa trên loại bảng hiện tại (Admin, Normal User, Total)
            if (currentTableAccount == "Admin")
            {
                accounts = _unitOfWork.Account.GetAll().Where(a => a.Type == 1);
            }
            else if (currentTableAccount == "NormalUser")
            {
                accounts = _unitOfWork.Account.GetAll().Where(a => a.Type == 2);
            }
            else
            {
                accounts = _unitOfWork.Account.GetAll(); // Total - hiển thị tất cả
            }
            paginationHelperAccount.NextPage(accounts);
            LoadDataForPage<Accounts>(AccountDataGrid, accounts, paginationHelperAccount);
        }

        private void btn_Account_Admin_GetTotal_Click(object sender, RoutedEventArgs e)
        {
            currentTableAccount = "Admin";
            TotalAccountWithType.Text = "Total: " + _unitOfWork.Account.GetAll().Where(x => x.Type == 1).Count().ToString();
            var accounts = _unitOfWork.Account.GetAll().Where(x => x.Type == 1);
            LoadDataForPage<Accounts>(AccountDataGrid, accounts, paginationHelperAccount);
        }

        private void btn_Account_NormalUser_GetTotal_Click(object sender, RoutedEventArgs e)
        {
            currentTableAccount = "NormalUser";
            TotalAccountWithType.Text = "Total: " + _unitOfWork.Account.GetAll().Where(x => x.Type == 2).Count().ToString();
            var accounts = _unitOfWork.Account.GetAll().Where(x => x.Type == 2);
            LoadDataForPage<Accounts>(AccountDataGrid, accounts, paginationHelperAccount);
        }
        private void textBoxFilter_AccountContent_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                string filter = textBoxFilter_AccountContent.Text;
                try
                {
                    IEnumerable<Accounts> accounts;
                    // Lọc dựa trên loại bảng hiện tại (Admin, Normal User, Total)
                    if (currentTableAccount == "Admin")
                    {
                        accounts = _unitOfWork.Account.GetAll().Where(a => a.Type == 1 && a.Email.Substring(0, a.Email.IndexOf("@")).Contains(filter));
                        LoadFullData(AccountDataGrid, accounts);
                    }
                    else if (currentTableAccount == "NormalUser")
                    {
                        accounts = _unitOfWork.Account.GetAll().Where(a => a.Type == 2 && a.Email.Substring(0, a.Email.IndexOf("@")).Contains(filter));
                        LoadFullData(AccountDataGrid, accounts);

                    }
                    else if (currentTableAccount == "Total")
                    {
                        accounts = _unitOfWork.Account.GetAll().Where(a => a.Email.Substring(0, a.Email.IndexOf("@")).Contains(filter));
                        LoadFullData(AccountDataGrid, accounts);

                    }
                }
                catch
                {
                    MessageBox.Show("Filter error");
                }
            }
        }

        #endregion

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 0;
            createOrUpdateWindow.TypeOf = 0;
            createOrUpdateWindow.ShowDialog();
        }

        private void Btn_UpdateUserInAccountContent_Click(object sender, RoutedEventArgs e)
        {
            Button updateButton = sender as Button;

            var selectedAccount = updateButton.DataContext as Accounts;
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 0;
            createOrUpdateWindow.TypeOf = 1;
            createOrUpdateWindow.updateAccount = selectedAccount;
            createOrUpdateWindow.ShowDialog();

        }

        private async void Btn_DeleteUserInAccountContent_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;

            // Lấy đối tượng `UserAccount` liên quan đến dòng chứa nút "Delete" này
            var selectedAccount = deleteButton.DataContext as Accounts;
            if (selectedAccount != null)
            {
                var result = MessageBox.Show("Are you sure you want to delete this account?", "Delete Account", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _userManager.DeleteAsync(selectedAccount);
                    MessageBox.Show("Delete account successfully");
                }
            }

        }
    }
}
