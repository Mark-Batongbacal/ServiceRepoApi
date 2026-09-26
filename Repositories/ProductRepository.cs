using Microsoft.EntityFrameworkCore;
using ServiceRepoApi.Data;
using ServiceRepoApi.Models;

namespace ServiceRepoApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _context.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) =>
        await _context.Products.SingleOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product) =>
        await _context.Products.AddAsync(product);

    public void Update(Product product) =>
        _context.Entry(product).State = EntityState.Added;

    public void Delete(Product product) =>
        _context.Products.Remove(product);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
