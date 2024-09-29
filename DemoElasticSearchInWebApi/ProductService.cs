using DemoElasticSearchInWebApi.Models;
using Elastic.Clients.Elasticsearch;

namespace DemoElasticSearchInWebApi
{
    public class ProductService
    {
        private readonly ElasticsearchClient client;
        private const string IndexName = "products";

        public ProductService()
        {
            client = new ElasticsearchClient(new Uri("http://localhost:9200"));
        }

        public async Task IndexProductAsync(Product product)
        {
            var indexResponse = await client.IndexAsync(product, x => x.Index(IndexName));
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await client.SearchAsync<Product>(s => s.Index(IndexName));
            return response.Hits.Select(hint => hint.Source).ToList();
        }

        public async Task<List<Product>> SearchProduct(string searchTerm)
        {
            var response = await client.SearchAsync<Product>(s => s
                .Index(IndexName)
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            bs => bs.Match(m => m.Field(f => f.Id.ToString()).Query(searchTerm)),
                            bs => bs.Match(m => m.Field(f => f.Name).Query(searchTerm)),
                            bs => bs.Match(m => m.Field(f => f.Price).Query(decimal.Parse(searchTerm)))
                        )
                    )
                )
            );

            if (!response.IsValidResponse)
                return [];
            else
                return response.Hits.Select(hint => hint.Source).ToList();
        }

        public async Task<List<Product>> SearchProductWithWildCard(string searchTerm)
        {
            var response = await client.SearchAsync<Product>(s => s
                .Index(IndexName)
                .Query(q => q
                    .Wildcard(w => w
                        .Field(f => f.Name)
                        .Value($"*{searchTerm}*")
                    )
                )
            );

            if (!response.IsValidResponse)
                return [];
            else
                return response.Hits.Select(hint => hint.Source).ToList();
        }

        public async Task<List<Product>> FuzzyProductSearch(string searchTerm)
        {
            var response = await client.SearchAsync<Product>(s => s
                .Index(IndexName)
                .Query(q => q
                    .Fuzzy(w => w
                        .Field(f => f.Name)
                        .Value(searchTerm)
                        .Fuzziness(new Fuzziness(2))
                    )
                )
            );

            if (!response.IsValidResponse)
                return [];
            else
                return response.Hits.Select(hint => hint.Source).ToList();
        }
    }
}
