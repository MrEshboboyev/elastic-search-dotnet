using DemoElasticSearchInWebApi.Models;

namespace DemoElasticSearchInWebApi.Services;

public interface IProductService
{
    Task IndexProductAsync(Product product);
    Task<List<Product>> GetProductsAsync();
    Task<List<Product>> SearchProduct(string searchTerm);
    Task<List<Product>> SearchProductWithWildCard(string searchTerm);
    Task<List<Product>> FuzzyProductSearch(string searchTerm);
}
