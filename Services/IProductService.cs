namespace backendcafe.Services
{
    public interface IProductService
    {
        Task<List<DTO.ProductResponseDTO>> GetProductsByBranchAndCategoryAsync(int branchId, int? categoryId);
        Task<DTO.ProductResponseDTO> GetProductByIdAsync(int id);
        Task<DTO.ProductResponseDTO> CreateProductAsync(DTO.ProductCreateDTO productDto);
        Task<DTO.ProductResponseDTO> UpdateProductAsync(int id, DTO.ProductUpdateDTO productDto);
        Task<bool> DeleteProductAsync(int id);
    }
}