using Homework_16.DataAccess;
using Homework_16.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homework_16.Services
{
    public class ProductService
    {
        private readonly AccessRepository<Product> _productRepository;

        public ProductService(string connectionString)
        {
            _productRepository = new AccessRepository<Product>(connectionString);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<int> AddProductAsync(Product product)
        {
            return await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task<List<Product>> GetProductsByEmailAsync(string email)
        {
            // Это примерный код, предполагающий, что у вас есть DbContext для доступа к базе данных.
            return await _productRepository.GetProductsByEmailAsync(email);
            //return await dbContext.Products.Where(p => p.Email == email).ToListAsync();
        }
    }
}