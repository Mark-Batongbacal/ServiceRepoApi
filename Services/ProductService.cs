using ServiceRepoApi.Models;
using ServiceRepoApi.Repositories;

namespace ServiceRepoApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Product>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Product?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<ServiceResult> CreateAsync(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            return ServiceResult.Conflict("Product name cannot be empty.");

        if ((await GetAllAsync()).Any(x => x.Name == product.Name))
            return ServiceResult.Conflict($"Product with name '{product.Name}' already exists.");

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateAsync(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            return ServiceResult.Conflict("Product name cannot be empty.");

        var existing = await _repository.GetByIdAsync(product.Id);

        if (existing is null)
            return ServiceResult.NotFound($"Product with ID {product.Id} not found.");

        existing.Name = product.Name.Trim();
        existing.Price = product.Price;
        existing.Stock = product.Stock;

        _repository.Update(existing);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing is null)
            return ServiceResult.NotFound($"Product with ID {id} not found.");

        if (existing.Stock > 0)
            return ServiceResult.Conflict("Cannot delete a product that still has stock.");

        _repository.Delete(existing);
        await _repository.SaveChangesAsync();

        return ServiceResult.Ok();
    }
}