using Homework_16.Models;
using Homework_16.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Homework_16.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ClientService  _clientService;
        private readonly ProductService _productService;
        public ClientService ClientService   => _clientService;
        public ProductService ProductService => _productService;

        public ObservableCollection<Client> Clients { get; } = new ObservableCollection<Client>();
        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();

        public ICommand UpdateClientCommand { get; private set; }
        public ICommand UpdateProductCommand { get; private set; }

        private List<string> _clientsEmails = new List<string>();
        public List<string> ClientsEmails
        {
            get
            {
                _clientsEmails.Clear();
                foreach (var client in Clients)
                {
                    _clientsEmails.Add(client.Email);
                }
                return _clientsEmails;
            }
            private set { }
        }

        private string _statusBarAccessContent;
        private string _statusBarSQLContent;

        public MainWindowViewModel()
        {
            _clientService = new ClientService("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ClisntsStorage;Integrated Security=True;Pooling=False");
            _productService = new ProductService("Provider=Microsoft.ACE.OLEDB.16.0;Data Source=S:\\Dev\\C#\\SkillBox\\HomeWork\\Theme_16\\Homework_16\\DB\\ProductsStorage.accdb");
            UpdateClientCommand = new RelayCommand<Client>(async (client) =>
            {
                await _clientService.UpdateClientAsync(client);
            });

            UpdateProductCommand = new RelayCommand<Product>(async (product) =>
            {
                await _productService.UpdateProductAsync(product);
            });
            LoadDataAsync();
        }


        private async void LoadDataAsync()
        {
            var clients = await _clientService.GetAllClientsAsync();
            foreach (var client in clients)
            {
                Clients.Add(client);
            }
            StatusBarSQLContent = $"Подключение к базе: Clients - Успешно!";

            var products = await _productService.GetAllProductsAsync();
            foreach (var product in products)
            {
                Products.Add(product);
            }
            StatusBarAccessContent = $"Подключение к базе: Products - Успешно!";
        }

        public string StatusBarAccessContent
        {
            get => _statusBarAccessContent;
            set
            {
                if (_statusBarAccessContent != value)
                {
                    _statusBarAccessContent = value;
                    OnPropertyChanged(nameof(StatusBarAccessContent));
                }
            }
        }

        public string StatusBarSQLContent
        {
            get => _statusBarSQLContent;
            set
            {
                if (_statusBarSQLContent != value)
                {
                    _statusBarSQLContent = value;
                    OnPropertyChanged(nameof(StatusBarSQLContent));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }     
    }

    public class RelayCommand<T> : ICommand
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}