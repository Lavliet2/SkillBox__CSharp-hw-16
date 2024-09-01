using Homework_16.DataAccess;
using Homework_16.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Homework_16.DataAccess
{
    public class AccessRepository<T> : IRepository<T> where T : Product, new()
    {
        private readonly string _connectionString;

        public AccessRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddAsync(T entity)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                await connection.OpenAsync();

                var insertQuery = $"INSERT INTO {typeof(T).Name}s (Email, ProductCode, ProductName) VALUES (@Email, @ProductCode, @ProductName)";
                var command = new OleDbCommand(insertQuery, connection);

                if (typeof(T) == typeof(Product))
                {
                    var product = entity as Product;
                    command.Parameters.AddWithValue("@Email", product.Email);
                    command.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                }

                await command.ExecuteNonQueryAsync();

                command = new OleDbCommand("SELECT @@IDENTITY;", connection);
                var result = await command.ExecuteScalarAsync();
                return (result != DBNull.Value && result is int) ? Convert.ToInt32(result) : 0;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new OleDbCommand($"DELETE FROM {typeof(T).Name}s WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var products = new List<T>();

            using (var connection = new OleDbConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new OleDbCommand("SELECT * FROM Products", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var product = new T
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("ID")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            ProductCode = reader.GetInt32(reader.GetOrdinal("ProductCode")),
                            ProductName = reader.GetString(reader.GetOrdinal("ProductName"))
                        };
                        products.Add(product);
                    }
                }
            }

            return products;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            T entity = new T();

            using (var connection = new OleDbConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new OleDbCommand($"SELECT * FROM {typeof(T).Name}s WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        if (typeof(T) == typeof(Product))
                        {
                            (entity as Product).ID = reader.GetInt32(reader.GetOrdinal("ID"));
                            (entity as Product).Email = reader.GetString(reader.GetOrdinal("Email"));
                            (entity as Product).Email = reader.GetString(reader.GetOrdinal("ProductCode"));
                            (entity as Product).Email = reader.GetString(reader.GetOrdinal("ProductName"));
                        }
                    }
                }
            }

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new OleDbCommand($"UPDATE Products SET Email = @Email, ProductCode = @ProductCode, ProductName = @ProductName WHERE ID = @ID", connection);

                if (typeof(T) == typeof(Product))
                {
                    var product = entity as Product;
                    command.Parameters.AddWithValue("@Email", product.Email);
                    command.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@ID", product.ID);
                }

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Product>> GetProductsByEmailAsync(string email)
        {
            var products = new List<Product>();
            var query = "SELECT * FROM Products WHERE Email = @Email";

            using (var connection = new OleDbConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var product = new Product
                            {
                                ID = int.Parse(reader["ID"].ToString()),
                                Email = reader["Email"].ToString(),
                                ProductCode = int.Parse(reader["ProductCode"].ToString()),
                                ProductName = reader["ProductName"].ToString(),
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }
    }
}