using Microsoft.AspNetCore.Mvc;
using ServiceRepoApi.Models;
using ServiceRepoApi.Services;

namespace ServiceRepoApi.Controllers;

[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        return Ok(await _productService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (result == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        var result = await _productService.CreateAsync(product);

        if (!result.Success)
            return ToErrorResult(result);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        product.Id = id;

        var result = await _productService.UpdateAsync(product);

        if (!result.Success)
            return ToErrorResult(result);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result.Success)
            return ToErrorResult(result);

        return NoContent();
    }

    private ActionResult ToErrorResult(ServiceResult result) => result.Status switch
    {
        ServiceResultStatus.NotFound =>
            NotFound(new { error = result.Error }),

        ServiceResultStatus.Conflict =>
            Conflict(new { error = result.Error }),

        _ =>
            BadRequest(new { error = result.Error })
    };
}