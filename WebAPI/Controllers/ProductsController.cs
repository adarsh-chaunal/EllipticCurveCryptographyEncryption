using Domain.Infrastructure;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;

    public ProductsController(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: api/<ProductsController>
    [HttpGet]
    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products.ToListAsync(cancellationToken);

        return products;
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    public async Task<Product?> GetAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(prod => prod.ID == id, cancellationToken);

        return product;
    }

    // POST api/<ProductsController>
    [HttpPost]
    public async Task PostAsync([FromBody] Product product, CancellationToken cancellation)
    {
        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync(cancellation);
    }

    //// PUT api/<ProductsController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<ProductsController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
