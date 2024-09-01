using Homework_16.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Homework_16.DataAccess
{
    public class SqlLocalDbRepository<T> : IRepository<T> where T : Client, new()
    {
        private readonly string _connectionString;

        public SqlLocalDbRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddAsync(T entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string insertQuery = "INSERT INTO Clients (LastName, FirstName, MiddleName, PhoneNumber, Email) VALUES (@LastName, @FirstName, @MiddleName, @PhoneNumber, @Email);";
                string identityQuery = "SELECT CAST(SCOPE_IDENTITY() AS INT);";
                var command = new SqlCommand(insertQuery + identityQuery, connection);

                command.Parameters.AddWithValue("@LastName", entity.LastName);
                command.Parameters.AddWithValue("@FirstName", entity.FirstName);
                command.Parameters.AddWithValue("@MiddleName", entity.MiddleName);
                command.Parameters.AddWithValue("@PhoneNumber", (object)entity.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", entity.Email);

                object result = await command.ExecuteScalarAsync();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0; // Возвращаем 0 или ID
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand($"DELETE FROM {typeof(T).Name}s WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var clients = new List<T>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Clients", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var client = new T
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("ID")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            MiddleName = reader.GetString(reader.GetOrdinal("MiddleName")),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            Email = reader.GetString(reader.GetOrdinal("Email"))
                        };
                        clients.Add(client);
                    }
                }
            }

            return clients;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            T entity = new T();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand($"SELECT * FROM {typeof(T).Name}s WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        if (typeof(T) == typeof(Client))
                        {
                            (entity as Client).ID = reader.GetInt32(reader.GetOrdinal("ID"));
                            (entity as Client).LastName = reader.GetString(reader.GetOrdinal("LastName"));
                            (entity as Client).FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                            (entity as Client).FirstName = reader.GetString(reader.GetOrdinal("MiddleName"));
                            (entity as Client).FirstName = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                            (entity as Client).FirstName = reader.GetString(reader.GetOrdinal("Email"));
                        }
                    }
                }
            }

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand($"UPDATE {typeof(T).Name}s SET LastName = @LastName, FirstName = @FirstName, MiddleName = @MiddleName, PhoneNumber = @PhoneNumber, Email = @Email WHERE ID = @ID", connection);

                if (typeof(T) == typeof(Client))
                {
                    var client = entity as Client;
                    command.Parameters.AddWithValue("@LastName", client.LastName);
                    command.Parameters.AddWithValue("@FirstName", client.FirstName);
                    command.Parameters.AddWithValue("@MiddleName", client.MiddleName);
                    command.Parameters.AddWithValue("@PhoneNumber", (object)client.PhoneNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", client.Email);
                    command.Parameters.AddWithValue("@ID", client.ID);
                }

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}