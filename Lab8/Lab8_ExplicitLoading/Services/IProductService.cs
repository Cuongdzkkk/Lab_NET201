// Services/IProductService.cs
// Interface định nghĩa các phương thức service cho Product
// Demo Explicit Loading

using Lab8_ExplicitLoading.Models;

namespace Lab8_ExplicitLoading.Services
{
    /// <summary>
    /// Interface cho ProductService
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Lấy tất cả products - KHÔNG load category
        /// </summary>
        Task<List<Product>> GetAllProductsAsync();

        /// <summary>
        /// Lấy 1 product và load Category thủ công với EXPLICIT LOADING
        /// Sử dụng Entry().Reference().Load()
        /// </summary>
        Task<Product?> GetProductWithCategoryExplicitAsync(int productId);

        /// <summary>
        /// Lấy tất cả products và load Category cho từng sản phẩm
        /// Demo Explicit Loading với nhiều entities
        /// </summary>
        Task<List<Product>> GetAllProductsWithExplicitCategoryAsync();

        /// <summary>
        /// Lấy 1 Category và load Products của nó thủ công
        /// Sử dụng Entry().Collection().Load()
        /// </summary>
        Task<Category?> GetCategoryWithProductsExplicitAsync(int categoryId);

        /// <summary>
        /// Demo chi tiết Explicit Loading với log
        /// </summary>
        Task<(Product? Product, List<string> QueryLogs)> GetProductWithExplicitLoadingDemoAsync(int productId);
    }
}
