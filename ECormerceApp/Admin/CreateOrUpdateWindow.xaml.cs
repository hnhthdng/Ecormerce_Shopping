using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Interaction logic for CreateOrUpdateWindow.xaml
    /// </summary>
    public partial class CreateOrUpdateWindow : Window
    {
        private enum TypeWindow
        {
            Account,
            Category,
            Supplier,
            Product,
            Ads,
            Order,
        }
        private enum Type
        {
            Create,
            Update
        }
        public int TypeOfWindow;
        public int TypeOf;

        public Accounts updateAccount;
        public Category updateCategory;
        public Supplier updateSupplier;
        public Product updateProduct;
        public Ads updateAdvertise;
        public Order updateOrder;
        private readonly UserManager<Accounts> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrUpdateWindow(UserManager<Accounts> userManager, IUnitOfWork unitOfWork)
        {
            InitializeComponent();
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        #region Method
        private void ShowOnlyStackPanel(StackPanel stackPanel)
        {
            //Account
            CreateOrUpdateAccount.Visibility = Visibility.Collapsed;
            //Category
            CreateOrUpdateCategory.Visibility = Visibility.Collapsed;
            //Supplier
            CreateOrUpdateSupplier.Visibility = Visibility.Collapsed;
            //Product
            CreateOrUpdateProduct.Visibility = Visibility.Collapsed;
            //Ads
            CreateOrUpdateAds.Visibility = Visibility.Collapsed;
            //Order
            CreateOrUpdateOrder.Visibility = Visibility.Collapsed;

            stackPanel.Visibility = Visibility.Visible;
        }

        private string UploadOrUpdateImage(string oldImageURL, string newImageURL)
        {
            // Lấy đường dẫn từ appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            string rootPath = config["RootPath:Path"];

            // Thư mục lưu trữ hình ảnh
            string imageFolderPath = System.IO.Path.Combine(rootPath, @"images\products");

            // Nếu có đường dẫn hình ảnh cũ, tiến hành xóa
            if (!string.IsNullOrEmpty(oldImageURL))
            {
                string oldImagePath = System.IO.Path.Combine(rootPath, oldImageURL.TrimStart('\\'));
                if (File.Exists(oldImagePath))
                {
                    try
                    {
                        File.Delete(oldImagePath);
                        MessageBox.Show("Old image deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting old image: " + ex.Message);
                    }
                }
            }

            // Upload hình ảnh mới
            string extension = System.IO.Path.GetExtension(newImageURL);
            string fileName = Guid.NewGuid().ToString();
            string newFilePath = System.IO.Path.Combine(imageFolderPath, fileName + extension);

            try
            {
                // Copy file mới vào thư mục lưu trữ
                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    using (var sourceStream = new FileStream(newImageURL, FileMode.Open))
                    {
                        sourceStream.CopyTo(fileStream);
                    }
                }

                // Cập nhật đường dẫn ProductImageURL mới
                string productImageURL = @"\images\products\" + fileName + extension;

                MessageBox.Show("Image uploaded successfully. URL: " + productImageURL);
                return productImageURL;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

            return null;
        }

        #endregion
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(TypeOf == (int)Type.Create)
            {
                txtBanner.Text = "CREATE";
                switch (TypeOfWindow)
                {
                    case (int)TypeWindow.Account:
                        TitleOfCreateOrUpdate.Text = "ACCOUNT";
                        ShowOnlyStackPanel(CreateOrUpdateAccount);
                        break;
                    case (int)TypeWindow.Category:
                        TitleOfCreateOrUpdate.Text = "CATEGORY";
                        ShowOnlyStackPanel(CreateOrUpdateCategory);
                        break;
                    case (int)TypeWindow.Supplier:
                        TitleOfCreateOrUpdate.Text = "SUPPLIER";
                        ShowOnlyStackPanel(CreateOrUpdateSupplier);
                        break;
                    case (int)TypeWindow.Product:
                        this.Height = 700;
                        btnSave.Margin = new Thickness(0, 600, 0, 0);
                        TitleOfCreateOrUpdate.Text = "PRODUCT";
                        cbbCategoryInProductContent.ItemsSource = _unitOfWork.Category.GetAll().Select(x => x.CategoryName).ToList();
                        cbbSupplierInProductContent.ItemsSource = _unitOfWork.Supplier.GetAll().Select(x => x.CompanyName).ToList();
                        ShowOnlyStackPanel(CreateOrUpdateProduct);
                        break;
                    case (int)TypeWindow.Ads:
                        TitleOfCreateOrUpdate.Text = "ADS";
                        ShowOnlyStackPanel(CreateOrUpdateAds);
                        break;
                }

            }
            else if (TypeOf == (int)Type.Update)
            {
                txtBanner.Text = "UPDATE";
                switch (TypeOfWindow)
                {
                    case (int)TypeWindow.Account:
                        TitleOfCreateOrUpdate.Text = "ACCOUNT";
                        ShowOnlyStackPanel(CreateOrUpdateAccount);
                        txtEmailInAccountContent.Text = updateAccount.Email;
                        txtFullNameInAccountContent.Text = updateAccount.FullName;
                        txtPhoneInAccountContent.Text = updateAccount.PhoneNumber;
                        if (updateAccount.Type == 1)
                        {
                            rdoStaffInAccountContent.IsChecked = true;
                        }
                        else
                        {
                            rdoNormalUserInAccountContent.IsChecked = true;
                        }
                        txtBlockPassConfirmInAccountContent.Visibility = Visibility.Collapsed;
                        txtBlockPassInAccountContent.Visibility = Visibility.Collapsed;
                        txtPassInAccountContent.Visibility = Visibility.Collapsed;
                        txtConfirmPassInAccountContent.Visibility = Visibility.Collapsed;
                        break;
                    case (int)TypeWindow.Category:
                        TitleOfCreateOrUpdate.Text = "CATEGORY";
                        ShowOnlyStackPanel(CreateOrUpdateCategory);
                        txtNameInCategoryContent.Text = updateCategory.CategoryName;
                        txtDescriptionInCategoryContent.Text = updateCategory.Description;
                        break;
                    case (int)TypeWindow.Supplier:
                        TitleOfCreateOrUpdate.Text = "SUPPLIER";
                        ShowOnlyStackPanel(CreateOrUpdateSupplier);
                        txtCompanyNameInSupplierContent.Text = updateSupplier.CompanyName;
                        txtAddressInSupplierContent.Text = updateSupplier.Address;
                        txtPhoneInSupplierContent.Text = updateSupplier.Phone;
                        break;
                    case (int)TypeWindow.Product:
                        this.Height = 700;
                        btnSave.Margin = new Thickness(0, 600, 0, 0);
                        TitleOfCreateOrUpdate.Text = "PRODUCT";
                        cbbCategoryInProductContent.ItemsSource = _unitOfWork.Category.GetAll().Select(x => x.CategoryName).ToList();
                        cbbSupplierInProductContent.ItemsSource = _unitOfWork.Supplier.GetAll().Select(x => x.CompanyName).ToList();
                        ShowOnlyStackPanel(CreateOrUpdateProduct);
                        txtProductNameInProductContent.Text = updateProduct.ProductName;
                        txtQuantityPerUnitInProductContent.Text = updateProduct.QuantityPerUnit;
                        txtUnitPriceInProductContent.Text = updateProduct.UnitPrice.ToString();
                        txtUnitsInStockInProductContent.Text = updateProduct.UnitsInStock.ToString();
                        txImageURLInProductContent.Text = updateProduct.ProductImageURL;
                        cbbCategoryInProductContent.SelectedValue = updateProduct.Category.CategoryName;
                        cbbSupplierInProductContent.SelectedValue = updateProduct.Supplier.CompanyName;
                        break;
                    case (int)TypeWindow.Ads:
                        TitleOfCreateOrUpdate.Text = "ADS";
                        ShowOnlyStackPanel(CreateOrUpdateAds);
                        txtTitleInAdsContent.Text = updateAdvertise.Title;
                        txtContentInAdsContent.Text = updateAdvertise.Content;
                        dpStartDateInAdsContent.SelectedDate = updateAdvertise.StartDate;
                        dpEndDateInAdsContent.SelectedDate = updateAdvertise.EndDate;
                        break;
                    case (int)TypeWindow.Order:
                        this.Height = 750;
                        btnSave.Margin = new Thickness(0, 625, 0, 0);
                        TitleOfCreateOrUpdate.Text = "ORDER";
                        ShowOnlyStackPanel(CreateOrUpdateOrder);
                        txtEmailInOrderContent.Text = updateOrder.Accounts.Email;
                        txtContactInOrderContent.Text = updateOrder.Customer.ContactName;
                        dpOrderDateInOrderContent.Text = updateOrder.OrderDate.ToString();
                        dpRequiredDateInOrderContent.Text = updateOrder.RequiredDate.ToString();
                        dpShippedDateInOrderContent.Text = updateOrder.ShippedDate.ToString();
                        txtFreightInOrderContent.Text = updateOrder.Freight.ToString();
                        txtAddressInOrderContent.Text = updateOrder.ShipAddress;
                        break;


                }

            }

            
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (TypeOf == (int)Type.Create)
            {
                switch (TypeOfWindow)
                {
                    case (int)TypeWindow.Account:
                        string email = txtEmailInAccountContent.Text;
                        string fullName = txtFullNameInAccountContent.Text;
                        string phone = txtPhoneInAccountContent.Text;
                        string pass = txtPassInAccountContent.Text;
                        string confirmPass = txtConfirmPassInAccountContent.Text;
                        bool isNormalUser = rdoNormalUserInAccountContent.IsChecked == true;
                        bool isStaff = rdoStaffInAccountContent.IsChecked == true;

                        //Check null
                        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(confirmPass))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        if (pass != confirmPass)
                        {
                            MessageBox.Show("Password and Confirm Password must be the same");
                            return;
                        }
                        if (isNormalUser == false && isStaff == false)
                        {
                            MessageBox.Show("Please choose type of account");
                            return;
                        }

                        var user = new Accounts
                        {
                            Email = email,
                            UserName = email,
                            FullName = fullName,
                            PhoneNumber = phone,
                            Type = isNormalUser ? 2 : 1
                        };
                        var result = await _userManager.CreateAsync(user, pass);
                        if (result.Succeeded)
                        {
                            if (user.Type == 1)
                            {
                                await _userManager.AddToRoleAsync(user, "Staff");

                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, "NormalUser");
                            }
                        }
                        MessageBox.Show("Create account successfully");

                        foreach (var error in result.Errors)
                        {
                            MessageBox.Show(error.Description);
                        }
                        break;
                    case (int)TypeWindow.Category:
                        string name = txtNameInCategoryContent.Text;
                        string description = txtDescriptionInCategoryContent.Text;
                        //Check null
                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        bool isNameExist = _unitOfWork.Category.GetAll().Any(x => x.CategoryName == name);
                        if (isNameExist)
                        {
                            MessageBox.Show("Category name is already exist");
                            return;
                        }
                        var category = new Category
                        {
                            CategoryName = name,
                            Description = description
                        };
                       _unitOfWork.Category.Add(category);
                        _unitOfWork.Save();
                        MessageBox.Show("Create category successfully");
                        break;
                    case (int)TypeWindow.Supplier:
                        string companyName = txtCompanyNameInSupplierContent.Text;
                        string address = txtAddressInSupplierContent.Text;
                        string phoneSupplier = txtPhoneInSupplierContent.Text;
                        //Check null
                        if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phoneSupplier))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        var supplier = new Supplier
                        {
                            CompanyName = companyName,
                            Address = address,
                            Phone = phoneSupplier
                        };
                        _unitOfWork.Supplier.Add(supplier);
                        _unitOfWork.Save();
                        MessageBox.Show("Create supplier successfully");
                        break;
                    case (int)TypeWindow.Product:
                        string productName = txtProductNameInProductContent.Text;
                        string quantityPerUnit = txtQuantityPerUnitInProductContent.Text;
                        string unitPrice = txtUnitPriceInProductContent.Text;
                        string unitsInStock = txtUnitsInStockInProductContent.Text;
                        string imageURL = txImageURLInProductContent.Text;
                        if (cbbCategoryInProductContent.SelectedItem == null || cbbSupplierInProductContent.SelectedItem == null)
                        {
                            MessageBox.Show("Please choose category and supplier");
                            return;
                        }
                        string categoryAdd = cbbCategoryInProductContent.SelectedItem.ToString();
                        string supplierAdd = cbbSupplierInProductContent.SelectedItem.ToString();
                        //Check null
                        if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(quantityPerUnit) || string.IsNullOrEmpty(unitPrice) 
                            || string.IsNullOrEmpty(unitsInStock) || string.IsNullOrEmpty(imageURL) || string.IsNullOrEmpty(categoryAdd) || string.IsNullOrEmpty(supplierAdd))

                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }

                        var product = new Product
                        {
                            ProductName = productName,
                            QuantityPerUnit = quantityPerUnit,
                            UnitPrice = decimal.Parse(unitPrice),
                            UnitsInStock = int.Parse(unitsInStock),
                            ProductImageURL = UploadOrUpdateImage(null,imageURL),
                            CategoryID = _unitOfWork.Category.GetAll().FirstOrDefault(x => x.CategoryName == categoryAdd).CategoryID,
                            SupplierID = _unitOfWork.Supplier.GetAll().FirstOrDefault(x => x.CompanyName == supplierAdd).SupplierID
                        };
                        _unitOfWork.Product.Add(product);
                        _unitOfWork.Save();
                        MessageBox.Show("Create product successfully");
                        break;
                    case (int)TypeWindow.Ads:
                        string title = txtTitleInAdsContent.Text;
                        string content = txtContentInAdsContent.Text;
                        DateTime startDate = dpStartDateInAdsContent.SelectedDate ?? DateTime.MinValue;
                        DateTime endDate = dpEndDateInAdsContent.SelectedDate ?? DateTime.MaxValue; // Use MaxValue for endDate for proper filtering
                        //Check null
                        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        if (startDate == DateTime.MinValue || endDate == DateTime.MaxValue)
                        {
                            MessageBox.Show("Please choose start date and end date");
                            return;
                        }
                        if (startDate > endDate)
                        {
                            MessageBox.Show("Start date must be less than end date");
                            return;
                        }
                        var ads = new Ads
                        {
                            Title = title,
                            Content = content,
                            StartDate = startDate,
                            EndDate = endDate
                        };
                        _unitOfWork.Ads.Add(ads);
                        _unitOfWork.Save();
                        MessageBox.Show("Create ads successfully");
                        break;

                }

            }
            else if (TypeOf == (int)Type.Update)
            {
                switch (TypeOfWindow)
                {
                    case (int)TypeWindow.Account:
                        string email = txtEmailInAccountContent.Text;
                        string fullName = txtFullNameInAccountContent.Text;
                        string phone = txtPhoneInAccountContent.Text;
                        bool isNormalUser = rdoNormalUserInAccountContent.IsChecked == true;
                        bool isStaff = rdoStaffInAccountContent.IsChecked == true;
                        //Check null
                        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        if (isNormalUser == false && isStaff == false)
                        {
                            MessageBox.Show("Please choose type of account");
                            return;
                        }
                        updateAccount.Email = email;
                        updateAccount.FullName = fullName;
                        updateAccount.PhoneNumber = phone;
                        updateAccount.Type = isNormalUser ? 2 : 1;
                        var result = await _userManager.UpdateAsync(updateAccount);
                        if (result.Succeeded)
                        {
                            if (updateAccount.Type == 1)
                            {
                                await _userManager.RemoveFromRoleAsync(updateAccount, "NormalUser");
                                await _userManager.AddToRoleAsync(updateAccount, "Staff");

                            }
                            else
                            {
                                await _userManager.RemoveFromRoleAsync(updateAccount, "Staff");
                                await _userManager.AddToRoleAsync(updateAccount, "NormalUser");
                            }
                        }
                        MessageBox.Show("Update account successfully");
                        break;
                    case (int)TypeWindow.Category:
                        string name = txtNameInCategoryContent.Text;
                        string description = txtDescriptionInCategoryContent.Text;
                        //Check null
                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        bool isNameExist = _unitOfWork.Category.GetAll().Any(x => x.CategoryName == name && x.CategoryID != updateCategory.CategoryID);
                        if (isNameExist)
                        {
                            MessageBox.Show("Category name is already exist");
                            return;
                        }
                        updateCategory.CategoryName = name;
                        updateCategory.Description = description;
                        _unitOfWork.Category.Update(updateCategory);
                        _unitOfWork.Save();
                        MessageBox.Show("Update category successfully");
                        break;
                    case (int)TypeWindow.Supplier:
                        string companyName = txtCompanyNameInSupplierContent.Text;
                        string address = txtAddressInSupplierContent.Text;
                        string phoneSupplier = txtPhoneInSupplierContent.Text;
                        //Check null
                        if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phoneSupplier))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        updateSupplier.CompanyName = companyName;
                        updateSupplier.Address = address;
                        updateSupplier.Phone = phoneSupplier;
                        _unitOfWork.Supplier.Update(updateSupplier);
                        _unitOfWork.Save();
                        MessageBox.Show("Update supplier successfully");
                        break;
                    case (int)TypeWindow.Product:
                        string oldImageURL = updateProduct.ProductImageURL;
                        string productName = txtProductNameInProductContent.Text;
                        string quantityPerUnit = txtQuantityPerUnitInProductContent.Text;
                        string unitPrice = txtUnitPriceInProductContent.Text;
                        string unitsInStock = txtUnitsInStockInProductContent.Text;
                        string imageURL = txImageURLInProductContent.Text;
                        if (cbbCategoryInProductContent.SelectedItem == null || cbbSupplierInProductContent.SelectedItem == null)
                        {
                            MessageBox.Show("Please choose category and supplier");
                            return;
                        }
                        string categoryUpdate = cbbCategoryInProductContent.SelectedItem.ToString();
                        string supplierUpdate = cbbSupplierInProductContent.SelectedItem.ToString();

                        //Check null
                        if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(quantityPerUnit) || string.IsNullOrEmpty(unitPrice) 
                            || string.IsNullOrEmpty(unitsInStock) || string.IsNullOrEmpty(imageURL) || string.IsNullOrEmpty(categoryUpdate) || string.IsNullOrEmpty(supplierUpdate))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        if (oldImageURL == imageURL)
                        {
                            MessageBox.Show("Please choose new image");
                            return;
                        }
                        updateProduct.ProductName = productName;
                        updateProduct.QuantityPerUnit = quantityPerUnit;
                        updateProduct.UnitPrice = decimal.Parse(unitPrice);
                        updateProduct.UnitsInStock = int.Parse(unitsInStock);
                        updateProduct.ProductImageURL = UploadOrUpdateImage(oldImageURL, imageURL);
                        updateProduct.CategoryID = _unitOfWork.Category.GetAll().FirstOrDefault(x => x.CategoryName == categoryUpdate).CategoryID;
                        updateProduct.SupplierID = _unitOfWork.Supplier.GetAll().FirstOrDefault(x => x.CompanyName == supplierUpdate).SupplierID;
                        _unitOfWork.Product.Update(updateProduct);
                        _unitOfWork.Save();
                        MessageBox.Show("Update product successfully");
                        break;
                    case (int)TypeWindow.Ads:
                        string title = txtTitleInAdsContent.Text;
                        string content = txtContentInAdsContent.Text;
                        DateTime startDate = dpStartDateInAdsContent.SelectedDate ?? DateTime.MinValue;
                        DateTime endDate = dpEndDateInAdsContent.SelectedDate ?? DateTime.MaxValue; // Use MaxValue for endDate for proper filtering
                        //Check null
                        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        if (startDate == DateTime.MinValue || endDate == DateTime.MaxValue)
                        {
                            MessageBox.Show("Please choose start date and end date");
                            return;
                        }
                        if (startDate > endDate)
                        {
                            MessageBox.Show("Start date must be less than end date");
                            return;
                        }
                        updateAdvertise.Title = title;
                        updateAdvertise.Content = content;
                        updateAdvertise.StartDate = startDate;
                        updateAdvertise.EndDate = endDate;
                        _unitOfWork.Ads.Update(updateAdvertise);
                        _unitOfWork.Save();
                        MessageBox.Show("Update ads successfully");
                        break;
                    case (int)TypeWindow.Order:
                        string emailOrder = txtEmailInOrderContent.Text;
                        string contact = txtContactInOrderContent.Text;
                        DateTime orderDate = DateTime.Parse(dpOrderDateInOrderContent.Text);
                        DateTime requiredDate = DateTime.Parse(dpRequiredDateInOrderContent.Text);
                        DateTime shippedDate = DateTime.Parse(dpShippedDateInOrderContent.Text);
                        string freight = txtFreightInOrderContent.Text;
                        string addressOrder = txtAddressInOrderContent.Text;
                        //Check null
                        if (string.IsNullOrEmpty(emailOrder) || string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(freight) || string.IsNullOrEmpty(addressOrder))
                        {
                            MessageBox.Show("Please fill all fields");
                            return;
                        }
                        if(orderDate > requiredDate || orderDate > shippedDate || requiredDate > shippedDate)
                        {
                            MessageBox.Show("Order date must be less than required date and shipped date");
                            return;
                        }
                        updateOrder.Accounts.Email = emailOrder;
                        updateOrder.Customer.ContactName = contact;
                        updateOrder.OrderDate = orderDate;
                        updateOrder.RequiredDate = requiredDate;
                        updateOrder.ShippedDate = shippedDate;
                        updateOrder.Freight = decimal.Parse(freight);
                        updateOrder.ShipAddress = addressOrder;
                        _unitOfWork.Order.Update(updateOrder);
                        _unitOfWork.Save();
                        MessageBox.Show("Update order successfully");
                        break;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnChooseImageInProductContent_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                txImageURLInProductContent.Text = filename;
                
            }
        }
    }
}
