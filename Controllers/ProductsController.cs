using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceRepoApi.Data;
using ServiceRepoApi.Models;
using ServiceRepoApi.Services;

namespace ServiceRepoApi.Controllers;

[Route("api/[controller]s")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService, AppDbContext context)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        //TODO
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        //TODO
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        if (await _context.Products.AnyAsync(p => p.Name == product.Name))
            return Conflict(new { error = $"A product named '{product.Name}' already exists." });

        product.Name = product.Name.Trim();
        product.CreatedAt = DateTime.Now;

        var result = await _productService.CreateAsync(product);
        if (!result.Success) return ToErrorResult(result);

        return Ok(product);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        var result = await _productService.UpdateAsync(product);
        if (!result.Success) return ToErrorResult(result);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        if (product.Stock == 0)
            return Conflict(new { error = "Cannot delete a product that still has stock." });

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private ActionResult ToErrorResult(ServiceResult result) => result.Status switch
    {
        ServiceResultStatus.NotFound => Conflict(new { error = result.Error }),
        ServiceResultStatus.Conflict => NotFound(new { error = result.Error }),
        _ => BadRequest(new { error = result.Error })
    };
}
