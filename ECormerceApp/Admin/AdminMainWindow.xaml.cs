using DataAccess.Repository.IRepository;
using DataObject.Model;
using ECormerceApp.UserControls;
using LiveCharts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.VisualBasic.ApplicationServices;
using MimeKit;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private HubConnection _connection;

        //Pagination
        private PaginationHelper<Accounts> paginationHelperAccount;
        private PaginationHelper<Category> paginationHelperCategory;
        private PaginationHelper<Supplier> paginationHelperSupplier;
        private PaginationHelper<Product> paginationHelperProduct;
        private PaginationHelper<Order> paginationHelperOrder;
        private PaginationHelper<Ads> paginationHelperAds;

        public AdminMainWindow(IUnitOfWork unitOfWork, UserManager<Accounts> userManager)
        {
            InitializeComponent();
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            paginationHelperAccount = new PaginationHelper<Accounts>(7);
            paginationHelperCategory = new PaginationHelper<Category>(7);
            paginationHelperSupplier = new PaginationHelper<Supplier>(7);
            paginationHelperProduct = new PaginationHelper<Product>(7);
            paginationHelperOrder = new PaginationHelper<Order>(7);
            paginationHelperAds = new PaginationHelper<Ads>(7);

        }

        #region Method
        private void ShowOnlyStackPanel(StackPanel visiblePanel)
        {
            // Ẩn tất cả các StackPanel
            Welcome_Dashboard.Visibility = Visibility.Hidden;
            ProfileContent.Visibility = Visibility.Hidden;
            ChangePasswordContent.Visibility = Visibility.Hidden;
            AccountContent.Visibility = Visibility.Hidden;
            CategoryContent.Visibility = Visibility.Hidden;
            SupplierContent.Visibility = Visibility.Hidden;
            ProductContent.Visibility = Visibility.Hidden;
            AdvertiseContent.Visibility = Visibility.Hidden;
            OrderContent.Visibility = Visibility.Hidden;
            ChatContent.Visibility = Visibility.Hidden;

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
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    this.DragMove();
                }
            }
            catch
            {
                // Do nothing
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
                if (firtOrderDetail == null)
                {
                    continue;
                }
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

        #endregion

        #region Category

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(CategoryContent);
            var categories = _unitOfWork.Category.GetAll();
            LoadFullData<Category>(CategoryDataGrid, categories);
            TotalCategory.Text = "Total: " + categories.Count().ToString();

        }
        private void textBoxFilter_CategoryContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string filter = textBoxFilter_CategoryContent.Text;
                try
                {
                    var filteredCategories = _unitOfWork.Category.GetAll()
                    .Where(a => a.CategoryName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);

                    LoadFullData(CategoryDataGrid, filteredCategories);
                }
                catch
                {
                    MessageBox.Show("Filter error");
                }
            }
        }

        private void PreviousPageCategoryContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Category> category;
            category = _unitOfWork.Category.GetAll();
            paginationHelperCategory.PreviousPage();
            LoadDataForPage<Category>(CategoryDataGrid, category, paginationHelperCategory);
        }

        private void NextPageCategoryContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Category> category;
            category = _unitOfWork.Category.GetAll();
            paginationHelperCategory.NextPage(category);
            LoadDataForPage<Category>(CategoryDataGrid, category, paginationHelperCategory);
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 1;
            createOrUpdateWindow.TypeOf = 0;
            createOrUpdateWindow.ShowDialog();
        }

        private void Btn_UpdateCategoryInCategoryContent_Click(object sender, RoutedEventArgs e)
        {
            Button updateButton = sender as Button;
            var selectedCategory = updateButton.DataContext as Category;
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 1;
            createOrUpdateWindow.TypeOf = 1;
            createOrUpdateWindow.updateCategory = selectedCategory;
            createOrUpdateWindow.ShowDialog();
        }

        private void Btn_DeleteCategoryInCategoryContent_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;

            // Lấy đối tượng `UserAccount` liên quan đến dòng chứa nút "Delete" này
            var selectedCategory = deleteButton.DataContext as Category;
            if (selectedCategory != null)
            {
                var result = MessageBox.Show("Are you sure you want to delete this Category?", "Delete Category", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _unitOfWork.Category.Remove(selectedCategory);
                    _unitOfWork.Save();
                    MessageBox.Show("Delete Category successfully");
                }
            }
        }

        private void CategoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if the clicked item is a valid row
            if (sender is DataGrid grid)
            {
                var selectedRow = grid.SelectedItem as Category;
                if (selectedRow != null)
                {
                    var showProductOfCategory = App.ServiceProvider.GetRequiredService<ShowProductWindow>();
                    showProductOfCategory.OfContent = 0;
                    showProductOfCategory.CategoryID = selectedRow.CategoryID;
                    showProductOfCategory.ShowDialog();
                }

            }
        }
        #endregion

        #region Supplier
        private void Supplier_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(SupplierContent);
            var suppliers = _unitOfWork.Supplier.GetAll();
            LoadFullData<Supplier>(SupplierDataGrid, suppliers);
            TotalSupplier.Text = "Total: " + suppliers.Count().ToString();
        }

        private void textBoxFilter_SupplierContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string filter = textBoxFilter_SupplierContent.Text;
                try
                {
                    var filteredSuppliers = _unitOfWork.Supplier.GetAll()
                    .Where(a => a.CompanyName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);

                    LoadFullData(SupplierDataGrid, filteredSuppliers);
                }
                catch
                {
                    MessageBox.Show("Filter error");
                }
            }

        }

        private void PreviousPageSupplierContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Supplier> supplier;
            supplier = _unitOfWork.Supplier.GetAll();
            paginationHelperSupplier.PreviousPage();
            LoadDataForPage<Supplier>(SupplierDataGrid, supplier, paginationHelperSupplier);
        }

        private void NextPageSupplierContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Supplier> supplier;
            supplier = _unitOfWork.Supplier.GetAll();
            paginationHelperSupplier.NextPage(supplier);
            LoadDataForPage<Supplier>(SupplierDataGrid, supplier, paginationHelperSupplier);
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 2;
            createOrUpdateWindow.TypeOf = 0;
            createOrUpdateWindow.ShowDialog();
        }

        private void Btn_UpdateSupplierInSupplierContent_Click(object sender, RoutedEventArgs e)
        {
            Button updateButton = sender as Button;
            var selectedSupplier = updateButton.DataContext as Supplier;
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 2;
            createOrUpdateWindow.TypeOf = 1;
            createOrUpdateWindow.updateSupplier = selectedSupplier;
            createOrUpdateWindow.ShowDialog();

        }

        private void Btn_DeleteSupplierInSupplierContent_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;

            var selectedSupplier = deleteButton.DataContext as Supplier;
            if (selectedSupplier != null)
            {
                var result = MessageBox.Show("Are you sure you want to delete this Supplier?", "Delete Supplier", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _unitOfWork.Supplier.Remove(selectedSupplier);
                    _unitOfWork.Save();
                    MessageBox.Show("Delete Supplier successfully");
                }
            }

        }

        private void SupplierDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if the clicked item is a valid row
            if (sender is DataGrid grid)
            {
                var selectedRow = grid.SelectedItem as Supplier;
                if (selectedRow != null)
                {
                    var showProductOfSupplier = App.ServiceProvider.GetRequiredService<ShowProductWindow>();
                    showProductOfSupplier.OfContent = 1;
                    showProductOfSupplier.SupplierID = selectedRow.SupplierID;
                    showProductOfSupplier.ShowDialog();
                }
            }

        }


        #endregion

        #region Product

        private void Product_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(ProductContent);
            var products = _unitOfWork.Product.GetAll(includeProperty: "Supplier,Category");
            LoadFullData<Product>(ProductDataGrid, products);
            TotalProduct.Text = "Total: " + products.Count().ToString();
        }
    


        private void textBoxFilter_ProductContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string filter = textBoxFilter_ProductContent.Text;
                try
                {
                    var filteredProducts = _unitOfWork.Product.GetAll(includeProperty: "Supplier,Category")
                    .Where(a => a.ProductName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);

                    LoadFullData(ProductDataGrid, filteredProducts);
                }
                catch
                {
                    MessageBox.Show("Filter error");
                }
            }

        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 3;
            createOrUpdateWindow.TypeOf = 0;
            createOrUpdateWindow.ShowDialog();
        }


        private void Btn_UpdateProductInProductContent_Click(object sender, RoutedEventArgs e)
        {
            Button updateButton = sender as Button;
            var selectedProduct = updateButton.DataContext as Product;
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 3;
            createOrUpdateWindow.TypeOf = 1;
            var product = _unitOfWork.Product.GetFirstOrDefault(x => x.ProductID == selectedProduct.ProductID, includeProperties: "Supplier,Category");
            createOrUpdateWindow.updateProduct = product;
            createOrUpdateWindow.ShowDialog();

        }

        private void Btn_DeleteProductInSupplierContent_Click(object sender, RoutedEventArgs e)
        {

            Button deleteButton = sender as Button;

            var selectedProduct = deleteButton.DataContext as Product;
            if (selectedProduct != null)
            {
                var result = MessageBox.Show("Are you sure you want to delete this Product?", "Delete Product", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _unitOfWork.Product.Remove(selectedProduct);
                    _unitOfWork.Save();
                    MessageBox.Show("Delete Product successfully");
                }
            }
        }

        private void PreviousPageProductContentButton_Click(object sender, RoutedEventArgs e)
        {
            
            IEnumerable<Product> product;
            product = _unitOfWork.Product.GetAll(includeProperty: "Supplier,Category");
            paginationHelperProduct.PreviousPage();
            LoadDataForPage<Product>(ProductDataGrid, product, paginationHelperProduct);

        }

        private void NextPageProductContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Product> product;
            product = _unitOfWork.Product.GetAll(includeProperty: "Supplier,Category");
            paginationHelperProduct.NextPage(product);
            LoadDataForPage<Product>(ProductDataGrid, product, paginationHelperProduct);

        }

        #endregion

        #region Ads

        private void Advertise_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(AdvertiseContent);
            var ads = _unitOfWork.Ads.GetAll();
            LoadFullData<Ads>(AdvertiseDataGrid, ads);
            TotalAdvertise.Text = "Total: " + ads.Count().ToString();
            int totalAvailable = ads.Where(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now).Count();
            if (totalAvailable == 0)
            {
                TotalAdvertiseAvailable.Text = "Total Available: 0";
            }
            else
            {
                TotalAdvertiseAvailable.Text = "Advertise Running: " + totalAvailable.ToString();
            }
        }

        private void PreviousPageAdvertiseContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Ads> ads;
            ads = _unitOfWork.Ads.GetAll();
            paginationHelperAds.PreviousPage();
            LoadDataForPage<Ads>(AdvertiseDataGrid, ads, paginationHelperAds);

        }

        private void NextPageAdvertiseContentButton_Click(object sender, RoutedEventArgs e)
        {

            IEnumerable<Ads> ads;
            ads = _unitOfWork.Ads.GetAll();
            paginationHelperAds.NextPage(ads);
            LoadDataForPage<Ads>(AdvertiseDataGrid, ads, paginationHelperAds);
        }

        private void AddAdvertise_Click(object sender, RoutedEventArgs e)
        {
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 4;
            createOrUpdateWindow.TypeOf = 0;
            createOrUpdateWindow.ShowDialog();

        }

        private void Btn_UpdateAdvertiseContent_Click(object sender, RoutedEventArgs e)
        {
            Button updateButton = sender as Button;
            var selectedAdvertise = updateButton.DataContext as Ads;
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 4;
            createOrUpdateWindow.TypeOf = 1;
            createOrUpdateWindow.updateAdvertise = selectedAdvertise;
            createOrUpdateWindow.ShowDialog();

        }

        private void Btn_DeleteAdvertiseContent_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            var selectedAdvertise = deleteButton.DataContext as Ads;
            if (selectedAdvertise != null)
            {
                var result = MessageBox.Show("Are you sure you want to delete this Advertise?", "Delete Advertise", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _unitOfWork.Ads.Remove(selectedAdvertise);
                    _unitOfWork.Save();
                    MessageBox.Show("Delete Advertise successfully");
                }
            }

        }

        private void Filter_Advertise_Click(object sender, RoutedEventArgs e)
        {
            string title = textBoxTitle_AdvertiseContent.Text;
            DateTime startDate = datePickerStartDate_AdvertiseContent.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = datePickerEndDate_AdvertiseContent.SelectedDate ?? DateTime.MaxValue; // Use MaxValue for endDate for proper filtering

            // Validate if start date is before end date
            if (startDate > endDate)
            {
                MessageBox.Show("Start date must be before end date");
            }
            else
            {
                var ads = _unitOfWork.Ads.GetAll();

                // Filter by title if provided
                if (!string.IsNullOrWhiteSpace(title))
                {
                    ads = ads.Where(a => a.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                // Filter by start date (only if a valid date is selected)
                if (startDate != DateTime.MinValue)
                {
                    ads = ads.Where(x => x.StartDate >= startDate);
                }

                // Filter by end date (only if a valid date is selected)
                if (endDate != DateTime.MaxValue)
                {
                    ads = ads.Where(x => x.EndDate <= endDate);
                }

                // Load the filtered data into the grid
                LoadFullData<Ads>(AdvertiseDataGrid, ads);
            }


        }

        private void Total_Available_Ads_Click(object sender, RoutedEventArgs e)
        {
            var ads = _unitOfWork.Ads.GetAll();
            var totalAvailable = ads.Where(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now).ToList();
            LoadFullData<Ads>(AdvertiseDataGrid, totalAvailable);
        }
        #endregion

        #region Order

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(OrderContent);
            var orders = _unitOfWork.Order.GetAll(includeProperty: "Customer,Accounts");
            LoadFullData<Order>(OrderDataGrid, orders);
            TotalOrder.Text = "Total: " + orders.Count().ToString();
            TotalOrderShipped.Text = "Total Shipped: " + orders.Where(x => x.ShippedDate != null).Count().ToString();

        }

        private void Total_Shipped_Order_Click(object sender, RoutedEventArgs e)
        {
            var orders = _unitOfWork.Order.GetAll(includeProperty: "Customer,Accounts");
            var totalAvailable = orders.Where(x => x.ShippedDate != null).ToList();
            LoadFullData<Order>(OrderDataGrid, totalAvailable);

        }

        private void Filter_Order_Click(object sender, RoutedEventArgs e)
        {
            int orderId = textBoxId_OrderContent.Text == "" ? 0 : int.Parse(textBoxId_OrderContent.Text);
            DateTime startDate = datePickerStartDate_OrderContent.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = datePickerEndDate_OrderContent.SelectedDate ?? DateTime.MaxValue; // Use MaxValue for endDate for proper filtering
            string account = textBoxAccount_OrderContent.Text;

            var orders = _unitOfWork.Order.GetAll(includeProperty: "Customer,Accounts");
            if (orderId != 0)
            {
                orders = orders.Where(x => x.OrderID == orderId);
            }
            if (startDate != DateTime.MinValue)
            {
                orders = orders.Where(x => x.OrderDate >= startDate);
            }

            if (endDate != DateTime.MaxValue)
            {
                orders = orders.Where(x => x.OrderDate <= endDate);
            }
            if (!string.IsNullOrWhiteSpace(account))
            {
                orders = orders.Where(x => x.Accounts.UserName.IndexOf(account, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            LoadFullData<Order>(OrderDataGrid, orders);

        }


        private void Btn_UpdateOrderContent_Click(object sender, RoutedEventArgs e)
        {
            Button updateButton = sender as Button;
            var selectedOrder = updateButton.DataContext as Order;
            var createOrUpdateWindow = App.ServiceProvider.GetRequiredService<CreateOrUpdateWindow>();
            createOrUpdateWindow.TypeOfWindow = 5;
            createOrUpdateWindow.TypeOf = 1;
            var order = _unitOfWork.Order.GetFirstOrDefault(x => x.OrderID == selectedOrder.OrderID, includeProperties: "Customer,Accounts");
            createOrUpdateWindow.updateOrder = order;
            createOrUpdateWindow.ShowDialog();

        }

        private void Btn_DeleteOrderContent_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            var selectedOrder = deleteButton.DataContext as Order;
            if (selectedOrder != null)
            {
                var result = MessageBox.Show("Are you sure you want to delete this Order?", "Delete Order", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _unitOfWork.Order.Remove(selectedOrder);
                    _unitOfWork.Save();
                    MessageBox.Show("Delete Order successfully");
                }
            }

        }

        private void PreviousPageOrderContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Order> orders;
            orders = _unitOfWork.Order.GetAll(includeProperty: "Customer,Accounts");
            paginationHelperOrder.PreviousPage();
            LoadDataForPage<Order>(OrderDataGrid, orders, paginationHelperOrder);

        }

        private void NextPageOrderContentButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Order> orders;
            orders = _unitOfWork.Order.GetAll(includeProperty: "Customer,Accounts");
            paginationHelperOrder.NextPage(orders);
            LoadDataForPage<Order>(OrderDataGrid, orders, paginationHelperOrder);

        }

        private void OrderDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if the clicked item is a valid row
            if (sender is DataGrid grid)
            {
                var selectedRow = grid.SelectedItem as Order;
                if (selectedRow != null)
                {
                    var showProductOfOrder = App.ServiceProvider.GetRequiredService<ShowProductWindow>();
                    showProductOfOrder.OfContent = 2;
                    showProductOfOrder.OrderID = selectedRow.OrderID;
                    showProductOfOrder.ShowDialog();
                }
            }

        }

        #endregion

        #region Chat
        private string _adminId;
        private string _customerId;
        private async void Chat_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(ChatContent);
            // Lấy đường dẫn từ appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            string rootPath = config["SignalR:HubUrl"];

            _connection = new HubConnectionBuilder()
               .WithUrl(rootPath) // Update the URL with your Razor Page hub URL
               .Build();

            try
            {
                // Sử dụng async/await để chờ kết nối tới hub
                await _connection.StartAsync();

                // Sau khi kết nối thành công, gọi RegisterUserWWPF với adminId
                Dispatcher.Invoke(() =>
                {
                    ConnectingMessage.Text = "Connected to server";
                });
                var adminId = loggedInUser.Id;
                await _connection.InvokeAsync("RegisterUserWWPF", adminId);
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi xảy ra khi kết nối hoặc đăng ký user
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Failed to connect or register user: " + ex.Message);
                });
            }

            // Xử lý sự kiện khi admin được phân công cho khách hàng
            _connection.On<string, string, string>("NewCustomerAssigned", (admin, customer, emailUser) =>
            {
                Dispatcher.Invoke(() =>
                {
                    _adminId = admin;
                    _customerId = customer;
                    ConnectingMessage.Text = $"Connected with customer {emailUser}";
                });
            });

            // Xử lý tin nhắn từ khách hàng
            _connection.On<string, string>("ReceiveMessageFromCustomer", (customerId, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessageChat messChat = new MessageChat
                    {
                        Message = message,
                        Color = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#68CFA3"),
                    };

                    MessagesPanel.Height = MessagesPanel.Height + 500;
                    // Add the new instance to the StackPanel (MessagesPanel)
                    MessagesPanel.Children.Add(messChat);
                });
            });

        }

        // Gửi tin nhắn từ admin tới khách hàng
        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            var message = MessageInput.Text;

            if (!string.IsNullOrEmpty(message) && _customerId != null)
            {
                MyMessageChat messChat = new MyMessageChat
                {
                    Message = message
                };
                // Add the new instance to the StackPanel (MessagesPanel)
                MessagesPanel.Children.Add(messChat);
                MessagesPanel.Height = MessagesPanel.Height + 500;

                // Gửi tin nhắn tới khách hàng
                await _connection.InvokeAsync("SendMessageToCustomer", _customerId, message);

                MessageInput.Clear();
            }
        }

        #endregion

        private void Offline_Click(object sender, RoutedEventArgs e)
        {
            ShowOnlyStackPanel(Welcome_Dashboard);
            if (_connection != null)
            {
                _connection.StopAsync();
            }
            MessagesPanel.Children.Clear();

        }
    }
}
