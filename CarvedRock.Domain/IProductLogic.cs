using CarvedRock.Core;
using CarvedRock.Data.Entities;

namespace CarvedRock.Domain;

public interface IProductLogic 
{
    Task<IEnumerable<ProductModel>> GetProductsForCategoryAsync(string category);
    Task<ProductModel?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductModel>> GetProductListForCategoryAsync(string category);
    ProductModel? GetProductById(int id);
    Task<Product> AddNewProductAsync(ProductModel product, bool invalidateCache);
}