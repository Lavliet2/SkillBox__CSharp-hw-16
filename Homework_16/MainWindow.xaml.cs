using Homework_16.Models;
using Homework_16.Services;
using Homework_16.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace Homework_16
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
        private void ClientsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var client = e.Row.Item as Client;
                var updateCommand = ((MainWindowViewModel)this.DataContext).UpdateClientCommand;
                if (updateCommand.CanExecute(client))
                {
                    updateCommand.Execute(client);
                }
            }
        }

        private void ProductsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

            if (e.EditAction == DataGridEditAction.Commit)
            {
                var product = e.Row.Item as Product;
                var updateCommand = ((MainWindowViewModel)this.DataContext).UpdateProductCommand;
                if (updateCommand.CanExecute(product))
                {
                    updateCommand.Execute(product);
                }
            }
        }
        private async void ShowProduct_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            var selectedClient = ClientsDataGrid.SelectedItem as Client;
            if (selectedClient != null)
            {
                var products = await viewModel.ProductService.GetProductsByEmailAsync(selectedClient.Email);

                // Создание нового окна
                var productsDialog = new MetroWindow
                {
                    Title = $"Товары клиента: {selectedClient.LastName} {selectedClient.FirstName[0]}. {selectedClient.MiddleName[0]}.",
                    Height = 300,
                    Width = 450,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                // Создание DataGrid и установка продуктов как источник данных
                DataGrid productsDataGrid = new DataGrid
                {
                    AutoGenerateColumns = false,
                    IsReadOnly = true,
                    Margin = new Thickness(10)
                };
                productsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Электронная почта", Binding = new Binding("Email") });
                productsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Код продукта", Binding = new Binding("ProductCode") });
                productsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Наименование", Binding = new Binding("ProductName") });
                productsDataGrid.ItemsSource = products;
                productsDialog.Content = productsDataGrid;
                productsDialog.ShowDialog();
            }
        }
        
        private async void AddClient_Click(object sender, RoutedEventArgs e)
        {
            var newClient = new Client
            {
                LastName = "Фамилия",
                FirstName = "Имя",
                MiddleName = "Отчество",
                PhoneNumber = string.Empty,
                Email = $"new_{new Random().Next(0,1000)}@example.com",
            };

            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel == null) return;

            int clientId = await viewModel.ClientService.AddClientAsync(newClient);
            newClient.ID = clientId;
            viewModel.Clients.Add(newClient);
        }

        private async void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
            Client selectedClient = ClientsDataGrid.SelectedItem as Client;

            if (selectedClient != null && viewModel != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить этого клиента?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    viewModel.Clients.Remove(selectedClient);
                    await viewModel.ClientService.DeleteClientAsync(selectedClient.ID);
                }
            }
        }

        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var newProduct = new Product
            {
                Email = $"new_{new Random().Next(0, 1000)}@example.com",
                ProductCode = 0,
                ProductName = "Новый продукт"
            };

            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel == null) return;

            int productId = await viewModel.ProductService.AddProductAsync(newProduct);
            newProduct.ID = productId;
            viewModel.Products.Add(newProduct);
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
            Product selectedProduct = ProductsDataGrid.SelectedItem as Product;

            if (selectedProduct != null && viewModel != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить этого клиента?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    viewModel.Products.Remove(selectedProduct);
                    await viewModel.ProductService.DeleteProductAsync(selectedProduct.ID);                    
                }
            }
        }

        private void ClientsDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var displayNameAttribute = typeof(Client).GetProperty(e.PropertyName)?.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
            if (displayNameAttribute != null)
            {
                e.Column.Header = displayNameAttribute.DisplayName;
            }
        }
    }
}
