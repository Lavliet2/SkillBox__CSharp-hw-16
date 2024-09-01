using Homework_16.DataAccess;
using Homework_16.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homework_16.Services
{
    public class ClientService
    {
        private readonly SqlLocalDbRepository<Client> _clientRepository;

        public ClientService(string connectionString)
        {
            _clientRepository = new SqlLocalDbRepository<Client>(connectionString);
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task<int> AddClientAsync(Client client)
        {
            return await _clientRepository.AddAsync(client);
        }

        public async Task UpdateClientAsync(Client client)
        {
            await _clientRepository.UpdateAsync(client);
        }

        public async Task DeleteClientAsync(int id)
        {
            await _clientRepository.DeleteAsync(id);
        }
    }
}