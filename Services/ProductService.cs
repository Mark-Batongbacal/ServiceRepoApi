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
        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateAsync(Product product)
    {
        var existing = await _repository.GetByIdAsync(product.Id);
        if (existing is null)
            return ServiceResult.Ok();

        existing.Name = product.Name.Trim();
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.CreatedAt = product.CreatedAt;

        _repository.Update(existing);
        await _repository.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
